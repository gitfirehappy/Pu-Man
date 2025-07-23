using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    private EnemyCore enemyCore;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [SerializeField][Header("血量上限")] private float maxHealth;
    [SerializeField][Header("当前血量")] private float currentHealth;
    [SerializeField][Header("碰撞伤害")] private float collisionDamage;
    [SerializeField][Header("受击无敌")] private float collisionImmunityDuration;
    [SerializeField][Header("碰撞范围")] private float attackRadius;
    [SerializeField][Header("检测间隔")] private float detectionInterval;

    [SerializeField] private bool _isDead = false;
    private Collider2D _collider;

    private float lastCollisionDamageTime;
    private float lastDetectionTime;
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;

    public event Action OnDeath;

    private bool isCollisionImmune => Time.time - lastCollisionDamageTime < collisionImmunityDuration;

    /// <summary>
    /// 敌人血量系统初始化
    /// </summary>
    /// <param name="data"></param>
    public void Initialize(EnemySO data, EnemyCore enemyCore, EnemyBonusStats bonusStats)
    {
        rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // 获取基础值+增长值
        maxHealth = data.maxHealth + bonusStats.maxHealthBonus;
        collisionDamage = data.collisionDamage + bonusStats.collisionDamageBonus;

        collisionImmunityDuration = data.collisionImmunityDuration;
        attackRadius = data.attackRadius;
        detectionInterval = data.detectionInterval;

        ResetToBaseStats();
        FindPlayer();
    }

    /// <summary>
    /// 刷新血量系统状态
    /// </summary>
    public void ResetToBaseStats()
    {
        _isDead = false;
        if (_collider != null) _collider.enabled = true;
        if (rb != null) rb.simulated = true;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        currentHealth = maxHealth;
        lastCollisionDamageTime = -collisionImmunityDuration;
    }


    private void FindPlayer()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (PauseManager.Instance.IsPaused) return;

        if (playerTransform == null || Time.time - lastDetectionTime < detectionInterval)
            return;

        DetectAndDamagePlayer();
        lastDetectionTime = Time.time;
    }

    private void DetectAndDamagePlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(
            transform.position,
            attackRadius,
            LayerMask.GetMask("Player"));

        if (playerCollider != null &&
            playerCollider.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            // 对玩家造成伤害
            playerHealth.TryTakeCollisionDamage(collisionDamage);

            // 敌人受到反伤（带无敌帧检查）
            TryTakeCollisionDamage(playerHealth.GetCollisionDamage());
        }
    }

    /// <summary>
    /// 敌人受伤
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage, DamageSource source)
    {
        if (PauseManager.Instance.IsPaused) return;

        if (_isDead) return;

        currentHealth -= damage;
        Debug.Log($"{source} caused {damage} damage to Enemy! Current HP: {currentHealth}");

        StartCoroutine(HitAnimationRoutine());

        if (currentHealth <= 0)
        {
            Die(source);
        }
    }

    private IEnumerator HitAnimationRoutine()
    {
        if (spriteRenderer == null) yield break;

        float duration = 0.3f; // 比Player更短
        float blinkSpeed = 0.07f; // 更快的闪烁频率
        float timer = 0;

        while (timer < duration)
        {
            // 使用较暗的半透明效果（alpha=0.5）
            spriteRenderer.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
            yield return new WaitForSeconds(blinkSpeed);

            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(blinkSpeed);

            timer += blinkSpeed * 2;
        }

        // 确保最终状态正常
        spriteRenderer.color = Color.white;
    }

    public bool TryTakeCollisionDamage(float damage)
    {
        if (isCollisionImmune) return false;

        TakeDamage(damage,DamageSource.Player);
        lastCollisionDamageTime = Time.time;
        return true;
    }

    /// <summary>
    /// 敌人死亡
    /// </summary>
    private void Die(DamageSource source)
    {
        if (_isDead || this == null) return;
        _isDead = true;

        // 立即禁用所有相关组件
        if (_collider != null) _collider.enabled = false;
        if (rb != null) rb.simulated = false;

        Debug.Log("敌人死亡", this);

        // 通知死亡事件（EnemyCore会处理回收）
        OnDeath?.Invoke();
        EnemyEvent.TriggerDeath(enemyCore, source); // 传递死亡来源
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    #region 公共属性
    public bool IsDead => _isDead;

    public float CurrentHealth => currentHealth;

    public float MaxHealth => maxHealth;

    #endregion
}