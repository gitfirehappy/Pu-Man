using UnityEngine;
using UnityEngine.Pool;

public abstract class BaseEnemy : MonoBehaviour, IDamageable, IPoolable
{
    [Header("Basic Properties")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public float collisionDamage = 10f;
    public float collisionImmunityDuration = 1f;
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

    protected virtual void MoveTowardsPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

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

    // 保留原有的OnTriggerEnter2D以防其他碰撞需求
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // 其他碰撞逻辑（如子弹）可以放在这里
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