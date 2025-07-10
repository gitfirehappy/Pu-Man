using System;
using UnityEngine;
using UnityEngine.Pool;
using static ObjectPoolManager;

[DisallowMultipleComponent]
public class EnemyCore : MonoBehaviour, IPoolable
{
    [SerializeField] private EnemySO enemyData;
    [SerializeField] private IObjectPool<GameObject> _managedPool;

    public event Action OnEnemyDeath;

    // 组件引用缓存
    private EnemyHealth _health;
    private EnemyMovement _movement;
    private EnemyShooting _shooting;
    private EnemyClash _clash;
    private EnemyReward _reward;

    private void Awake()
    {
        if (enemyData == null)
        {
            Debug.LogError("EnemySO not assigned!");
            return;
        }

        InitializeComponents();
        RegisterEventHandlers();
    }

    /// <summary>
    /// 敌人组件初始化
    /// </summary>
    public void InitializeComponents()
    {
        // 确保基础组件存在
        _health = GetOrAddComponent<EnemyHealth>();
        _movement = GetOrAddComponent<EnemyMovement>();

        // 根据EnemyType动态添加组件
        switch (enemyData.enemyType)
        {
            case EnemyType.Remote:
            case EnemyType.BigRemote:
                _shooting = GetOrAddComponent<EnemyShooting>();
                break;

            case EnemyType.Clash:
            case EnemyType.BigClash:
                _clash = GetOrAddComponent<EnemyClash>();
                break;

            case EnemyType.Reward:
            case EnemyType.BigReward:
                _reward = GetOrAddComponent<EnemyReward>();
                break;

            case EnemyType.Boss:
                _shooting = GetOrAddComponent<EnemyShooting>();
                _clash = GetOrAddComponent<EnemyClash>();
                _reward = GetOrAddComponent<EnemyReward>();
                break;
        }

        // 初始化
        _health.Initialize(enemyData, this);
        _movement.Initialize(enemyData);
        _shooting?.Initialize(enemyData);
        _clash?.Initialize(enemyData);
        _reward?.Initialize(enemyData);
    }

    private T GetOrAddComponent<T>() where T : Component
    {
        var component = GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }

    /// <summary>
    /// 注册敌人事件
    /// </summary>
    private void RegisterEventHandlers()
    {
        _health.OnDeath += HandleDeath;
    }

    /// <summary>
    /// 处理死亡事件
    /// </summary>
    private void HandleDeath()
    {
        // 触发全局事件
        EnemyEvent.TriggerDeath(this,DamageSource.Player);

        // 触发内部事件（通知EnemyReward等组件）
        OnEnemyDeath?.Invoke();

        ReturnToPool();
    }

    // IPoolable接口实现
    /// <summary>
    /// 重置敌人所有状态，从关闭到启用
    /// </summary>
    public void OnRelease()
    {
        _movement?.ResetToBaseStats();
        _shooting?.ResetToBaseStats();
        _clash?.ResetToBaseStats();
        _reward?.ResetToBaseStats();
        _health?.ResetToBaseStats();
    }

    /// <summary>
    /// 从对象池取出对象时,从无到有的生成
    /// </summary>
    public void OnGet()
    {
        // 重新初始化
        InitializeComponents();
        EnemyEvent.TriggerSpawned(this); // 触发生成事件
    }

    public void SetPool(IObjectPool<GameObject> pool)
    {
        _managedPool = pool;
    }

    /// <summary>
    /// 统一回收
    /// </summary>
    public void ReturnToPool()
    {
        // 添加状态检查
        if (this == null || gameObject == null) return;

        if (_managedPool != null)
        {
            Debug.Log($"成功回收敌人到对象池: {gameObject.name}", this);
            _managedPool.Release(gameObject);
        }
        else
        {
            Debug.LogError($"无法回收敌人: 未设置对象池引用. 敌人: {gameObject.name}", this);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage, DamageSource source)
    {
        _health?.TakeDamage(damage, source);
    }

    #region 公共属性
    public EnemyType EnemyType => enemyData.enemyType;

    public bool IsDead => _health != null && _health.IsDead;

    public float CurrentHealth => _health.CurrentHealth;

    public float MaxHealth => _health.MaxHealth;

    public EnemySO EnemyData => enemyData;


    #endregion
}