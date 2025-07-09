using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("固定配置")]
    [SerializeField] private float minSpawnDistance = 5f; // 避免刷脸的最小距离
    [SerializeField] private float maxSpawnDistance = 15f; // 最大生成半径
    [SerializeField] private float groupSpawnRadius = 3f;   //敌人组生成半径
    [SerializeField] private int maxEnemies = 100;            // 全局敌人数上限

    [Header("地图边界配置")]
    [SerializeField] private float mapWidth = 50f;  // 地图宽度（X轴范围±值）
    [SerializeField] private float mapHeight = 50f; // 地图高度（Y轴范围±值）

    [Header("动态配置")]
    [SerializeField] private float spawnInterval = 3f;        // 每次生成的时间间隔

    [Header("敌人预制体池")]
    [SerializeField] private EnemySpawnConfig[] enemyPrefabs;

    private Transform playerTransform;
    private bool isSpawning;


    private void Awake()
    {
        playerTransform = FindObjectOfType<PlayerCore>().transform;
        EventBus.OnBattleStart += StartSpawning;
        EventBus.OnBuffSelected += StopSpawning;
        EventBus.OnPlayerDeath += StopSpawning;
    }

    private void OnDestroy()
    {
        EventBus.OnBattleStart -= StartSpawning;
        EventBus.OnBuffSelected -= StopSpawning;
        EventBus.OnPlayerDeath -= StopSpawning;
    }

    private void StartSpawning() => isSpawning = true;
    private void StopSpawning() => isSpawning = false;

    private void Update()
    {
        if (!isSpawning || Time.time % spawnInterval > Time.deltaTime)
            return;

        if (EnemyManager.Instance.GetActiveEnemyCount() < maxEnemies)
        {
            SpawnEnemyGroup();
        }
    }

    /// <summary>
    /// 生成敌人组
    /// </summary>
    private void SpawnEnemyGroup()
    {
        GameObject enemyPrefab = GetRandomEnemyByWeight();
        EnemySizeType sizeType = GetEnemySizeType(enemyPrefab.GetComponent<EnemyCore>().EnemyType);

        Vector2 spawnCenter = GetValidSpawnPosition();
        int spawnCount = sizeType == EnemySizeType.Normal ? Random.Range(3, 6) : Random.Range(1, 3);

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 spawnPos = spawnCenter + Random.insideUnitCircle * groupSpawnRadius;
            if (IsPositionInMap(spawnPos))
            {
                SpawnSingleEnemy(enemyPrefab, spawnPos);
            }
        }
    }

    /// <summary>
    /// 获取敌人大小
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private EnemySizeType GetEnemySizeType(EnemyType type)
    {
        return type.ToString().StartsWith("Big") ? EnemySizeType.Big :
               type == EnemyType.Boss ? EnemySizeType.Boss : EnemySizeType.Normal;
    }

    /// <summary>
    /// 敌人组生成位置
    /// </summary>
    /// <returns></returns>
    private Vector2 GetValidSpawnPosition()
    {
        Vector2 spawnPos;
        int attempts = 0;
        do
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
            spawnPos = (Vector2)playerTransform.position + dir * distance;
            attempts++;
        } while (!IsPositionInMap(spawnPos) && attempts < 10);

        return spawnPos;
    }

    /// <summary>
    /// 是否在地图内
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool IsPositionInMap(Vector2 pos)
    {
        return pos.x > -mapWidth / 2 && pos.x < mapWidth / 2 &&
               pos.y > -mapHeight / 2 && pos.y < mapHeight / 2;
    }

    /// <summary>
    /// 获取敌人权重
    /// </summary>
    /// <returns></returns>
    private GameObject GetRandomEnemyByWeight()
    {
        float totalWeight = 0;
        foreach (var config in enemyPrefabs) totalWeight += config.weight;

        float randomPoint = Random.Range(0, totalWeight);
        float cumulativeWeight = 0;

        foreach (var config in enemyPrefabs)
        {
            cumulativeWeight += config.weight;
            if (randomPoint <= cumulativeWeight)
                return config.prefab;
        }

        return enemyPrefabs[0].prefab;
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

        //对象池取出时会初始化
    }

    [System.Serializable]
    public class EnemySpawnConfig
    {
        public GameObject prefab; // 已配置EnemyCore+EnemySO的预制体
        [Range(0.1f, 10f)] public float weight = 1f;
    }

}
public enum EnemySizeType
{
    Normal = 0,
    Big = 1 << 0,
    Boss = 1 << 1
}
