using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("基础属性")]
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float baseFireRate = 0.2f;
    [SerializeField] private float baseKnockback = 0f;
    [SerializeField] private int baseProjectileCount = 1;
    [SerializeField] private float baseProjectileSize = 1f;
    [SerializeField] private float baseProjectileLifeTime = 3f;
    [SerializeField] private float baseProjectileSpeed = 20f;

    [Header("当前属性")]
    [Header("子弹伤害")] public float damage;
    [Header("射速")]     public float fireRate;
    [Header("击退距离")] public float knockback;
    [Header("弹道数量")] public int projectileCount;
    [Header("子弹大小")] public float projectileSize;
    [Header("范围伤害")] public bool isAoeDamage;
    [Header("AOE范围")] public float aoeRadius = 1.5f;
    [Header("飞行速度")] public float projectileSpeed;

    [Header("设置")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    private float nextFireTime;
    private bool isShooting;

    private void Start()
    {
        ResetToBaseStats();
    }

    public void ResetToBaseStats()
    {
        damage = baseDamage;
        fireRate = baseFireRate;
        knockback = baseKnockback;
        projectileCount = baseProjectileCount;
        projectileSize = baseProjectileSize;
        projectileSpeed = baseProjectileSpeed;
        isAoeDamage = false;
    }

    public void StartShooting()
    {
        isShooting = true;
    }

    public void StopShooting()
    {
        isShooting = false;
    }

    private void Update()
    {
        if (isShooting && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void Shoot()
    {
        float angleStep = projectileCount > 1 ? 15f / (projectileCount - 1) : 0f;
        float startAngle = -(angleStep * (projectileCount - 1)) / 2f;

        var config = new BulletConfig
        {
            damage = damage,
            knockback = knockback,
            isAoeDamage = isAoeDamage,
            aoeRadius = aoeRadius,
            size = projectileSize,
            lifeTime = baseProjectileLifeTime,
            speed = projectileSpeed,
        };

        for (int i = 0; i < projectileCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Vector2 direction = Quaternion.Euler(0, 0, currentAngle) * transform.right;

            GameObject bulletObj = ObjectPoolManager.SpawnObject(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bullet.Initialize(config, direction);
        }
    }

    // 增益效果相关方法
    public void AddDamage(float amount) => damage += amount;
    public void AddFireRate(float amount) => fireRate += amount;
    public void AddKnockback(float amount) => knockback += amount;
    public void AddProjectileCount(int amount) => projectileCount = Mathf.Min(projectileCount + amount, 5);
    public void AddProjectileSize(float amount) => projectileSize += amount;
    public void SetAoeDamage(bool value) => isAoeDamage = value;
    public void SetAoeRadius(float radius) => aoeRadius = radius;
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
    public float aoeRadius;
    public float speed;
    public float size;
    public float lifeTime;
}