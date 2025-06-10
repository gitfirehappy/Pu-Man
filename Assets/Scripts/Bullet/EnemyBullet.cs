using UnityEngine;
using static ObjectPoolManager;

public class EnemyBullet : MonoBehaviour
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
            LayerMask.GetMask("Obstacle")) != null)
        {
            ReturnToPool();
        }
    }

    private void ApplyDamage(Collider2D playerCollider, float damageAmount)
    {
        if (playerCollider.TryGetComponent<PlayerHealth>(out var player))
        {
            player.TakeDamage(damageAmount);
        }
    }

    private void ReturnToPool()
    {
        CancelInvoke(nameof(ReturnToPool));
        ObjectPoolManager.ReturnObjectToPool(gameObject, PoolType.EnemyBullet);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}

[System.Serializable]
public struct EnemyBulletConfig
{
    public float damage;
    public float speed;
    public float size;
    public float lifeTime;
}