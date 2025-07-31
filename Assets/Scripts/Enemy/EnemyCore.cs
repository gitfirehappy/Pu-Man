using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using static EnemyEvent;
using static ObjectPoolManager;
using static Unity.VisualScripting.Member;

[DisallowMultipleComponent]
public class EnemyCore : MonoBehaviour, IPoolable
{
    [SerializeField] private EnemySO enemyData;
    [SerializeField] private IObjectPool<GameObject> _managedPool;

    public DamageSource LastDamageSource { get; private set; }//最后伤害来源

    public event Action OnEnemyDeath;

    // 组件引用缓存
    private EnemyHealth _health;
    private EnemyMovement _movement;
    private EnemyShooting _shooting;
    private EnemyClash _clash;
    private EnemyReward _reward;
    private EnemyAnimatorController animatorController;

    private void Awake()
    {
        if (enemyData == null)
        {
            Debug.LogError("EnemySO not assigned!");
            return;
        }

        InitializeComponents();
    }

    /// <summary>
    /// 敌人组件初始化
    /// </summary>
    public void InitializeComponents()
    {

        // 确保基础组件存在
        _health = GetOrAddComponent<EnemyHealth>();
        _movement = GetOrAddComponent<EnemyMovement>();
        animatorController = GetOrAddComponent<EnemyAnimatorController>();

        // 获取当前类型的额外属性
        var bonusStats = EnemyManager.Instance.GetBonusStats(enemyData.enemyType);

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
        _health.Initialize(enemyData, this, bonusStats);
        _movement.Initialize(enemyData);
        _shooting?.Initialize(enemyData, bonusStats);
        _clash?.Initialize(enemyData);
        _reward?.Initialize(enemyData);
        animatorController?.Initialize();

        RegisterEventHandlers();

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
        if (_health != null)
        {
            _health.OnDeath -= HandleDeath; // 先取消旧注册
            _health.OnDeath += HandleDeath; // 再重新注册
        }
    }

    /// <summary>
    /// 处理死亡事件
    /// </summary>
    private void HandleDeath(DamageSource source)
    {
        if (IsDead) return;

        // 触发内部事件
        OnEnemyDeath?.Invoke();

        // 触发全局事件
        EnemyEvent.TriggerDeath(this, source);

        // 如果是Boss，额外触发全局事件
        if (EnemyData.isBoss)
        {
            EnemyEvent.TriggerBossState(EnemyEvent.BossState.WaveEnded);
            AudioManager.Instance.ClearConditionTag();
        }

        // 延迟回收确保事件完成
        StartCoroutine(DelayedReturn());
    }

    private IEnumerator DelayedReturn()
    {
        // 等待两帧确保所有事件处理完成
        yield return null;
        yield return null;

        if (this == null || gameObject == null) yield break;

        ReturnToPool();
    }

    // IPoolable接口实现
    /// <summary>
    /// 回收敌人调用
    /// </summary>
    public void OnRelease()
    {
        _movement?.ResetToBaseStats();
        _shooting?.ResetToBaseStats();
        _clash?.ResetToBaseStats();
        _reward?.ResetToBaseStats();
        _health?.ResetToBaseStats();
        animatorController?.ResetToBaseStats();
    }

    /// <summary>
    /// 取出敌人调用
    /// </summary>
    public void OnGet()
    {
        _movement?.ResetToBaseStats();
        _shooting?.ResetToBaseStats();
        _clash?.ResetToBaseStats();
        _reward?.ResetToBaseStats();
        _health?.ResetToBaseStats();
        animatorController?.ResetToBaseStats();

        StartCoroutine(DelayedSpawnEvent());
    }

    private IEnumerator DelayedSpawnEvent()
    {
        yield return null; // 等待一帧确保SetPool被调用

        // 重新注册事件
        RegisterEventHandlers();
        EnemyEvent.TriggerSpawned(this);

        if (EnemyData.isBoss)
        {
            EnemyEvent.TriggerBossState(BossState.Spawned, this);
        }
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
        if (this == null || gameObject == null) return;

        if (_managedPool != null)
        {
            // 移除事件监听避免回调到已回收对象
            if (_health != null)
                _health.OnDeath -= HandleDeath;

            Debug.Log($"成功回收敌人到对象池: {gameObject.name}", this);
            ObjectPoolManager.ReturnObjectToPool(gameObject, ObjectPoolManager.PoolType.Enemy);
        }
        else if (gameObject.activeInHierarchy)
        {
            Debug.LogError($"无法回收敌人: 未设置对象池引用. 敌人: {gameObject.name}", this);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage, DamageSource source)
    {
        LastDamageSource = source; // 记录伤害来源
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