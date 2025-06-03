using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    /// <summary>
    /// 角色技能种类
    /// </summary>
    public enum AbilityType
    {
        Classic,    // 经典吐豆人：获得10秒无敌
        Berserk,    // 狂暴吐豆人：短时间大幅提升攻速
        Skilled     // 会玩的吐豆人：刷新次数+1
    }

    [Header("冷却和持续时间")]
    public AbilityType currentAbility;
    [SerializeField] private float classicCooldown = 5f;
    [SerializeField] private float berserkCooldown = 2f;
    [SerializeField] private float classicDuration = 10f;
    [SerializeField] private float berserkDuration = 5f;
    [SerializeField] private float berserkFireRateMultiplier = 2f;

    [Header("State")]
    public int extraRefreshChances;
    public bool isAbilityActive;
    private float cooldownTimer;
    private float abilityDurationTimer;

    private PlayerCore playerCore;

    private void Awake()
    {
        playerCore = GetComponent<PlayerCore>();
    }

    private void Update()
    {
        UpdateTimers();
    }

    private void UpdateTimers()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (isAbilityActive)
        {
            abilityDurationTimer -= Time.deltaTime;
            if (abilityDurationTimer <= 0)
            {
                DeactivateAbility();
            }
        }
    }

    public void ActivateAbility()
    {
        if (cooldownTimer > 0 || isAbilityActive) return;

        isAbilityActive = true;
        abilityDurationTimer = currentAbility == AbilityType.Classic ? classicDuration : berserkDuration;

        switch (currentAbility)
        {
            case AbilityType.Classic:
                // 无敌效果在PlayerHealth中实现
                playerCore.Health.SetCheatDeath(true);
                break;
            case AbilityType.Berserk:
                playerCore.Shooting.AddFireRate(playerCore.Shooting.fireRate * berserkFireRateMultiplier);
                break;
            case AbilityType.Skilled:
                extraRefreshChances++;
                // UI更新
                break;
        }

        // 设置冷却时间
        cooldownTimer = currentAbility == AbilityType.Classic ? classicCooldown : berserkCooldown;
    }

    private void DeactivateAbility()
    {
        isAbilityActive = false;

        switch (currentAbility)
        {
            case AbilityType.Classic:
                playerCore.Health.SetCheatDeath(false);
                break;
            case AbilityType.Berserk:
                playerCore.Shooting.ResetToBaseStats();
                break;
        }
    }

    public void ChangeAbility(AbilityType newAbility)
    {
        if (isAbilityActive) DeactivateAbility();
        currentAbility = newAbility;
        cooldownTimer = 0;
    }
}