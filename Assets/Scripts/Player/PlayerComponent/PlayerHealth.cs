using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("当前属性")]
    [Tooltip("当前血量")][SerializeField] private float currentHealth;
    [Tooltip("当前能否触发名刀")][SerializeField] private bool currentHasCheatDeath;

    [Header("基础属性")]
    [Tooltip("血量上限")][SerializeField] private float maxHealth;
    [Tooltip("护甲值")][SerializeField] private float armor;
    [Tooltip("生命恢复")][SerializeField] private float healthRegen;
    [Tooltip("闪避率")][SerializeField] private float dodgeChance;
    [Tooltip("碰撞伤害")][SerializeField] private float collisionDamage;
    [Tooltip("碰撞受击无敌时间")][SerializeField] private float collisionImmunityDuration;
    [Tooltip("是否有名刀")][SerializeField] private bool hasCheatDeath;
    [Tooltip("名刀无敌时间")][SerializeField] private float cheatDeathInvincibleTime;

    [Header("无敌效果")]
    public bool isInvincible; // 当前是否无敌
    public float invincibleDuration; // 无敌剩余时间

    //碰撞
    private float lastCollisionDamageTime;
    private bool isCollisionImmune => Time.time - lastCollisionDamageTime < collisionImmunityDuration;

    private SpriteRenderer spriteRenderer;//图片引用

    // 死亡事件
    public event Action OnDeath;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        RegenerateHealth();
    }

    #region PlayerCore相关
    public void EnableHealth()
    {
    }

    public void DisableHealth()
    {
    }

    /// <summary>
    /// 玩家血量系统初始化
    /// </summary>
    public void Initialize(PlayerSO playerData)
    {
        maxHealth = playerData.healthConfig.maxHealth;
        armor = playerData.healthConfig.armor;
        healthRegen = playerData.healthConfig.healthRegen;
        dodgeChance = playerData.healthConfig.dodgeChance;
        collisionDamage = playerData.healthConfig.collisionDamage;
        collisionImmunityDuration = playerData.healthConfig.collisionImmunityDuration;

        cheatDeathInvincibleTime = playerData.healthConfig.cheatDeathInvincibleTime;
        hasCheatDeath = playerData.healthConfig.hasCheatDeath;

        ResetToBaseStats();
    }

    /// <summary>
    /// 刷新状态
    /// </summary>
    public void ResetToBaseStats()
    {
        currentHealth = maxHealth;
        currentHasCheatDeath = hasCheatDeath;
        RemoveInvincible();
    }
    #endregion

    #region 基本受伤与死亡逻辑
    /// <summary>
    /// 生命恢复
    /// </summary>
    private void RegenerateHealth()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += healthRegen * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
    }

    /// <summary>
    /// 玩家受击
    /// </summary>
    /// <param name="damage">原伤害</param>
    public void TakeDamage(float damage, DamageSource source = DamageSource.Enemy)
    {
        if (PauseManager.Instance.IsPaused) return;

        //无敌判定
        if (isInvincible) return;

        // 闪避判定
        if (UnityEngine.Random.value < dodgeChance)
        {
            Debug.Log("闪避了攻击!");
            return;
        }

        // 护甲减伤
        float damageTaken = Mathf.Max(0, damage - armor); // 伤害下限为0

        // 致命伤害检查,名刀
        if (damageTaken >= currentHealth)
        {
            if (currentHasCheatDeath && hasCheatDeath)
            {
                ApplyCheatDeath();
                return;
            }
        }

        currentHealth -= damageTaken;
        Debug.Log($"{source} 对Player造成了 {damageTaken} 伤害! Player当前血量: {currentHealth}");

        StartCoroutine(HitAnimationRoutine());//简单动画

        if (currentHealth <= 0)
        {
            Die(source);
        }
    }

    private IEnumerator HitAnimationRoutine()
    {
        float duration = 0.5f;
        float blinkSpeed = 0.1f;
        float timer = 0;

        while (timer < duration)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(blinkSpeed);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(blinkSpeed);
            timer += blinkSpeed * 2;
        }
    }

    public bool TryTakeCollisionDamage(float damage)
    {
        if (isCollisionImmune) return false;

        TakeDamage(damage);
        lastCollisionDamageTime = Time.time;
        return true;
    }

    public float GetCollisionDamage()
    {
        return collisionDamage;
    }

    /// <summary>
    /// 玩家死亡
    /// </summary>
    private void Die(DamageSource source)
    {
        Debug.Log("玩家死亡!");
        OnDeath?.Invoke(); // 通知Core死亡事件
    }

    #endregion

    #region 额外效果
    /// <summary>
    /// 应用名刀
    /// </summary>
    public void ApplyCheatDeath()
    {
        currentHasCheatDeath = false;
        AddInvincible(cheatDeathInvincibleTime);
        Debug.Log("触发名刀!");
    }

    /// <summary>
    /// 添加无敌效果
    /// </summary>
    /// <param name="duration">无敌持续时间(秒)，-1表示永久</param>
    public void AddInvincible(float duration)
    {
        isInvincible = true;
        invincibleDuration = duration;

        if (duration > 0)
        {
            StartCoroutine(InvincibleTimerRoutine());
        }
    }

    private IEnumerator InvincibleTimerRoutine()
    {
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
        invincibleDuration = 0;
        Debug.Log("无敌时间结束");
    }

    /// <summary>
    /// 取消无敌效果
    /// </summary>
    public void RemoveInvincible()
    {
        isInvincible = false;
        invincibleDuration = 0;
        StopCoroutine("InvincibleTimerRoutine");
    }
    #endregion

    #region 公共属性

    public float MaxHealth => maxHealth;
    public float Armor => armor;
    public float HealthRegen => healthRegen;
    public float DodgeChance => dodgeChance;
    public float CollisionDamage => collisionDamage;
    public float CurrentHealth => currentHealth;

    #endregion 公共属性

    #region 增益效果相关方法

    public void AddMaxHealth(float amount) => maxHealth += amount;

    public void AddArmor(float amount) => armor += amount;

    public void AddHealthRegen(float amount) => healthRegen += amount;

    public void AddCollitionDamage(float amount) => collisionDamage += amount;

    public void AddDodgeChance(float amount) => dodgeChance = Mathf.Min(dodgeChance + amount, 0.6f);

    public void SetCheatDeath(float invincibleTime)
    {
        hasCheatDeath = true;
        cheatDeathInvincibleTime = invincibleTime;
    }

    //临时
    public void AddCurrentHealth(float amount) => currentHealth += amount;

    #endregion 增益效果相关方法
}