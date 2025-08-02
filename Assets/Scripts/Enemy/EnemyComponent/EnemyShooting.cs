using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [Tooltip("开火点")][SerializeField] private Transform firePoint;
    [Tooltip("子弹预制体")][SerializeField] private GameObject bulletPrefab;
    [Tooltip("玩家坐标")][SerializeField] private Transform playerTransform;

    [Header("射击参数")]
    [Tooltip("子弹伤害")][SerializeField] private float bulletDamage;
    [Tooltip("飞行速度")][SerializeField] private float bulletSpeed;
    [Tooltip("大小")][SerializeField] private float bulletSize;
    [Tooltip("射速")][SerializeField] private float shootRate;
    [Tooltip("范围")][SerializeField] private float shootRadius;
    [Tooltip("弹道")][SerializeField] private int projectileCount;
    [Tooltip("生命周期")][SerializeField] private float bulletLifeTime;

    private float nextShootTime;

    private void Update()
    {
        if (PauseManager.Instance.IsPaused) return;

        if (playerTransform == null || Time.time < nextShootTime)
            return;

        // 检查玩家是否在射击范围内
        if (Vector2.Distance(transform.position, playerTransform.position) <= shootRadius)
        {
            Shoot();
            nextShootTime = Time.time + 1f / shootRate;
        }
    }

    #region EneCore相关
    /// <summary>
    /// 敌人射击系统初始化
    /// </summary>
    public void Initialize(EnemySO data, EnemyBonusStats bonusStats)
    {
        if (data.shootingConfig == null)
        {
            Debug.LogError("Missing shooting config for enemy!");
            return;
        }

        bulletPrefab = data.shootingConfig.bulletPrefab;

        // 获取基础值+增长值
        bulletDamage = data.shootingConfig.bulletDamage + bonusStats.bulletDamageBonus;

        bulletSpeed = data.shootingConfig.bulletSpeed;
        bulletSize = data.shootingConfig.bulletSize;
        shootRate = data.shootingConfig.shootRate;
        shootRadius = data.shootingConfig.shootRadius;

        projectileCount = data.shootingConfig.projectileCount;
        if (GetComponent<EnemyCore>().EnemyData.isBoss)
        {
            projectileCount += bonusStats.projectileCountBonus;
        }

        bulletLifeTime = data.shootingConfig.bulletLifeTime;

        playerTransform = PlayerManager.Instance.Player.transform;
        firePoint = firePoint != null ? firePoint : transform;
    }

    /// <summary>
    /// 重置射击系统状态
    /// </summary>
    public void ResetToBaseStats()
    {
        nextShootTime = 0f;
    }
    #endregion

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

    private void OnDrawGizmos()
    {
        if (shootRadius > 0)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, shootRadius);
        }
    }
}