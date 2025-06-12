using UnityEngine;
using UnityEngine.Pool;

[DisallowMultipleComponent]
public class EnemyCore : MonoBehaviour, IPoolable
{
    [SerializeField] private EnemySO enemyData;
    private IObjectPool<GameObject> _managedPool;

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
    public void OnRelease()
    {
        // 重置所有状态
        GetComponent<EnemyHealth>().ResetHealth();
    }

    public void OnGet()
    {
        // 重新初始化
        InitializeComponents();
    }

    public void SetPool(IObjectPool<GameObject> pool)
    {
        _managedPool = pool;
    }

    public void ReturnToPool()
    {
        _managedPool?.Release(gameObject);
    }

    public EnemyType GetEnemyType() => enemyData.enemyType;
}