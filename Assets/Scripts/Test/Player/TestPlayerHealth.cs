using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerHealth : MonoBehaviour
{
    [Header("当前状态")]
    [Header("当前血量")] public float currentHealth;
    [Header("当前能否触发名刀")] public bool currentHasCheatDeath;

    [Header("基础属性")]
    [Header("血量上限")] public float maxHealth;
    [Header("护甲值")] public float armor;
    [Header("生命恢复")] public float healthRegen;
    [Header("闪避率")] public float dodgeChance;
    [Header("碰撞伤害")] public float collisionDamage;
    [Header("碰撞受击无敌时间")] public float collisionImmunityDuration;
    [Header("是否有名刀")] public bool hasCheatDeath;
    [Header("名刀无敌时间")] public float cheatDeathInvincibleTime;
    [Header("无敌效果")]
    public bool isInvincible; // 当前是否无敌
    public float invincibleDuration; // 无敌剩余时间

    [Header("碰撞检测")]
    public float lastCollisionDamageTime;

    private bool isCollisionImmune => Time.time - lastCollisionDamageTime < collisionImmunityDuration;

    private SpriteRenderer spriteRenderer;//图片

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        RegenerateHealth();
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
    public void TakeDamage(float damage, DamageSource source = DamageSource.Enemy)
    {
        //无敌判定
        if (isInvincible) return;

        // 闪避判定
        if (UnityEngine.Random.value < dodgeChance)
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
        Debug.Log($"{source} caused {damageTaken} damage to Player! Current HP: {currentHealth}");

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

    /// <summary>
    /// 玩家死亡
    /// </summary>
    private void Die(DamageSource source)
    {
        Debug.Log("Player died!");
        gameObject.SetActive(false);
    }

}
