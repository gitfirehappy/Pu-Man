using UnityEngine;

/// <summary>
/// 基础敌人类，实现最简单的追踪和伤害逻辑
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BasicEnemy : BaseEnemy
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Die()
    {
        base.Die();
    }
} 