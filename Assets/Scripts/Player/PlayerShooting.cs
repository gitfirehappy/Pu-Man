using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    private float baseDamage;
    private float baseFireRate;
    private float baseKnockback;
    private int baseProjectileCount;
    private float baseProjectileSize;
    private float baseProjectileSpeed;
    private float baseProjectileLifeTime;
    private bool baseIsAoeDamage;


    [Header("当前属性")]
    [SerializeField][Header("子弹伤害")] private float damage;
    [SerializeField][Header("射速")] private float fireRate;
    [SerializeField][Header("击退距离")] private float knockback;
    [SerializeField][Header("弹道数量")] private int projectileCount;
    [SerializeField][Header("子弹大小")] private float projectileSize;
    [SerializeField][Header("飞行速度")] private float projectileSpeed;
    [SerializeField][Header("生命周期")] private float projectileLifeTime;
    [SerializeField][Header("范围伤害")] private bool isAoeDamage;

    [Header("设置")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    private float nextFireTime;
    private bool isRotating;
    private PlayerInput playerInput;
    private InputAction fireAction;

    private void Awake()
    {
        playerInput = new PlayerInput();
        fireAction = playerInput.Player.Fire;
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }
    private void OnDisable()
    {
        playerInput.Disable();
    }

    /// <summary>
    /// 玩家射击系统初始化
    /// </summary>
    /// <param name="playerData"></param>
    public void Initialize(PlayerSO playerData)
    {
        // 存储基础值
        baseDamage = playerData.shootingConfig.damage;
        baseFireRate = playerData.shootingConfig.fireRate;
        baseKnockback = playerData.shootingConfig.knockback;
        baseProjectileCount = playerData.shootingConfig.projectileCount;
        baseProjectileSize = playerData.shootingConfig.projectileSize;
        baseProjectileSpeed = playerData.shootingConfig.projectileSpeed;
        baseProjectileLifeTime = playerData.shootingConfig.projectileLifeTime;
        baseIsAoeDamage = playerData.shootingConfig.isAoeDamage;

        // 初始化当前值
        damage = baseDamage;
        fireRate = baseFireRate;
        knockback = baseKnockback;
        projectileCount = baseProjectileCount;
        projectileSize = baseProjectileSize;
        projectileSpeed = baseProjectileSpeed;
        projectileLifeTime = baseProjectileLifeTime;
        isAoeDamage = baseIsAoeDamage;
    }

    /// <summary>
    /// 恢复正常射击
    /// </summary>
    public void ResetToBaseStats()
    {
        damage = baseDamage;
        fireRate = baseFireRate;
        knockback = baseKnockback;
        projectileCount = baseProjectileCount;
        projectileSize = baseProjectileSize;
        projectileSpeed = baseProjectileSpeed;
        projectileLifeTime = baseProjectileLifeTime;
        isAoeDamage = baseIsAoeDamage;
    }

    private void Update()
    {
        if (isRotating)
        {
            RotateTowardsMouse();
        }

        if (fireAction.IsPressed() && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    /// <summary>
    /// 射击
    /// </summary>
    private void Shoot()
    {
        float angleStep = projectileCount > 1 ? 15f / (projectileCount - 1) : 0f;
        float startAngle = -(angleStep * (projectileCount - 1)) / 2f;

        var config = new BulletConfig
        {
            damage = damage,
            knockback = knockback,
            isAoeDamage = isAoeDamage,
            size = projectileSize,
            lifeTime = projectileLifeTime,
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



    /// <summary>
    /// 跟随鼠标旋转
    /// </summary>
    private void RotateTowardsMouse()
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


    #region 公共属性
    public float Damage => baseDamage;
    public float FireRate => baseFireRate;
    public float Knockback => baseKnockback;
    public int ProjectileCount => baseProjectileCount;
    public float ProjectileSize => baseProjectileSize;
    public float ProjectileSpeed => baseProjectileSpeed;
    public float ProjectileLifeTime => baseProjectileLifeTime;
    public bool IsAoeDamage => baseIsAoeDamage;
    #endregion

    #region 增益效果相关方法
    public void AddDamage(float amount) => baseDamage += amount;
    public void AddFireRate(float amount) => baseFireRate += amount;
    public void AddKnockback(float amount) => baseKnockback += amount;
    public void AddProjectileCount(int amount) => baseProjectileCount = Mathf.Min(projectileCount + amount, 5);
    public void AddProjectileSize(float amount) => baseProjectileSize += amount;
    public void SetAoeDamage(bool value) => baseIsAoeDamage = value;

    #endregion


}

