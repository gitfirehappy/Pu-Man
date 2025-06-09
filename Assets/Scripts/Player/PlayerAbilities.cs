using UnityEngine;
using System.Collections;
using DG.Tweening.Core.Easing;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    /// <summary>
    /// 角色技能种类
    /// </summary>
    public enum AbilityType
    {
        None,
        Classic,    // 经典吐豆人：获得无敌
        Berserk,    // 狂暴吐豆人：短时间大幅提升攻速
        Skilled,    // 会玩的吐豆人：刷新次数+1
        CheatDeath,  // 名刀效果：每波次触发一次免死，通过buff获得
        ChainKill,  //亵渎清场,通过buff获取
    }

    [Header("主动技能冷却")]
    public AbilityType currentAbility;
    [SerializeField] private int classicCooldownWaves = 5;
    [SerializeField] private int berserkCooldownWaves = 3;
    [SerializeField] private int chainkillCooldownWaves = 2;
    [SerializeField] private float classicDuration = 10f;
    [SerializeField] private float berserkDuration = 5f;
    [SerializeField] private float berserkFireRateMultiplier = 2f;

    [Header("被动效果")]
    [SerializeField] private int extraRefreshChancesPerWave = 1; // 每波增加的刷新次数
    public bool hasCheatDeath; // 名刀效果
    public int cheatDeathCooldownWaves = 1; // 名刀冷却波次

    [Header("当前状态")]
    public int nextAvailableWave = 0; // 下次可用技能的波次
    public int nextCheatDeathAvailableWave = 0; // 下次可用名刀的波次
    public bool isAbilityActive;
    private Coroutine activeAbilityCoroutine;

    private PlayerCore playerCore;
    private PlayerInput playerInput;
    private int currentWave;//临时变量，当前波次数,后续需要从关卡系统中获取该变量 

    private void Awake()
    {
        playerCore = GetComponent<PlayerCore>();
        playerInput = new PlayerInput();
        playerInput.Player.Ability.started += ActivateAbility;

    }


    /// <summary>
    /// 使用主动技能
    /// </summary>
    public void ActivateAbility(InputAction.CallbackContext context)
    {
        if (currentWave <= nextAvailableWave || isAbilityActive) return;

        isAbilityActive = true;

        switch (currentAbility)
        {
            case AbilityType.Classic:
                nextAvailableWave = currentWave + classicCooldownWaves;
                ClassicAbility();
                break;
            case AbilityType.Berserk:
                nextAvailableWave = currentWave + berserkCooldownWaves;
                activeAbilityCoroutine = StartCoroutine(BerserkAbilityRoutine());
                break;
            case AbilityType.ChainKill:
                nextAvailableWave = currentWave + chainkillCooldownWaves;
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
        playerCore.Health.AddInvincible(classicDuration);
        isAbilityActive = false;
    }

    /// <summary>
    /// 加攻速
    /// </summary>
    /// <returns></returns>
    private IEnumerator BerserkAbilityRoutine()
    {
        Debug.Log("释放了狂暴技能！");
        float originalFireRate = playerCore.Shooting.fireRate;
        playerCore.Shooting.AddFireRate(originalFireRate * berserkFireRateMultiplier);
        yield return new WaitForSeconds(berserkDuration);
        playerCore.Shooting.ResetToBaseStats();
        isAbilityActive = false;
    }

    /// <summary>
    /// 名刀
    /// </summary>
    public void TryApplyCheatDeath()
    {
        Debug.Log("触发了名刀！");
        hasCheatDeath = false;
        nextCheatDeathAvailableWave = currentWave + classicCooldownWaves;
    }

    /// <summary>
    /// 重置冷却
    /// </summary>
    public void ResetAbilityCooldown()
    {
        nextAvailableWave = currentWave++;
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
        // 实现亵渎技能 - 需要在EnemyManager中实现
        // StartCoroutine(ChainKillRoutine());

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

    public void ChangeAbility(AbilityType newAbility)
    {
        if (isAbilityActive && activeAbilityCoroutine != null)
        {
            StopCoroutine(activeAbilityCoroutine);
            DeactivateAbility();
        }
        currentAbility = newAbility;
        ResetAbilityCooldown();
        Debug.Log("当前技能:"+currentAbility);
    }

    private void DeactivateAbility()
    {
        switch (currentAbility)
        {
            case AbilityType.Berserk:
                playerCore.Shooting.ResetToBaseStats();
                break;
        }
        isAbilityActive = false;
    }
}