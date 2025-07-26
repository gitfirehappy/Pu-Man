using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyShooting : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float bulletDamage;
    public float bulletSpeed;
    public float bulletSize;
    public float shootRate;
    public float shootRadius;
    public int projectileCount;
    public float bulletLifeTime;

    public float nextShootTime;
    public Transform playerTransform;

    /// <summary>
    /// 敌人射击系统初始化
    /// </summary>
    private void Awake()
    {
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

            TestEnemyBullet bullet = bulletObj.GetComponent<TestEnemyBullet>();
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
