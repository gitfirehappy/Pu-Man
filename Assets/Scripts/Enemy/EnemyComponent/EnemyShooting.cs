using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    private GameObject bulletPrefab;

    [SerializeField] private float bulletDamage;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletSize;
    [SerializeField] private float shootRate;
    [SerializeField] private float shootRadius;
    [SerializeField] private int projectileCount;
    [SerializeField] private float bulletLifeTime;

    private float nextShootTime;
    private Transform playerTransform;

    /// <summary>
    /// 敌人射击系统初始化
    /// </summary>
    public void Initialize(EnemySO data)
    {
        if (data.shootingConfig == null)
        {
            Debug.LogError("Missing shooting config for enemy!");
            return;
        }

        bulletPrefab = data.shootingConfig.bulletPrefab;
        bulletDamage = data.shootingConfig.bulletDamage;
        bulletSpeed = data.shootingConfig.bulletSpeed;
        bulletSize = data.shootingConfig.bulletSize;
        shootRate = data.shootingConfig.shootRate;
        shootRadius = data.shootingConfig.shootRadius;
        projectileCount = data.shootingConfig.projectileCount;
        bulletLifeTime = data.shootingConfig.bulletLifeTime;

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        firePoint = firePoint != null ? firePoint : transform;
    }

    private void Update()
    {
        if (playerTransform == null || Time.time < nextShootTime)
            return;

        // 检查玩家是否在射击范围内
        if (Vector2.Distance(transform.position, playerTransform.position) <= shootRadius)
        {
            Shoot();
            nextShootTime = Time.time + 1f / shootRate;
        }
    }

    /// <summary>
    /// 向玩家方向射击
    /// </summary>
    private void Shoot()
    {
        if (bulletPrefab == null) return;

        Vector2 directionToPlayer = (playerTransform.position - firePoint.position).normalized;

        var config = new EnemyBulletConfig
        {
            damage = bulletDamage,
            speed = bulletSpeed,
            size = bulletSize,
            lifeTime = bulletLifeTime, //生命周期
        };

        // 多弹道散射逻辑
        float angleStep = projectileCount > 1 ? 15f / (projectileCount - 1) : 0f;
        float startAngle = -(angleStep * (projectileCount - 1)) / 2f;

        for (int i = 0; i < projectileCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Vector2 direction = Quaternion.Euler(0, 0, currentAngle) * directionToPlayer;

            GameObject bulletObj = ObjectPoolManager.SpawnObject(
                bulletPrefab,
                firePoint.position,
                Quaternion.identity,
                ObjectPoolManager.PoolType.EnemyBullet);

            EnemyBullet bullet = bulletObj.GetComponent<EnemyBullet>();
            bullet.Initialize(config, direction);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (shootRadius > 0)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, shootRadius);
        }
    }
}