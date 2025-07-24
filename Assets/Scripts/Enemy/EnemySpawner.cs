using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : SingletonMono<EnemySpawner>
{
    [Header("固定配置")]
    [SerializeField] private float minSpawnDistance = 5f; // 避免刷脸的最小距离
    [SerializeField] private float groupSpawnRadius = 3f;   //敌人组生成半径
    [SerializeField] private int maxEnemies = 100;            // 全局敌人数上限
    [SerializeField] private float bossRadius = 20f;            // BOSS生成范围

    [Header("地图边界配置")]
    [SerializeField] private float mapWidth = 50f;  // 地图宽度（X轴范围±值）
    [SerializeField] private float mapHeight = 50f; // 地图高度（Y轴范围±值）

    [Header("敌人预制体池")]
    [SerializeField] private List<GameObject> enemyPrefabs;

    private Transform playerTransform;
    private bool isSpawning;
    private Coroutine spawnRoutine;

    public string BossBGMTag = "BossBattle";

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
        spawnRoutine = StartCoroutine(SpawnLoop());
        Debug.Log("[EnemySpawner] 敌人生成已开始");
    }


    private void StopSpawning()
    {
        isSpawning = false;
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
        Debug.Log("[EnemySpawner] 敌人生成已停止");
    }

    private IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            if (PauseManager.Instance.IsPaused)
            {
                yield return null;
                continue;
            }

            if (EnemyManager.Instance.GetActiveEnemyCount() < maxEnemies)
            {
                SpawnEnemyGroup();
            }

            float interval = EnemyManager.Instance.GetCurrentSpawnInterval();
            yield return new WaitForSeconds(interval);
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
            Vector2? bossSpawnPos = SampleBossSpawnPosition(bossRadius);
            if (bossSpawnPos.HasValue)
            {
                SpawnSingleEnemy(bossPrefab, bossSpawnPos.Value);

                AudioManager.Instance.UpdateBGM(BossBGMTag);//切换音乐
            }
            else
            {
                Debug.LogWarning("[EnemySpawner] 无法找到合适的Boss生成点！");
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
    /// 普通生成敌人组
    /// </summary>
    private void SpawnEnemyGroup()
    {
        int currentWave = WaveCounter.Instance.CurrentWave;

        // 普通敌人生成逻辑（Boss波也会执行）
        GameObject enemyPrefab = GetRandomEnemy(currentWave);
        if (enemyPrefab == null) return;

        EnemySO enemyData = enemyPrefab.GetComponent<EnemyCore>().EnemyData;

        int spawnCount = enemyData.isLargeEnemy ?
            UnityEngine.Random.Range(1, 3) :  // 大型敌人1-2只
            UnityEngine.Random.Range(3, 6);   // 普通敌人3-5只

        // 获取一个合适的生成点（外部 Poisson）
        var candidateCenters = SampleEnemyGroupPositions(10);
        if (candidateCenters.Count == 0) return;

        Vector2 spawnCenter = candidateCenters[0]; // 这里只用一个位置
        Vector2 worldCenter = spawnCenter - new Vector2(mapWidth, mapHeight) / 2f;

        // 获取组内敌人位置（圆形 Poisson）
        var offsets = SampleGroupInnerPositions(spawnCount, groupSpawnRadius);

        for (int i = 0; i < Mathf.Min(spawnCount, offsets.Count); i++)
        {
            Vector2 localOffset = offsets[i] - Vector2.one * groupSpawnRadius;
            Vector2 spawnPos = worldCenter + localOffset;

            if (IsPositionInMap(spawnPos))
                SpawnSingleEnemy(enemyPrefab, spawnPos);
        }
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

    #region 采样条件判断

    /// <summary>
    /// 敌人组采样条件判断
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private List<Vector2> SampleEnemyGroupPositions(int count)
    {
        Vector2 mapSize = new Vector2(mapWidth, mapHeight);

        return PoissonDiskSampler.GeneratePoints(
            radius: groupSpawnRadius * 2f, // 确保不同组不会重叠
            regionSize: mapSize,
            isValid: (pos) =>
            {
                // 坐标转为世界坐标（地图中心为0）
                Vector2 worldPos = pos - mapSize / 2f;
                bool insideMap = IsPositionInMap(worldPos);
                bool safeFromPlayer = playerTransform != null &&
                    Vector2.Distance(worldPos, playerTransform.position) > (minSpawnDistance + groupSpawnRadius);
                return insideMap && safeFromPlayer;
            }
        );
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
    /// 组内单个敌人采样
    /// </summary>
    /// <param name="count"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    private List<Vector2> SampleGroupInnerPositions(int count, float radius)
    {
        float regionDiameter = radius * 2;

        return PoissonDiskSampler.GeneratePoints(
            radius: 1.0f, // 敌人之间最小距离
            regionSize: new Vector2(regionDiameter, regionDiameter),
            isValid: (pos) =>
            {
                // 保证在圆形范围内（用于组内视觉分布）
                Vector2 center = new Vector2(regionDiameter / 2, regionDiameter / 2);
                return (pos - center).sqrMagnitude < radius * radius;
            }
        );
    }

    /// <summary>
    /// boss采样坐标
    /// </summary>
    /// <param name="bossSafeRadius"></param>
    /// <returns></returns>
    private Vector2? SampleBossSpawnPosition(float bossSafeRadius = 10f)
    {
        Vector2 regionSize = new Vector2(mapWidth, mapHeight);

        var candidates = PoissonDiskSampler.GeneratePoints(
            radius: 5f, // boss 本身距离其他 boss / 边界距离（不重要，因为只生成一个）
            regionSize: regionSize,
            isValid: (pos) =>
            {
                Vector2 worldPos = pos - regionSize / 2f;
                bool insideMap = IsPositionInMap(worldPos);
                bool awayFromPlayer = playerTransform != null &&
                    Vector2.Distance(worldPos, playerTransform.position) > bossSafeRadius;
                return insideMap && awayFromPlayer;
            }
        );

        if (candidates.Count > 0)
            return candidates[0] - regionSize / 2f;

        return null;
    }

    #endregion

}
