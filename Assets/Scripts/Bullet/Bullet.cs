using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _damage;
    private float _knockbackForce;
    private bool _isAoeDamage;
    private Vector2 _direction;
    private float _speed;
    private float _detectionRadius; // 统一使用一个检测半径
    private float _lastDetectionTime;
    private float _detectionInterval = 0.1f;

    public void Initialize(BulletConfig config, Vector2 direction)
    {
        _damage = config.damage;
        _knockbackForce = config.knockback;
        _isAoeDamage = config.isAoeDamage;
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
        // 检测范围内的敌人
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            transform.position,
            _detectionRadius,
            LayerMask.GetMask("Enemy"));

        if (hitEnemies.Length > 0)
        {
            if (_isAoeDamage)
            {
                // AOE伤害：对所有检测到的敌人造成伤害
                foreach (var enemyCollider in hitEnemies)
                {
                    ApplyDamage(enemyCollider, _damage);
                }
            }
            else
            {
                // 单体伤害：只对第一个检测到的敌人造成伤害
                ApplyDamage(hitEnemies[0], _damage);
            }

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

    private void ApplyDamage(Collider2D enemyCollider, float damageAmount)
    {
        // 增加组件有效性检查
        if (enemyCollider == null || !enemyCollider.gameObject.activeInHierarchy)
            return;

        if (enemyCollider.TryGetComponent<IDamageable>(out var enemy) &&
            enemyCollider.TryGetComponent<EnemyHealth>(out var health) &&
            !health.IsDead)
        {
            enemy.TakeDamage(damageAmount, DamageSource.Player);

            // 击退效果
            if (_knockbackForce > 0 && enemyCollider.TryGetComponent<Rigidbody2D>(out var rb))
            {
                Vector2 knockbackDirection = (enemyCollider.transform.position - transform.position).normalized;
                rb.AddForce(knockbackDirection * _knockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    /// <summary>
    /// 玩家子弹回收
    /// </summary>
    private void ReturnToPool()
    {
        CancelInvoke(nameof(ReturnToPool));

        // 添加null检查
        if (this == null || gameObject == null) return;

        // 确保ObjectPoolManager实例存在
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject, ObjectPoolManager.PoolType.PlayerBullet);
        }
        else
        {
            Debug.LogWarning("ObjectPoolManager is not available. Skipping return to pool.");
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _isAoeDamage ? Color.yellow : Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}

/// <summary>
/// 子弹属性
/// </summary>
[System.Serializable]
public struct BulletConfig
{
    public float damage;
    public float knockback;
    public bool isAoeDamage;
    public float speed;
    public float size;
    public float lifeTime;
}