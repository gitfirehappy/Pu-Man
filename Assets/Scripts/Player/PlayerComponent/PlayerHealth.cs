using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("属性")]
    [SerializeField][Header("当前血量")] private float currentHealth;
    [SerializeField][Header("当前能否触发名刀")] private bool currentHasCheatDeath;


    [SerializeField][Header("血量上限")] private float maxHealth;
    [SerializeField][Header("护甲值")]   private float armor;
    [SerializeField][Header("生命恢复")] private float healthRegen;
    [SerializeField][Header("闪避率")]   private float dodgeChance;
    [SerializeField][Header("碰撞伤害")]   private float collisionDamage;
    [SerializeField][Header("碰撞受击无敌时间")] private float collisionImmunityDuration;
    [SerializeField][Header("是否有名刀")] private bool hasCheatDeath;
    [SerializeField][Header("名刀无敌时间")] private float cheatDeathInvincibleTime;


    [Header("无敌效果")]
    public bool isInvincible; // 当前是否无敌
    public float invincibleDuration; // 无敌剩余时间

    //碰撞
    private float lastCollisionDamageTime;
    private bool isCollisionImmune => Time.time - lastCollisionDamageTime < collisionImmunityDuration;

    private void Update()
    {
        RegenerateHealth();
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

        currentHealth = maxHealth;
    }

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
    public void TakeDamage(float damage)
    {
        //无敌判定
        if (isInvincible) return;

        // 闪避判定
        if (Random.value < dodgeChance)
        {
            Debug.Log("Dodged the attack!");
            return;
        }

        // 护甲减伤
        float damageTaken = damage - armor; 

        // 致命伤害检查,名刀
        if (damageTaken >= currentHealth)
        {
            TryApplyCheatDeath();
            AddInvincible(cheatDeathInvincibleTime);
            Debug.Log("Cheat death activated!");
            return;
        }

        currentHealth -= damageTaken;
        Debug.Log($"Player took {damageTaken} damage! Current HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
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
    /// 应用名刀
    /// </summary>
    public void TryApplyCheatDeath()
    {
        if (hasCheatDeath && currentHasCheatDeath)
        {
            currentHasCheatDeath = false;
        }
    }


    /// <summary>
    /// 玩家死亡
    /// </summary>
    private void Die()
    {
        Debug.Log("Player died!");
        // 游戏结束逻辑
    }

    /// <summary>
    /// 添加无敌效果
    /// </summary>
    /// <param name="duration">无敌持续时间(秒)，-1表示永久</param>
    public void AddInvincible(float duration)
    {
        isInvincible = true;

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

    /// <summary>
    /// 重置状态
    /// </summary>
    public void ResetToBaseStats()
    {
        currentHealth = maxHealth;
        RemoveInvincible();

    }

    #region 公共属性
    public float MaxHealth => maxHealth;
    public float Armor => armor;
    public float HealthRegen => healthRegen;
    public float DodgeChance => dodgeChance;
    public float CollisionDamage => collisionDamage;


    #endregion

    #region 增益效果相关方法

    public void AddMaxHealth(float amount) => maxHealth += amount;
    public void AddArmor(float amount) => armor += amount;
    public void AddHealthRegen(float amount) => healthRegen += amount;
    public void AddCollitionDamage(float amount) => collisionDamage += amount;
    public void AddDodgeChance(float amount) => dodgeChance = Mathf.Min(dodgeChance + amount, 0.6f);
    public void SetCheatDeath(bool value) => hasCheatDeath = value;

    //临时
    public void AddCurrentHealth(float amount) => currentHealth += amount;

    #endregion

}