using UnityEngine;
using System.Collections;
using DG.Tweening.Core.Easing;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    [Header("基准技能配置")]
    [SerializeField] private AbilityData baseAbilityData;

    [Header("当前技能状态")]
    [SerializeField]private AbilityData currentAbilityData;

    public int nextAvailableWave; // 下次可用技能的波次
    public bool isAbilityActive;

    [SerializeField] private AudioClip abilityActivationSFX;

    private Coroutine activeAbilityCoroutine;
    private PlayerCore playerCore;
    private WaveCounter waveCounter;
    private InvertEffect invertEffect;
    private BattleUIPanel battleUIPanel;

    private void Awake()
    {
        playerCore = GetComponent<PlayerCore>();
        waveCounter = WaveCounter.Instance;
        battleUIPanel = UIManager.Instance.GetForm<BattleUIPanel>();
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

    /// <summary>
    /// 玩家技能系统初始化
    /// </summary>
    public void Initialize(PlayerSO playerData)
    {
        if (playerCore != null && playerCore.playerInput != null)
        {
            playerCore.playerInput.Player.Ability.started += ActivateAbility;
        }
        else
        {
            Debug.LogError("PlayerCore 或 PlayerInput 未正确初始化！");
        }

        // 初始化反色效果
        var mainCam = Camera.main;
        if (mainCam != null && invertEffect == null)
        {
            invertEffect = mainCam.GetComponent<InvertEffect>();
            if (invertEffect == null)
            {
                invertEffect = mainCam.gameObject.AddComponent<InvertEffect>();
                invertEffect.fadeDuration = 0.3f; // 可配置渐变时长
            }
        }

        nextAvailableWave = 0;

        abilityActivationSFX = playerData.abilitiesConfig.abilityActivationSFX;

        baseAbilityData = playerData.abilitiesConfig.startingAbilityData;

        EventQueueManager.AddStateEvent(GameState.Battle, () =>
        {
            OnWaveChanged(WaveCounter.Instance.CurrentWave);
        }, 8);

        ResetToBaseStats(); // 初始化时调用重置方法
    }

    /// <summary>
    /// 重置为初始状态（每波开始时调用）
    /// </summary>
    public void ResetToBaseStats()
    {
        // 重置当前属性为基准值
        currentAbilityData = baseAbilityData;

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
        if (PauseManager.Instance.IsPaused) return;

        if (waveCounter == null) return;

        int currentWave = waveCounter.CurrentWave;

        if (currentWave <= nextAvailableWave || isAbilityActive)
        {
            Debug.Log("技能不可用");
            return;
        }

        isAbilityActive = true;

        if (abilityActivationSFX != null)
        {
            AudioManager.Instance.PlaySFX(abilityActivationSFX);
        }

        switch (currentAbilityData.type)
        {
            case AbilityType.Classic:
                nextAvailableWave = currentWave + currentAbilityData.cooldownWaves;
                ClassicAbility();
                break;

            case AbilityType.Berserk:
                nextAvailableWave = currentWave + currentAbilityData.cooldownWaves;
                activeAbilityCoroutine = StartCoroutine(BerserkAbilityRoutine());
                break;

            case AbilityType.ChainKill:
                nextAvailableWave = currentWave + currentAbilityData.cooldownWaves;
                ChainKill();
                break;
        }

        // 立即更新技能冷却UI
        battleUIPanel.UpdateSkillCooldownUI();
    }

    /// <summary>
    /// 无敌技能
    /// </summary>
    private void ClassicAbility()
    {
        Debug.Log("释放了无敌技能！");
        playerCore.Health.AddInvincible(currentAbilityData.duration);

        // 启动渐变反色
        if (invertEffect != null)
        {
            invertEffect.ToggleEffect(true);
            StartCoroutine(EndEffectAfterDuration(currentAbilityData.duration));
        }

        isAbilityActive = false;
    }

    private IEnumerator EndEffectAfterDuration(float delay)
    {
        yield return new WaitForSeconds(delay - invertEffect.fadeDuration);
        invertEffect?.ToggleEffect(false);
    }

    /// <summary>
    /// 加攻速
    /// </summary>
    /// <returns></returns>
    private IEnumerator BerserkAbilityRoutine()
    {
        Debug.Log("释放了狂暴技能！");
        float originalFireRate = playerCore.Shooting.FireRate;
        playerCore.Shooting.AddCurrentFireRate(originalFireRate * currentAbilityData.effectValue);
        yield return new WaitForSeconds(currentAbilityData.duration);
        playerCore.Shooting.AddCurrentFireRate(-originalFireRate * currentAbilityData.effectValue);
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
        if (currentAbilityData.type == AbilityType.Skilled)
        {
            var buffUIManager = GameUIManager.Instance?.GetSubUIManager<SelectBuffUIManager>();
            if (buffUIManager != null)
            {
                buffUIManager.AddRefreshCount(currentAbilityData.extraRefreshPerWave); // 每波增加刷新机会
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
        int maxChains = 14;

        // 调用EnemyManager执行亵渎技能
        EnemyManager.Instance.ChainKill(origin, radius, damage, maxChains);

        // 播放特效和音效

        isAbilityActive = false;
    }

    /// <summary>
    /// 随机替换技能（仅在3个初始技能间随机）
    /// </summary>
    public void RandomizeAbility()
    {
        AbilityType[] initialAbilities = { AbilityType.Classic, AbilityType.Berserk, AbilityType.Skilled };
        AbilityType newType;
        do
        {
            newType = initialAbilities[Random.Range(0, initialAbilities.Length)];
        } while (newType == currentAbilityData.type);

        ChangeAbility(new AbilityData(newType));
    }
    
    /// <summary>
    /// 更换技能
    /// </summary>
    public void ChangeAbility(AbilityData newAbilityData)
    {
        if (isAbilityActive && activeAbilityCoroutine != null)
        {
            StopCoroutine(activeAbilityCoroutine);
            DeactivateAbility();
        }
        currentAbilityData = newAbilityData;
        ResetAbilityCooldown();
        Debug.Log($"技能已切换为: {currentAbilityData.type}，冷却: {currentAbilityData.cooldownWaves}波");
    }

    /// <summary>
    /// 中断技能
    /// </summary>
    private void DeactivateAbility()
    {
        switch (currentAbilityData.type)
        {
            case AbilityType.Berserk:
                //重置射击
                playerCore.Shooting.ResetToBaseStats();
                break;

            case AbilityType.Classic:
                playerCore.Health.RemoveInvincible();
                invertEffect?.ToggleEffect(false);
                break;
        }
        isAbilityActive = false;
    }

    public int GetCooldownWaves()
    {
        return currentAbilityData.cooldownWaves;
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

[System.Serializable]
public struct AbilityData
{
    public AbilityType type;
    public int cooldownWaves;  // 冷却波次
    public float duration;     // 持续时间（如无敌/狂暴的持续时间）
    public float effectValue;  // 效果数值（如狂暴的攻速倍率）

    public bool isPassive;         // 是否为被动技能
    public int extraRefreshPerWave; // 每波增加的刷新次数（仅Skilled技能需要）

    // 构造函数：为不同技能类型提供默认值
    public AbilityData(AbilityType type)
    {
        this.type = type;
        cooldownWaves = 0;
        duration = 0;
        effectValue = 0;
        isPassive = false;
        extraRefreshPerWave = 0;

        // 根据类型设置默认值（可选）
        switch (type)
        {
            case AbilityType.Classic:
                cooldownWaves = 5;
                duration = 10f;
                break;
            case AbilityType.Berserk:
                cooldownWaves = 3;
                duration = 5f;
                effectValue = 1.5f; // 攻速倍率
                break;
            case AbilityType.Skilled:
                isPassive = true;
                extraRefreshPerWave = 1; // 默认每波+1次刷新
                break;
            case AbilityType.ChainKill:
                cooldownWaves = 2; // 默认冷却
                effectValue = 1f;
                break;
        }
    }
}