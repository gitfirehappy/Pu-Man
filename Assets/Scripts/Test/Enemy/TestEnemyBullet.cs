using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectPoolManager;

public class TestEnemyBullet : MonoBehaviour
{
    private float _damage;
    private Vector2 _direction;
    private float _speed;
    private float _detectionRadius;
    private float _lastDetectionTime;
    private float _detectionInterval = 0.1f;

    public void Initialize(EnemyBulletConfig config, Vector2 direction)
    {
        _damage = config.damage;
        _speed = config.speed;
        _detectionRadius = config.size;
        _direction = direction.normalized;

        transform.localScale = Vector3.one * config.size;
        Invoke(nameof(ReturnToPool), config.lifeTime);
    }

    private void Update()
    {
        if (PauseManager.Instance.IsPaused) return;

        transform.Translate(_direction * _speed * Time.deltaTime);

        if (Time.time - _lastDetectionTime >= _detectionInterval)
        {
            DetectAndDamage();
            _lastDetectionTime = Time.time;
        }
    }

    private void DetectAndDamage()
    {
        // 检测玩家
        Collider2D player = Physics2D.OverlapCircle(
            transform.position,
            _detectionRadius,
            LayerMask.GetMask("Player"));

        if (player != null)
        {
            ApplyDamage(player, _damage);
            ReturnToPool();
        }

        // 检测障碍物
        if (Physics2D.OverlapCircle(
            transform.position,
            _detectionRadius,
            LayerMask.GetMask("Wall")) != null)
        {
            ReturnToPool();
        }
    }

    private void ApplyDamage(Collider2D playerCollider, float damageAmount)
    {
        if (playerCollider.TryGetComponent<TestPlayerHealth>(out var player))
        {
            player.TakeDamage(damageAmount, DamageSource.Enemy);
        }
    }

    private void ReturnToPool()
    {
        CancelInvoke(nameof(ReturnToPool));
        ObjectPoolManager.ReturnObjectToPool(gameObject, PoolType.EnemyBullet);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
