using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private EnemyClash enemyClash;

    [SerializeField][Header("移动速度")] private float moveSpeed;

    private Transform playerTransform;
    private float knockbackDuration = 0.2f;
    private float knockbackEndTime;
    private Vector2 knockbackDirection;
    private float knockbackForce;

    /// <summary>
    /// 敌人移动系统初始化
    /// </summary>
    /// <param name="data"></param>
    public void Initialize(EnemySO data)
    {
        rb = GetComponent<Rigidbody2D>();

        moveSpeed = data.moveSpeed;
        playerTransform = PlayerManager.Instance.Player.transform;
    }

    /// <summary>
    /// 重置移动系统状态
    /// </summary>
    public void ResetToBaseStats()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void Update()
    {
        if (PauseManager.Instance.IsPaused) return;

        if (playerTransform == null || (enemyClash != null && enemyClash.IsClashing))
            return;

        // 处理击退效果
        if (Time.time < knockbackEndTime)
        {
            rb.velocity = knockbackDirection * knockbackForce;
            return;
        }

        MoveTowardsPlayer();
    }

    /// <summary>
    /// 向玩家移动
    /// </summary>
    private void MoveTowardsPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    /// <summary>
    /// 应用击退效果
    /// </summary>
    public void ApplyKnockback(Vector2 direction, float force)
    {
        // 1. 先清零速度
        rb.velocity = Vector2.zero;

        // 2. 记录击退参数
        knockbackDirection = direction;
        knockbackForce = force;

        // 3. 设置击退持续时间
        knockbackEndTime = Time.time + knockbackDuration;
    }
}