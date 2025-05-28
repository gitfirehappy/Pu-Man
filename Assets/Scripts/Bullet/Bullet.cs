using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("基础属性")]
    public float damage = 10f;
    public float speed = 10f;
    public float lifeTime = 3f;
    public float size = 1f;

    [Header("碰撞检测")]
    public LayerMask targetLayers;
    public float hitRadius = 0.5f;

    protected Vector2 direction;
    protected bool isInitialized;

    private void OnEnable()
    {
        isInitialized = false;
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    public virtual void Initialize(Vector2 dir, float damage, float speed, float size)
    {
        if (isInitialized) return;
        
        this.damage = damage;
        this.speed = speed;
        this.direction = dir.normalized;
        transform.localScale = Vector3.one * size;
        isInitialized = true;
    }

    protected virtual void Update()
    {
        if (!isInitialized) return;
        transform.Translate(direction * speed * Time.deltaTime);
        
        // 碰撞检测
        Collider2D hit = Physics2D.OverlapCircle(transform.position, hitRadius, targetLayers);
        if (hit != null)
        {
            OnHit(hit);
        }
    }

    protected virtual void OnHit(Collider2D other)
    {
        // 处理伤害
        IDamageable damageable = other.GetComponent<IDamageable>();
        damageable?.TakeDamage(damage);
        
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
} 