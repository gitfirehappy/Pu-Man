using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : SingletonMono<EnemySpawner>
{
    [Header("固定配置")]
    [SerializeField] private float minSpawnDistance = 5f; // 避免刷脸的最小距离
    [SerializeField] private float maxSpawnDistance = 15f; // 最大生成半径
    [SerializeField] private float groupSpawnRadius = 3f;   //敌人组生成半径
    [SerializeField] private int maxEnemies = 100;            // 全局敌人数上限

    [Header("地图边界配置")]
    [SerializeField] private float mapWidth = 50f;  // 地图宽度（X轴范围±值）
    [SerializeField] private float mapHeight = 50f; // 地图高度（Y轴范围±值）

    [Header("敌人预制体池")]
    [SerializeField] private List<GameObject> enemyPrefabs;

    private Transform playerTransform;
    private bool isSpawning;

    public List<GameObject> EnemyPrefabs => enemyPrefabs;


    protected override void Init()
    {
        PlayerManager.Instance.GetPlayerAsync(player =>
        {
            playerTransform = player.transform;
        });

        EventBus.OnBattleStart += StartSpawning;
        EventBus.OnPlayerDeath += StopSpawning;
        EventBus.OnTimeOut += HandleWaveTimeout;

        EventBus.OnBossWaveStarted += OnBossWaveStarted;
    }

    private void OnDestroy()
    {
        EventBus.OnBattleStart -= StartSpawning;
        EventBus.OnPlayerDeath -= StopSpawning;
        EventBus.OnTimeOut -= HandleWaveTimeout;

        EventBus.OnBossWaveStarted -= OnBossWaveStarted;
    }

    /// <summary>
    /// 倒计时为0
    /// </summary>
    private void HandleWaveTimeout()
    {
        StopSpawning();//停止生成
        StartCoroutine(CleanupAllEnemies());//清理所有敌人
    }

    /// <summary>
    /// 协程方式清理所有敌人，确保在下一帧前完成
    /// </summary>
    private IEnumerator CleanupAllEnemies()
    {
        // 获取所有活跃敌人
        var activeEnemies = EnemyManager.Instance.GetActiveEnemies();

        // 遍历并杀死所有敌人
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null && !enemy.IsDead)
            {
                enemy.TakeDamage(enemy.MaxHealth, DamageSource.SystemCleanup);
            }
        }

        yield return null; // 等待一帧确保所有敌人都被处理
    }

    private void StartSpawning()
    {
        isSpawning = true;
        Debug.Log("[EnemySpawner] 敌人生成已开始");
    }


    private void StopSpawning()
    {
        isSpawning = false;
        Debug.Log("[EnemySpawner] 敌人生成已停止");
    }

    private void Update()
    {
        if (!isSpawning) return;

        float currentInterval = EnemyManager.Instance.GetCurrentSpawnInterval();
        if (Time.time % currentInterval > Time.deltaTime) return;

        if (EnemyManager.Instance.GetActiveEnemyCount() < maxEnemies)
        {
            SpawnEnemyGroup();
        }
    }

    /// <summary>
    /// Boss波事件回调
    /// </summary>
    private void OnBossWaveStarted()
    {
        StartCoroutine(SpawnBossWithDelay());
    }

    private IEnumerator SpawnBossWithDelay()
    {
        yield return null; // 等待一帧确保事件处理完成

        GameObject bossPrefab = GetBossEnemy();
        if (bossPrefab != null)
        {
            Vector2 spawnPos = GetValidSpawnPosition();
            SpawnSingleEnemy(bossPrefab, spawnPos);
            //不用对boss做标记，事件会处理

            // 通知AudioManager更新BGM
            AudioManager.Instance.UpdateBGM();
        }
    }

    /// <summary>
    /// 普通生成敌人组
    /// </summary>
    private void SpawnEnemyGroup()
    {
        int currentWave = WaveCounter.Instance.CurrentWave;

        // 普通敌人生成逻辑（Boss波也会执行）
        GameObject enemyPrefab = GetRandomEnemy(currentWave);
        if (enemyPrefab == null) return;

        EnemySO enemyData = enemyPrefab.GetComponent<EnemyCore>().EnemyData;
        Vector2 spawnCenter = GetValidSpawnPosition();

        int spawnCount = enemyData.isLargeEnemy ?
            UnityEngine.Random.Range(1, 3) :  // 大型敌人1-2只
            UnityEngine.Random.Range(3, 6);   // 普通敌人3-5只

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 spawnPos = spawnCenter + UnityEngine.Random.insideUnitCircle * groupSpawnRadius;
            if (IsPositionInMap(spawnPos))
            {
                SpawnSingleEnemy(enemyPrefab, spawnPos);
            }
        }
    }

    /// <summary>
    /// 获取boss预制体
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private GameObject GetBossEnemy()
    {
        var bossPrefabs = enemyPrefabs.FindAll(p =>
             p.GetComponent<EnemyCore>().EnemyData.isBoss);

        return bossPrefabs.Count > 0 ?
            bossPrefabs[UnityEngine.Random.Range(0, bossPrefabs.Count)] :
            null;
    }

    /// <summary>
    /// 获取随机敌人
    /// </summary>
    /// <param name="currentWave"></param>
    /// <returns></returns>
    private GameObject GetRandomEnemy(int currentWave)
    {
        var availablePrefabs = enemyPrefabs.FindAll(p => {
            var data = p.GetComponent<EnemyCore>().EnemyData;
            return !data.isBoss && currentWave >= data.unlockWave;
        });

        if (availablePrefabs.Count == 0) return null;

        // 计算总权重
        float totalWeight = 0;
        foreach (var prefab in availablePrefabs)
            totalWeight += prefab.GetComponent<EnemyCore>().EnemyData.spawnWeight;

        // 权重随机
        float randomPoint = UnityEngine.Random.Range(0, totalWeight);
        float cumulativeWeight = 0;

        foreach (var prefab in availablePrefabs)
        {
            cumulativeWeight += prefab.GetComponent<EnemyCore>().EnemyData.spawnWeight;
            if (randomPoint <= cumulativeWeight)
                return prefab;
        }

        return availablePrefabs[0];
    }

    /// <summary>
    /// 随机生成位置
    /// </summary>
    /// <returns></returns>
    private Vector2 GetValidSpawnPosition()
    {
        Vector2 spawnPos;
        int attempts = 0;
        do
        {
            Vector2 dir = UnityEngine.Random.insideUnitCircle.normalized;
            float distance = UnityEngine.Random.Range(minSpawnDistance, maxSpawnDistance);
            spawnPos = (Vector2)playerTransform.position + dir * distance;
            attempts++;
        } while (!IsPositionInMap(spawnPos) && attempts < 10);

        return spawnPos;
    }

    /// <summary>
    /// 是否超出边界
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool IsPositionInMap(Vector2 pos)
    {
        return pos.x > -mapWidth / 2 && pos.x < mapWidth / 2 &&
               pos.y > -mapHeight / 2 && pos.y < mapHeight / 2;
    }

    /// <summary>
    /// 生成单个敌人
    /// </summary>
    /// <param name="enemyPrefab"></param>
    /// <param name="position"></param>
    private void SpawnSingleEnemy(GameObject enemyPrefab, Vector2 position)
    {
        var enemy = ObjectPoolManager.SpawnObject(
            enemyPrefab,
            position,
            Quaternion.identity,
            ObjectPoolManager.PoolType.Enemy
        ).GetComponent<EnemyCore>();

        enemy?.InitializeComponents();
    }

}
