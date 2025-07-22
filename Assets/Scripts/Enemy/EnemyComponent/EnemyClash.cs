using UnityEngine;

public class EnemyClash : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private EnemyMovement movement;

    private float seekRadius;
    private float clashSpeed;
    private float clashCooldown;

    private float lastClashTime;
    private bool isClashing;
    private Vector2 clashTarget;
    private Transform playerTransform;

    public bool IsClashing => isClashing; // 暴露冲撞状态

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

        // 冲撞移动（直线运动）
        Vector2 direction = (clashTarget - (Vector2)transform.position).normalized;
        rb.velocity = direction * clashSpeed;

        // 检查是否到达目标点
        if (Vector2.Distance(transform.position, clashTarget) < 0.1f)
        {
            EndClash();
        }
    }

    /// <summary>
    /// 开始冲撞
    /// </summary>
    private void StartClash()
    {
        isClashing = true;
        clashTarget = playerTransform.position; // 锁定冲撞时的玩家位置
        lastClashTime = Time.time;

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

        // 重新启用普通移动
        if (movement != null)
            movement.enabled = true;
    }

    private void OnDrawGizmosSelected()
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