using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [SerializeField][Header("移动速度")] private float moveSpeed;
    private Transform playerTransform;

    /// <summary>
    /// 敌人移动系统初始化
    /// </summary>
    /// <param name="data"></param>
    public void Initialize(EnemySO data)
    {
        moveSpeed = data.moveSpeed;
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (playerTransform == null) return;
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