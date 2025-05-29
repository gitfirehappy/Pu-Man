using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("基础属性")]
    public float damage = 10f;
    public float speed = 10f;
    public float lifeTime = 3f;
    public float size = 1f;

    protected Vector2 direction;
    protected bool isInitialized;

    private void OnEnable()
    {
        isInitialized = false;
        Invoke(nameof(ReturnToPool), lifeTime); // 3秒后自动回收
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 只对敌人造成伤害（避免打到自己或玩家时消失）
        if (other.CompareTag("Enemy")) // 确保敌人标签是 "Enemy"
        {
            if (other.TryGetComponent<IDamageable>(out var enemy))
            {
                enemy.TakeDamage(damage);
                ReturnToPool(); // 打中敌人才回收
            }
        }
    }

    private void ReturnToPool()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}