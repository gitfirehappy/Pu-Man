using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField]private bool _isShootingEnabled = true;

    [Header("基础属性")]
    [Tooltip("子弹伤害")][SerializeField] private float baseDamage;
    [Tooltip("射速")][SerializeField] private float baseFireRate;
    [Tooltip("击退距离")][SerializeField]private float baseKnockback;
    [Tooltip("弹道数量")][SerializeField]private int baseProjectileCount;
    [Tooltip("子弹大小")][SerializeField]private float baseProjectileSize;
    [Tooltip("飞行速度")][SerializeField]private float baseProjectileSpeed;
    [Tooltip("生命周期")][SerializeField]private float baseProjectileLifeTime;
    [Tooltip("范围伤害")][SerializeField]private bool baseIsAoeDamage;


    [Header("当前属性")]
    [Tooltip("子弹伤害")][SerializeField] private float currentDamage;
    [Tooltip("射速")][SerializeField] private float currentFireRate;
    [Tooltip("击退距离")][SerializeField] private float currentKnockback;
    [Tooltip("弹道数量")][SerializeField] private int currentProjectileCount;
    [Tooltip("子弹大小")][SerializeField] private float currentProjectileSize;
    [Tooltip("飞行速度")][SerializeField] private float currentProjectileSpeed;
    [Tooltip("生命周期")][SerializeField] private float currentProjectileLifeTime;
    [Tooltip("范围伤害")][SerializeField] private bool currentIsAoeDamage;

    [Header("设置")]
    [Tooltip("子弹预制体")][SerializeField] private GameObject bulletPrefab;
    [Tooltip("开火点")][SerializeField] private Transform firePoint;
    [Tooltip("射击音效")][SerializeField] private AudioClip shootingSFX;

    private float lastShootTime;//高频音效处理
    private float nextFireTime;//射速管理

    [Tooltip("是否跟随鼠标")][SerializeField] private bool isRotating;

    private PlayerCore playerCore;
    private InputAction fireAction;
    public bool IsShooting { get; private set; }//射击状态

    private void Awake()
    {
        playerCore = GetComponent<PlayerCore>();

        if (firePoint == null)
        {
            firePoint = transform;
        }
    }
    private void Update()
    {
        if (PauseManager.Instance.IsPaused || !_isShootingEnabled) return;

        if (!_isShootingEnabled) return;

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

    #region PlayerCore相关
    public void EnableShooting()
    {
        _isShootingEnabled = true;
        isRotating = true;
        nextFireTime = 0f;
    }

    public void DisableShooting()
    {
        _isShootingEnabled = false;
        isRotating = false;
        nextFireTime = float.MaxValue; // 确保不会射击
    }

    /// <summary>
    /// 玩家射击系统初始化
    /// </summary>
    /// <param name="playerData"></param>
    public void Initialize(PlayerSO playerData)
    {
        if (playerCore != null && playerCore.playerInput != null)
        {
            fireAction = playerCore.playerInput.Player.Fire;
        }
        else
        {
            Debug.LogError("PlayerCore 或 PlayerInput 未正确初始化！");
        }

        isRotating = true;

        // 存储基础值
        bulletPrefab = playerData.shootingConfig.bulletPrefab;
        shootingSFX = playerData.shootingConfig.shootingSFX;

        baseDamage = playerData.shootingConfig.damage;
        baseFireRate = playerData.shootingConfig.fireRate;
        baseKnockback = playerData.shootingConfig.knockback;
        baseProjectileCount = playerData.shootingConfig.projectileCount;
        baseProjectileSize = playerData.shootingConfig.projectileSize;
        baseProjectileSpeed = playerData.shootingConfig.projectileSpeed;
        baseProjectileLifeTime = playerData.shootingConfig.projectileLifeTime;
        baseIsAoeDamage = playerData.shootingConfig.isAoeDamage;

        // 初始化当前值
        ResetToBaseStats();
    }

    /// <summary>
    /// 恢复射击参数
    /// </summary>
    public void ResetToBaseStats()
    {
        isRotating = true;

        currentDamage = baseDamage;
        currentFireRate = baseFireRate;
        currentKnockback = baseKnockback;
        currentProjectileCount = baseProjectileCount;
        currentProjectileSize = baseProjectileSize;
        currentProjectileSpeed = baseProjectileSpeed;
        currentProjectileLifeTime = baseProjectileLifeTime;
        currentIsAoeDamage = baseIsAoeDamage;
    }

    #endregion
    
    #region 射击逻辑
    /// <summary>
    /// 发射子弹逻辑
    /// </summary>
    private void Shoot()
    {
        // 添加预制体检查
        if (bulletPrefab == null)
        {
            Debug.LogError("PlayerShooting中子弹预制体未设置!");
            return;
        }

        // 播放射击音效 (高频播放优化)
        if (shootingSFX != null && Time.time > lastShootTime + 0.05f)
        {
            AudioManager.Instance.PlaySFX(shootingSFX);
            lastShootTime = Time.time;
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

            GameObject bulletObj = ObjectPoolManager.SpawnObject(bulletPrefab, firePoint.position, Quaternion.identity,ObjectPoolManager.PoolType.PlayerBullet);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bullet.Initialize(config, direction);
        }
    }

    /// <summary>
    /// 玩家跟随鼠标旋转
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
    #endregion

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
    public void AddFireRate(float amount) => Mathf.Min(baseFireRate += amount,15f);
    public void AddKnockback(float amount) => Mathf.Min(baseKnockback += amount,10f);
    public void AddProjectileCount(int amount) => baseProjectileCount = Mathf.Min(baseProjectileCount + amount, 5);
    public void AddProjectileSize(float amount) => Mathf.Min(baseProjectileSize += amount,1f);
    public void SetAoeDamage() => baseIsAoeDamage = true;

    //临时增益方法

    public void AddCurrentFireRate(float amount) => Mathf.Min(currentFireRate += amount,15f);

    #endregion
}

