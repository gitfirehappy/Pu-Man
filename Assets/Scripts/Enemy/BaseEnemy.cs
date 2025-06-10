using UnityEngine;
using UnityEngine.Pool;

public abstract class BaseEnemy : MonoBehaviour, IDamageable, IPoolable
{
    //需要修改架构
    //敌人内部只处理逻辑，数据由SO读取

    [Header("基础属性")]
    [Header("血量上限")] public float maxHealth = 100f;
    [Header("移动速度")] public float moveSpeed = 3f;
    [Header("碰撞伤害")] public float collisionDamage = 10f;
    [Header("受击无敌")] public float collisionImmunityDuration = 1f;
    [Tooltip("敌人攻击检测半径")]
    public float attackRadius = 1.5f;
    [Tooltip("检测间隔（秒）")]
    public float detectionInterval = 0.2f;

    protected float currentHealth;
    protected Transform playerTransform;
    protected Rigidbody2D rb;
    protected IObjectPool<GameObject> managedPool;

    private float lastCollisionDamageTime;
    private float lastDetectionTime;
    protected bool isCollisionImmune => Time.time - lastCollisionDamageTime < collisionImmunityDuration;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        InitializeEnemy();
        FindPlayer();
    }

    protected virtual void InitializeEnemy()
    {
        currentHealth = maxHealth;
        lastCollisionDamageTime = -collisionImmunityDuration;
        lastDetectionTime = Time.time;
    }

    protected virtual void FindPlayer()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
        {
            Debug.LogWarning($"[{GetType().Name}] Player not found!");
        }
    }

    protected virtual void Update()
    {
        if (playerTransform != null)
        {
            MoveTowardsPlayer();

            // 间隔检测，优化性能
            if (Time.time - lastDetectionTime >= detectionInterval)
            {
                DetectAndDamagePlayer();
                lastDetectionTime = Time.time;
            }
        }
    }

    /// <summary>
    /// 追击玩家
    /// </summary>
    protected virtual void MoveTowardsPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    /// <summary>
    /// 碰撞造成伤害
    /// </summary>
    protected virtual void DetectAndDamagePlayer()
    {
        // 使用OverlapCircle检测范围内的玩家
        Collider2D playerCollider = Physics2D.OverlapCircle(
            transform.position,
            attackRadius,
            LayerMask.GetMask("Player"));

        if (playerCollider != null && playerCollider.CompareTag("Player"))
        {
            if (playerCollider.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                // 对玩家造成伤害（带无敌帧检查）
                playerHealth.TryTakeCollisionDamage(collisionDamage);

                // 敌人受到玩家的碰撞伤害（带无敌帧）
                TryTakeCollisionDamage(playerHealth.GetCollisionDamage());
            }
        }
    }

    /// <summary>
    /// 造成伤害(核心)
    /// </summary>
    /// <param name="damage"></param>
    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage! Current HP: {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual bool TryTakeCollisionDamage(float damage)
    {
        if (isCollisionImmune) return false;

        TakeDamage(damage);
        lastCollisionDamageTime = Time.time;
        return true;
    }


    protected virtual void Die()
    {
        if (managedPool != null)
        {
            managedPool.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 绘制检测范围Gizmo（仅在编辑器可见）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    // IPoolable接口实现
    public virtual void OnRelease() { /* 实现略 */ }
    public virtual void OnGet() { /* 实现略 */ }
    public virtual void SetPool(IObjectPool<GameObject> pool) { /* 实现略 */ }
}