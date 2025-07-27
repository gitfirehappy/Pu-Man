using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyMovement : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public TestEnemyClash enemyClash;
    public Transform playerTransform;
    public float knockbackDuration = 0.2f;
    public float knockbackEndTime;
    private Vector2 knockbackDirection;
    private float knockbackForce;

    [Header("移动速度")] public float moveSpeed;

    /// <summary>
    /// 敌人移动系统初始化
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (playerTransform == null || (enemyClash != null && enemyClash.isClashing))
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

        knockbackEndTime = Time.time + knockbackDuration;
    }

}
