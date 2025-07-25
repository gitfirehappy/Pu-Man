using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerShooting : MonoBehaviour
{
    [Header("当前属性")]
    [Header("子弹伤害")] public float currentDamage;
    [Header("射速")] public float currentFireRate;
    [Header("击退距离")] public float currentKnockback;
    [Header("弹道数量")] public int currentProjectileCount;
    [Header("子弹大小")] public float currentProjectileSize;
    [Header("飞行速度")] public float currentProjectileSpeed;
    [Header("生命周期")] public float currentProjectileLifeTime;
    [Header("范围伤害")] public bool currentIsAoeDamage;

    [Header("设置")]
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] public Transform firePoint;

    [SerializeField][Header("是否跟随鼠标")] public bool isRotating;

    private InputAction fireAction;
    private PlayerInput playerInput;
    private float nextFireTime;
    public bool IsShooting { get; private set; }//射击状态

    private void Awake()
    {
        playerInput = new PlayerInput();
        fireAction = playerInput.Player.Fire;

        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    private void OnEnable() => playerInput?.Enable();
    private void OnDisable() => playerInput?.Disable();

    private void Update()
    {
        if (isRotating)
        {
            RotateTowardsMouse();
        }

        if (fireAction.IsPressed() && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / currentFireRate;
            IsShooting = true;
        }
        else
        {
            IsShooting = false;
        }
    }

    /// <summary>
    /// 射击
    /// </summary>
    public void Shoot()
    {
        // 添加预制体检查
        if (bulletPrefab == null)
        {
            Debug.LogError("PlayerShooting中子弹预制体未设置!");
            return;
        }

        float angleStep = currentProjectileCount > 1 ? 15f / (currentProjectileCount - 1) : 0f;
        float startAngle = -(angleStep * (currentProjectileCount - 1)) / 2f;

        var config = new BulletConfig
        {
            damage = currentDamage,
            knockback = currentKnockback,
            isAoeDamage = currentIsAoeDamage,
            size = currentProjectileSize,
            lifeTime = currentProjectileLifeTime,
            speed = currentProjectileSpeed,
        };

        for (int i = 0; i < currentProjectileCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Vector2 direction = Quaternion.Euler(0, 0, currentAngle) * transform.right;

            GameObject bulletObj = ObjectPoolManager.SpawnObject(bulletPrefab, firePoint.position, Quaternion.identity, ObjectPoolManager.PoolType.PlayerBullet);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bullet.Initialize(config, direction);
        }
    }



    /// <summary>
    /// 跟随鼠标旋转
    /// </summary>
    public void RotateTowardsMouse()
    {
        // 获取鼠标位置（屏幕坐标）
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();

        // 将鼠标位置从屏幕坐标转换为世界坐标
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;  // 确保在 2D 平面

        // 计算方向向量
        Vector3 direction = mouseWorldPos - transform.position;
        direction.z = 0f;
        direction.Normalize();

        // 计算角度并设置旋转（Z 轴朝向）
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
