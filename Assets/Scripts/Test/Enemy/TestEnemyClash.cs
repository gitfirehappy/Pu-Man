using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyClash : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public TestEnemyMovement movement;
    public Transform playerTransform;

    [Header("基本参数")]
    public float seekRadius;
    public float clashSpeed;
    public float clashCooldown;
    public float clashEndDistance = 0.2f;

    [Header("冲撞状态")]
    public bool isClashing;
    public Vector2 clashTarget;
    public float lastClashTime;

    /// <summary>
    /// 冲撞系统初始化
    /// </summary>
    private void Awake()
    {
        movement = GetComponent<TestEnemyMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

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

        Debug.Log($"Clash {(isClashing ? "Start" : "End")} at {Time.time}");
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

        Debug.Log($"Clash {(isClashing ? "Start" : "End")} at {Time.time}");
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
