using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("基础属性")]
    [SerializeField] private float baseMaxHealth = 100f;//血量上限
    [SerializeField] private float baseArmor = 0f;      //护甲值
    [SerializeField] private float baseHealthRegen = 0.5f;//生命恢复
    [SerializeField] private float baseDodgeChance = 0f;//闪避率
    [SerializeField] private float basecollisionDamage = 10f;//碰撞伤害

    [Header("当前属性")]
    [Header("血量上限")] public float maxHealth;
    [Header("当前血量")] public float currentHealth;
    [Header("护甲值")]   public float armor;
    [Header("生命恢复")] public float healthRegen;
    [Header("闪避率")]   public float dodgeChance;
    [Header("碰撞伤害")]   public float collisionDamage;
    [Header("碰撞受击无敌时间")] public float collisionImmunityDuration = 1.5f;

    [Header("无敌效果")]
    public bool isInvincible; // 当前是否无敌
    public float invincibleDuration; // 无敌剩余时间

    PlayerAbilities playerAbilities;

    //碰撞
    private float lastCollisionDamageTime;
    private bool isCollisionImmune => Time.time - lastCollisionDamageTime < collisionImmunityDuration;

    private void Start()
    {
        ResetToBaseStats();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        RegenerateHealth();
    }

    /// <summary>
    /// 玩家血量系统初始化
    /// </summary>
    public void ResetToBaseStats()
    {
        maxHealth = baseMaxHealth;
        armor = baseArmor;
        healthRegen = baseHealthRegen;
        dodgeChance = baseDodgeChance;
        collisionDamage = basecollisionDamage;
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

        // 致命伤害检查
        if (damageTaken >= currentHealth)
        {
            playerAbilities.TryApplyCheatDeath();
            Debug.Log("Cheat death activated!");
            // 这里可以触发无敌效果
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

    #region 增益效果相关方法

    public void AddMaxHealth(float amount) => maxHealth += amount;
    public void AddArmor(float amount) => armor += amount;
    public void AddHealthRegen(float amount) => healthRegen += amount;
    public void AddCollitionDamage(float amount) => collisionDamage += amount;
    public void AddDodgeChance(float amount) => dodgeChance = Mathf.Min(dodgeChance + amount, 0.6f);
    public void SetCheatDeath(bool value) => playerAbilities.hasCheatDeath = value;

    #endregion

}