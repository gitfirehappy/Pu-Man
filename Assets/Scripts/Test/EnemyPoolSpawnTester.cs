// EnemyPoolSpawnTester.cs
using UnityEngine;

public class EnemyPoolSpawnTester : MonoBehaviour
{
    [Header("生成配置")]
    [SerializeField] private EnemySO enemyData; // 拖入需要测试的敌人SO预制体

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private KeyCode spawnKey = KeyCode.T;

    private GameObject _currentEnemy;

    private void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            SpawnEnemyFromPool();
        }
    }

    private void SpawnEnemyFromPool()
    {
        // 如果已有敌人则回收
        if (_currentEnemy != null)
        {
            ObjectPoolManager.ReturnObjectToPool(_currentEnemy, ObjectPoolManager.PoolType.Enemy);
        }

        // 从对象池获取敌人
        _currentEnemy = ObjectPoolManager.SpawnObject(
            enemyData.enemyPrefab,
            spawnPoint.position,
            Quaternion.identity,
            ObjectPoolManager.PoolType.Enemy
        );

        // 获取并初始化核心组件
        var core = _currentEnemy.GetComponent<EnemyCore>();
        if (core != null)
        {
            // 获取对象池实例并设置给敌人
            var pool = ObjectPoolManager.GetPoolForPrefab(enemyData.enemyPrefab);
            core.SetPool(pool);
            core.OnGet(); // 手动触发对象池获取逻辑
        }

        Debug.Log($"从对象池生成敌人: {enemyData.enemyType}", _currentEnemy);
    }
}