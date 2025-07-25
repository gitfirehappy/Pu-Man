using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyMovement : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public EnemyClash enemyClash;
    public Transform playerTransform;

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
