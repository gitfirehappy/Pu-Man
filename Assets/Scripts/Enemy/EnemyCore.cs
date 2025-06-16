using UnityEngine;
using UnityEngine.Pool;
using static ObjectPoolManager;

[DisallowMultipleComponent]
public class EnemyCore : MonoBehaviour, IPoolable
{
    [SerializeField] private EnemySO enemyData;
    [SerializeField] private IObjectPool<GameObject> _managedPool;

    private void Awake()
    {
        if (enemyData == null)
        {
            Debug.LogError("EnemySO not assigned!");
            return;
        }

        InitializeComponents();
    }

    private void InitializeComponents()
    {
        // 确保基础组件存在
        if (!TryGetComponent<EnemyHealth>(out var health))
            health = gameObject.AddComponent<EnemyHealth>();

        if (!TryGetComponent<EnemyMovement>(out var movement))
            movement = gameObject.AddComponent<EnemyMovement>();

        // 根据EnemyType动态添加组件
        switch (enemyData.enemyType)
        {
            case EnemyType.Remote:
            case EnemyType.BigRemote:
                if (!TryGetComponent<EnemyShooting>(out _))
                    gameObject.AddComponent<EnemyShooting>();
                break;

            case EnemyType.Clash:
            case EnemyType.BigClash:
                if (!TryGetComponent<EnemyClash>(out _))
                    gameObject.AddComponent<EnemyClash>();
                break;

            case EnemyType.Reward:
            case EnemyType.BigReward:
                if (!TryGetComponent<EnemyReward>(out _))
                    gameObject.AddComponent<EnemyReward>();
                break;

            case EnemyType.Boss:
                if (!TryGetComponent<EnemyShooting>(out _))
                    gameObject.AddComponent<EnemyShooting>();
                if (!TryGetComponent<EnemyClash>(out _))
                    gameObject.AddComponent<EnemyClash>();
                if (!TryGetComponent<EnemyReward>(out _))
                    gameObject.AddComponent<EnemyReward>();
                break;
        }

        // 初始化所有组件
        GetComponent<EnemyHealth>().Initialize(enemyData, this);
        GetComponent<EnemyMovement>().Initialize(enemyData);

        var shooting = GetComponent<EnemyShooting>();
        if (shooting != null) shooting.Initialize(enemyData);

        var clash = GetComponent<EnemyClash>();
        if (clash != null) clash.Initialize(enemyData);

        var reward = GetComponent<EnemyReward>();
        if (reward != null) reward.Initialize(enemyData);
    }

    // IPoolable接口实现
    /// <summary>
    /// 重置敌人所有状态
    /// </summary>
    public void OnRelease()
    {
        GetComponent<EnemyHealth>()?.ResetToBaseStats();
        GetComponent<EnemyMovement>()?.ResetToBaseStats();
        GetComponent<EnemyShooting>()?.ResetToBaseStats();
        GetComponent<EnemyClash>()?.ResetToBaseStats();
        GetComponent<EnemyReward>()?.ResetToBaseStats();
    }

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

    public EnemyType GetEnemyType() => enemyData.enemyType;
}