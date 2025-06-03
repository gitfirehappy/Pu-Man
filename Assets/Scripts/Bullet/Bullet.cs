using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 移除所有基础属性，只保留必要的引用
    private float _damage;
    private float _knockbackForce;
    private bool _isAoeDamage;
    private float _aoeRadius;
    private Vector2 _direction;
    private float _speed;

    public void Initialize(BulletConfig config, Vector2 direction)
    {
        _damage = config.damage;
        _knockbackForce = config.knockback;
        _isAoeDamage = config.isAoeDamage;
        _aoeRadius = config.aoeRadius;
        _speed = config.speed;
        _direction = direction.normalized;

        transform.localScale = Vector3.one * config.size;
        Invoke(nameof(ReturnToPool), config.lifeTime);
    }

    private void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            HandleEnemyHit(other);
            ReturnToPool();
        }
        else if (other.CompareTag("Obstacle"))
        {
            ReturnToPool();
        }
    }

    private void HandleEnemyHit(Collider2D enemyCollider)
    {
        // 单个目标伤害
        if (enemyCollider.TryGetComponent<IDamageable>(out var enemy))
        {
            enemy.TakeDamage(_damage);

            // 击退效果
            if (_knockbackForce > 0 && enemyCollider.TryGetComponent<Rigidbody2D>(out var rb))
            {
                Vector2 knockbackDirection = (enemyCollider.transform.position - transform.position).normalized;
                rb.AddForce(knockbackDirection * _knockbackForce, ForceMode2D.Impulse);
            }
        }

        // AOE伤害
        if (_isAoeDamage)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, _aoeRadius, LayerMask.GetMask("Enemy"));
            foreach (var hit in hitEnemies)
            {
                if (hit != enemyCollider && hit.TryGetComponent<IDamageable>(out var aoeEnemy))
                {
                    aoeEnemy.TakeDamage(_damage * 0.5f); // AOE伤害减半
                }
            }
        }
    }

    private void ReturnToPool()
    {
        CancelInvoke(nameof(ReturnToPool));
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (_isAoeDamage)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _aoeRadius);
        }
    }
}