using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private EnemyClash enemyClash;

    [SerializeField][Header("移动速度")] private float moveSpeed;
    private Transform playerTransform;

    /// <summary>
    /// 敌人移动系统初始化
    /// </summary>
    /// <param name="data"></param>
    public void Initialize(EnemySO data)
    {
        rb = GetComponent<Rigidbody2D>();

        moveSpeed = data.moveSpeed;
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
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
}