using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private EnemyClash enemyClash;
    [SerializeField] private EnemyCore enemyCore;
    [SerializeField] private Transform playerTransform;

    [Header("移动参数")]
    [Tooltip("移动速度")][SerializeField] private float moveSpeed;

    private float knockbackDuration = 0.2f;
    private float knockbackEndTime;
    private Vector2 knockbackDirection;
    private float knockbackForce;

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

        MoveWithBoidBehavior();
    }

    #region EnemCore相关
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
        knockbackEndTime = 0;
    }
    #endregion

    /// <summary>
    /// 结合鸟群行为向玩家移动
    /// </summary>
    private void MoveWithBoidBehavior()
    {
        // 基础目标方向（朝向玩家）
        Vector2 targetDirection = (playerTransform.position - transform.position).normalized;

        // 获取管理器和当前敌人索引
        var enemyManager = EnemyManager.Instance;
        int selfIndex = enemyManager.GetEnemyIndex(enemyCore);
        if (selfIndex == -1)
        {
            rb.velocity = targetDirection * moveSpeed;
            return;
        }

        // 获取全局数据
        float neighborRadius = enemyManager.GetBoidNeighborRadius();
        List<Vector2> allPositions = enemyManager.GetEnemyPositions();
        List<Vector2> allVelocities = enemyManager.GetEnemyVelocities();

        // 查找邻居索引
        List<int> neighborIndices = BoidMath.FindNeighborIndices(
            transform.position,
            allPositions,
            neighborRadius
        );
        neighborIndices.Remove(selfIndex); // 排除自己

        // 提取邻居的位置和速度
        List<Vector2> neighborPositions = new List<Vector2>();
        List<Vector2> neighborVelocities = new List<Vector2>();
        foreach (int idx in neighborIndices)
        {
            neighborPositions.Add(allPositions[idx]);
            neighborVelocities.Add(allVelocities[idx]);
        }

        // 计算行为向量
        Vector2 cohesion = BoidMath.CalculateCohesion(neighborPositions, transform.position);
        Vector2 separation = BoidMath.CalculateSeparation(
            neighborPositions,
            transform.position,
            enemyManager.separationMinDistance // 使用管理器中的参数
        );
        Vector2 alignment = BoidMath.CalculateAlignment(neighborVelocities, rb.velocity);

        // 综合方向（使用管理器中的权重）
        Vector2 finalDirection =
            (cohesion * enemyManager.boidCohesionWeight) +
            (separation * enemyManager.boidSeparationWeight) +
            (alignment * enemyManager.boidAlignmentWeight) +
            (targetDirection * enemyManager.boidTargetWeight);

        // 应用移动
        rb.velocity = finalDirection.normalized * moveSpeed;
    }

    #region 外部调用接口
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
    #endregion
}