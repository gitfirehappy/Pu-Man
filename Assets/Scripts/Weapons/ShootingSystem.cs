using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ShootingSystem : MonoBehaviour
{
    [Header("子弹配置")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    
    [Header("射击属性")]
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float bulletDamage = 10f;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletSize = 1f;
    [SerializeField] private int bulletCount = 1;  // 子弹数量（用于散弹）
    [SerializeField] private float spreadAngle = 0f;  // 散射角度
    
    [Header("射击控制")]
    [SerializeField] private bool autoFire = true;  // 是否自动连发
    
    private bool isShooting = false;
    private bool canShoot = true;
    private PlayerInput inputControl;
    private WaitForSeconds waitForFireRate;

    private void Awake()
    {
        inputControl = new PlayerInput();
        waitForFireRate = new WaitForSeconds(fireRate);
        
        // 绑定输入事件
        inputControl.Player.Fire.started += OnFireStarted;
        inputControl.Player.Fire.canceled += OnFireCanceled;
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    private void OnFireStarted(InputAction.CallbackContext context)
    {
        isShooting = true;
        if (autoFire)
        {
            StartCoroutine(AutoShootRoutine());
        }
        else if (canShoot)
        {
            Shoot();
        }
    }

    private void OnFireCanceled(InputAction.CallbackContext context)
    {
        isShooting = false;
    }

    private IEnumerator AutoShootRoutine()
    {
        while (isShooting)
        {
            if (canShoot)
            {
                Shoot();
                canShoot = false;
                yield return waitForFireRate;
                canShoot = true;
            }
            yield return null;
        }
    }

    private void Shoot()
    {
        float angleStep = spreadAngle / (bulletCount > 1 ? bulletCount - 1 : 1);
        float startAngle = -spreadAngle / 2;

        for (int i = 0; i < bulletCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Vector2 direction = Quaternion.Euler(0, 0, currentAngle) * transform.right;
            
            GameObject bulletObj = ObjectPoolManager.SpawnObject(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            
            if (bullet != null)
            {
                bullet.Initialize(direction, bulletDamage, bulletSpeed, bulletSize);
            }
        }
    }

    // 提供公共方法用于修改射击属性
    public void SetFireRate(float newRate)
    {
        fireRate = newRate;
        waitForFireRate = new WaitForSeconds(fireRate);
    }

    public void SetBulletCount(int count)
    {
        bulletCount = Mathf.Max(1, count);
    }

    public void SetSpreadAngle(float angle)
    {
        spreadAngle = angle;
    }

    public void SetBulletProperties(float damage, float speed, float size)
    {
        bulletDamage = damage;
        bulletSpeed = speed;
        bulletSize = size;
    }
} 