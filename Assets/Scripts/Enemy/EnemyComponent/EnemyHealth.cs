using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [SerializeField][Header("血量上限")] private float maxHealth;
    [SerializeField][Header("当前血量")] private float currentHealth;
    [SerializeField][Header("碰撞伤害")] private float collisionDamage;
    [SerializeField][Header("受击无敌")] private float collisionImmunityDuration;
    [SerializeField][Header("碰撞范围")] private float attackRadius;
    [SerializeField][Header("检测间隔")] private float detectionInterval;

    private bool _isDead = false;
    private Collider2D _collider;

    private float lastCollisionDamageTime;
    private float lastDetectionTime;
    private Transform playerTransform;
    private EnemyCore core;

    private bool isCollisionImmune => Time.time - lastCollisionDamageTime < collisionImmunityDuration;

    /// <summary>
    /// 敌人血量系统初始化
    /// </summary>
    /// <param name="data"></param>
    public void Initialize(EnemySO data,EnemyCore enemyCore)
    {
        rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
        collisionDamage = data.collisionDamage;
        collisionImmunityDuration = data.collisionImmunityDuration;
        attackRadius = data.attackRadius;
        detectionInterval = data.detectionInterval;

        lastCollisionDamageTime = -collisionImmunityDuration;
        FindPlayer();
    }

    private void FindPlayer()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
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
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage! Current HP: {currentHealth}");
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

    /// <summary>
    /// 敌人死亡
    /// </summary>
    private void Die()
    {
        if (_isDead || this == null) return;
        _isDead = true;

        // 立即禁用所有相关组件
        if (_collider != null) _collider.enabled = false;
        if (rb != null) rb.simulated = false;

        // 获取核心引用（防止缓存失效）
        var validCore = GetComponent<EnemyCore>();
        if (validCore == null)
        {
            Debug.LogWarning("EnemyCore missing on death", gameObject);
            Destroy(gameObject);
            return;
        }

        Debug.Log("敌人死亡", this);

        // 使用协程确保执行顺序
        StartCoroutine(DeathRoutine(validCore));
    }

    private IEnumerator DeathRoutine(EnemyCore validCore)
    {
        // 第一步：触发事件
        EnemyEvent.TriggerDied(validCore);
        EnemyEvent.TriggerReturnedToPool(validCore, true);

        // 等待一帧让事件处理完成
        yield return null;

        // 第二步：执行回收
        if (validCore != null && !validCore.Equals(null))
        {
            validCore.ReturnToPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 重置血量
    /// </summary>
    public void ResetHealth()
    {
        _isDead = false;
        if (_collider != null) _collider.enabled = true;
        if (rb != null) rb.simulated = true;

        currentHealth = maxHealth;
        lastCollisionDamageTime = -collisionImmunityDuration;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    public bool IsDead => _isDead;
}