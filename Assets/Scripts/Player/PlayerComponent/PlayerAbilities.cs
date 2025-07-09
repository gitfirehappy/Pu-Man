using UnityEngine;
using System.Collections;
using DG.Tweening.Core.Easing;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    [Header("主动技能冷却")]
    [SerializeField] private AbilityType baseAbility;
    [SerializeField] private int baseClassicCooldownWaves;
    [SerializeField] private int baseBerserkCooldownWaves;
    [SerializeField] private int baseChainkillCooldownWaves;
    [SerializeField] private float baseClassicDuration;
    [SerializeField] private float baseBerserkDuration;
    [SerializeField] private float baseBerserkFireRateMultiplier;

    [Header("当前属性")]
    private AbilityType currentAbility;
    private int currentClassicCooldownWaves;
    private int currentBerserkCooldownWaves;
    private int currentChainkillCooldownWaves;
    private float currentClassicDuration;
    private float currentBerserkDuration;
    private float currentBerserkFireRateMultiplier;

    [Header("被动效果")]
    [SerializeField] private int extraRefreshChancesPerWave = 1;

    [Header("当前状态")]
    public int nextAvailableWave; // 下次可用技能的波次
    public bool isAbilityActive;

    private Coroutine activeAbilityCoroutine;
    private PlayerCore playerCore;
    private PlayerInput playerInput;
    private WaveCounter waveCounter;

    private void Awake()
    {
        playerCore = GetComponent<PlayerCore>();
        waveCounter = WaveCounter.Instance;
        if (playerCore != null && playerCore.playerInput != null)
        {
            playerCore.playerInput.Player.Ability.started += ActivateAbility;
        }
        else
        {
            Debug.LogError("PlayerCore 或 PlayerInput 未正确初始化！");
        }
    }

    public void DisableAbilities()
    {
        // 强制中断当前技能
        if (isAbilityActive)
        {
            DeactivateAbility();
        }
    }

    public void EnableAbilities()
    {
    }

    private void OnEnable()
    {
        EventBus.OnWaveChanged += OnWaveChanged;
    }

    private void OnDisable()
    {
        EventBus.OnWaveChanged -= OnWaveChanged;
    }

    /// <summary>
    /// 玩家技能系统初始化
    /// </summary>
    public void Initialize(PlayerSO playerData)
    {
        nextAvailableWave = 0;

        baseAbility = playerData.abilitiesConfig.startingAbility;
        baseClassicCooldownWaves = playerData.abilitiesConfig.classicCooldownWaves;
        baseBerserkCooldownWaves = playerData.abilitiesConfig.berserkCooldownWaves;
        baseChainkillCooldownWaves = playerData.abilitiesConfig.chainkillCooldownWaves;
        baseClassicDuration = playerData.abilitiesConfig.classicDuration;
        baseBerserkDuration = playerData.abilitiesConfig.berserkDuration;
        baseBerserkFireRateMultiplier = playerData.abilitiesConfig.berserkFireRateMultiplier;

        ResetToBaseStats(); // 初始化时调用重置方法
    }

    /// <summary>
    /// 重置为初始状态（每波开始时调用）
    /// </summary>
    public void ResetToBaseStats()
    {
        // 重置当前属性为基准值
        currentAbility = baseAbility;
        currentClassicCooldownWaves = baseClassicCooldownWaves;
        currentBerserkCooldownWaves = baseBerserkCooldownWaves;
        currentChainkillCooldownWaves = baseChainkillCooldownWaves;
        currentClassicDuration = baseClassicDuration;
        currentBerserkDuration = baseBerserkDuration;
        currentBerserkFireRateMultiplier = baseBerserkFireRateMultiplier;

        // 重置技能状态
        isAbilityActive = false;

        // 如果有正在运行的技能协程，停止它
        if (activeAbilityCoroutine != null)
        {
            StopCoroutine(activeAbilityCoroutine);
            DeactivateAbility();
        }
    }

    /// <summary>
    /// 使用主动技能
    /// </summary>
    public void ActivateAbility(InputAction.CallbackContext context)
    {
        if (waveCounter == null) return;

        int currentWave = waveCounter.CurrentWave;

        if (currentWave <= nextAvailableWave || isAbilityActive)
        {
            Debug.Log("技能不可用");
            return;
        }

        isAbilityActive = true;

        switch (currentAbility)
        {
            case AbilityType.Classic:
                nextAvailableWave = currentWave + currentClassicCooldownWaves;
                ClassicAbility();
                break;

            case AbilityType.Berserk:
                nextAvailableWave = currentWave + currentBerserkCooldownWaves;
                activeAbilityCoroutine = StartCoroutine(BerserkAbilityRoutine());
                break;

            case AbilityType.ChainKill:
                nextAvailableWave = currentWave + currentChainkillCooldownWaves;
                ChainKill();
                break;
        }
    }

    /// <summary>
    /// 无敌技能
    /// </summary>
    private void ClassicAbility()
    {
        Debug.Log("释放了无敌技能！");
        playerCore.Health.AddInvincible(currentClassicDuration);
        isAbilityActive = false;
    }

    /// <summary>
    /// 加攻速
    /// </summary>
    /// <returns></returns>
    private IEnumerator BerserkAbilityRoutine()
    {
        Debug.Log("释放了狂暴技能！");
        float originalFireRate = playerCore.Shooting.FireRate;
        playerCore.Shooting.AddCurrentFireRate(originalFireRate * currentBerserkFireRateMultiplier);
        yield return new WaitForSeconds(currentBerserkDuration);
        playerCore.Shooting.AddCurrentFireRate(-originalFireRate * currentBerserkFireRateMultiplier);
        isAbilityActive = false;
        Debug.Log("狂暴技能结束");
    }

    /// <summary>
    /// 波次变化加刷新次数
    /// </summary>
    /// <param name="wave"></param>
    private void OnWaveChanged(int wave)
    {
        // 拥有Skilled技能的玩家获得每波刷新次数
        if (currentAbility == AbilityType.Skilled)
        {
            var buffUIManager = GameUIManager.Instance?.GetSubUIManager<SelectBuffUIManager>();
            if (buffUIManager != null)
            {
                buffUIManager.AddRefreshCount(extraRefreshChancesPerWave); // 每波增加刷新机会
                Debug.Log($"Skilled技能生效: 获得1次额外刷新机会 (当前波次: {wave})");
            }
        }
    }

    /// <summary>
    /// 重置冷却
    /// </summary>
    public void ResetAbilityCooldown()
    {
        if (waveCounter != null)
        {
            nextAvailableWave = waveCounter.CurrentWave;
        }
    }

    /// <summary>
    /// 减少技能冷却
    /// </summary>
    /// <param name="reduceWave"></param>
    public void ReduceAbilityCooldown(int reduceWave)
    {
        nextAvailableWave -= reduceWave;
    }

    /// <summary>
    /// 亵渎技能
    /// </summary>
    public void ChainKill()
    {
        Debug.Log("释放了亵渎技能！");

        // 获取玩家位置作为技能中心
        Vector2 origin = playerCore.transform.position;

        // 设置技能半径（可根据需要调整或从配置读取）
        float radius = 5f;
        float damage = 1f;

        // 调用EnemyManager执行亵渎技能
        EnemyManager.Instance.ChainKill(origin, radius, damage);

        // 播放特效和音效

        isAbilityActive = false;
    }

    /// <summary>
    /// 随机替换技能（仅在3个初始技能间随机）
    /// </summary>
    public void RandomizeAbility()
    {
        // 定义初始技能池
        AbilityType[] initialAbilities = new AbilityType[]
        {
        AbilityType.Classic,
        AbilityType.Berserk,
        AbilityType.Skilled
        };

        // 从初始技能池中随机选择（排除当前技能）
        AbilityType newAbility;
        do
        {
            newAbility = initialAbilities[Random.Range(0, initialAbilities.Length)];
        } while (newAbility == currentAbility);

        ChangeAbility(newAbility);
    }

    /// <summary>
    /// 更换技能
    /// </summary>
    /// <param name="newAbility"></param>
    public void ChangeAbility(AbilityType newAbility)
    {
        if (isAbilityActive && activeAbilityCoroutine != null)
        {
            StopCoroutine(activeAbilityCoroutine);
            DeactivateAbility();
        }
        currentAbility = newAbility;
        ResetAbilityCooldown();
        Debug.Log("当前技能:" + currentAbility);
    }

    /// <summary>
    /// 中断技能
    /// </summary>
    private void DeactivateAbility()
    {
        switch (currentAbility)
        {
            case AbilityType.Berserk:
                //重置射击
                playerCore.Shooting.ResetToBaseStats();
                break;

            case AbilityType.Classic:
                playerCore.Health.RemoveInvincible();
                break;
        }
        isAbilityActive = false;
    }

    public int GetCooldownWaves()
    {
        switch (currentAbility)
        {
            case AbilityType.Classic: return currentClassicCooldownWaves;
            case AbilityType.Berserk: return currentBerserkCooldownWaves;
            case AbilityType.ChainKill: return currentChainkillCooldownWaves;
            default: return 0;
        }
    }
}

/// <summary>
/// 角色技能种类
/// </summary>
public enum AbilityType
{
    None,
    Classic,    // 经典吐豆人：获得无敌
    Berserk,    // 狂暴吐豆人：短时间大幅提升攻速
    Skilled,    // 会玩的吐豆人：刷新次数+1
    ChainKill,  //亵渎清场,通过buff获取
}