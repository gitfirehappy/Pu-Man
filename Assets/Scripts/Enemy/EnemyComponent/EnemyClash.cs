using UnityEngine;

public class EnemyClash : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private EnemyMovement movement;

    [Tooltip("检测范围")][SerializeField] private float seekRadius;
    [Tooltip("冲刺速度")][SerializeField] private float clashSpeed;
    [Tooltip("冲刺冷却")][SerializeField] private float clashCooldown;
    [Tooltip("冲撞结束距离阈值")][SerializeField] private float clashEndDistance = 0.2f;

    private float lastClashTime;
    private bool isClashing;

    private bool originalFlipX;
    private Vector2 clashTarget;
    private Transform playerTransform;

    public bool IsClashing => isClashing; // 冲撞状态

    private void Update()
    {
        if (PauseManager.Instance.IsPaused) return;

        if (playerTransform == null || isClashing)
            return;

        // 冷却检查
        if (Time.time - lastClashTime < clashCooldown)
            return;

        // 距离检查
        if (Vector2.Distance(transform.position, playerTransform.position) <= seekRadius)
        {
            StartClash();
        }
    }

    private void FixedUpdate()
    {
        if (PauseManager.Instance.IsPaused)
        {
            if (rb != null) rb.velocity = Vector2.zero;
            return;
        }

        if (!isClashing) return;

        // 计算到目标点的距离
        float distance = Vector2.Distance(transform.position, clashTarget);

        // 在目标点附近结束冲撞
        if (distance < clashEndDistance)
        {
            rb.velocity = Vector2.zero;
            EndClash();
            return;
        }

        // 继续向目标点移动
        Vector2 moveDirection = (clashTarget - (Vector2)transform.position).normalized;
        rb.velocity = moveDirection * clashSpeed;
    }

    #region EnemyCore相关
    /// <summary>
    /// 冲撞系统初始化
    /// </summary>
    public void Initialize(EnemySO data)
    {
        if (data.clashConfig == null)
        {
            Debug.LogError("Missing clash config for enemy!");
            return;
        }

        seekRadius = data.clashConfig.seekRadius;
        clashSpeed = data.clashConfig.clashSpeed;
        clashCooldown = data.clashConfig.clashCooldown;

        playerTransform = PlayerManager.Instance.Player.transform;
        movement = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 重置冲撞系统状态
    /// </summary>
    public void ResetToBaseStats()
    {
        isClashing = false;
        lastClashTime = 0f;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        if (movement != null)
        {
            movement.enabled = true;
        }
    }
    #endregion

    /// <summary>
    /// 开始冲撞
    /// </summary>
    private void StartClash()
    {
        isClashing = true;
        clashTarget = playerTransform.position; // 锁定冲撞时的玩家位置
        lastClashTime = Time.time;

        if (spriteRenderer != null)
        {
            originalFlipX = spriteRenderer.flipX; // 保存原始状态
            // 玩家在左侧则翻转（素材默认朝右）
            spriteRenderer.flipX = clashTarget.x < transform.position.x;
        }

        // 禁用普通移动
        if (movement != null)
            movement.enabled = false;
    }

    /// <summary>
    /// 结束冲撞
    /// </summary>
    private void EndClash()
    {
        isClashing = false;
        rb.velocity = Vector2.zero;

        spriteRenderer.flipX = originalFlipX;

        // 重新启用普通移动
        if (movement != null)
            movement.enabled = true;
    }

    private void OnDrawGizmos()
    {
        if (seekRadius > 0)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, seekRadius);
        }

        if (isClashing)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, clashTarget);
        }
    }
}