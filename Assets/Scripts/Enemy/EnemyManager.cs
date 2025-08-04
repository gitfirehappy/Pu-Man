using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : SingletonMono<EnemyManager>
{
    [Header("波次成长配置")]
    [SerializeField] private WaveScalingConfig scalingConfig;

    private Dictionary<EnemyType, EnemyTypeScalingOverride> _typeOverrides;

    private Dictionary<EnemyType, EnemyBaseStats> _baseStatsTable = new Dictionary<EnemyType, EnemyBaseStats>();
    private Dictionary<EnemyType, EnemyBonusStats> _bonusStatsTable = new Dictionary<EnemyType, EnemyBonusStats>();

    private List<EnemyCore> activeEnemies = new List<EnemyCore>();

    [Header("生成频率")]
    [SerializeField] private float spawnRateReductionFactor = 1f;
    [SerializeField] private float _currentSpawnInterval;

    [Header("鸟群算法配置")]
    [Tooltip("鸟群邻居检测半径，用于判断哪些敌人属于同一群体")]
    public float boidNeighborRadius = 5f;
    [Tooltip("鸟群行为更新间隔时间（秒），控制算法计算频率以优化性能")]
    public float boidUpdateInterval = 0.2f;
    private float lastBoidUpdateTime;
    [Tooltip("鸟群个体间最小分离距离，防止敌人过度拥挤")]
    public float separationMinDistance = 1.5f;
    [Tooltip("凝聚力权重，控制敌人向群体中心聚集的强度")]
    public float boidCohesionWeight = 1f;
    [Tooltip("分离力权重，控制敌人保持个体间距的强度")]
    public float boidSeparationWeight = 2f;
    [Tooltip("对齐力权重，控制敌人与群体运动方向保持一致的强度")]
    public float boidAlignmentWeight = 1f;
    [Tooltip("目标吸引力权重，控制敌人向目标移动的强度（优先级高于群体行为）")]
    public float boidTargetWeight = 3f;

    private List<Vector2> enemyPositions = new List<Vector2>();
    private List<Vector2> enemyVelocities = new List<Vector2>();

    [Header("调试信息")]
    [SerializeField] private int debugActiveEnemyCount; // 仅用于显示

    public WaveScalingConfig ScalingConfig => scalingConfig;

    private void Update()
    {
        // 实时更新调试用的敌人数量（仅在编辑器中生效）
        debugActiveEnemyCount = activeEnemies.Count;

        if (Time.time - lastBoidUpdateTime > boidUpdateInterval)
        {
            UpdateEnemyPositionAndVelocityLists();
            BoidMath.UpdateSpatialGrid(enemyPositions, boidNeighborRadius);
            lastBoidUpdateTime = Time.time;
        }
    }

    /// <summary>
    /// 同步敌人位置和速度列表
    /// </summary>
    private void UpdateEnemyPositionAndVelocityLists()
    {
        enemyPositions.Clear();
        enemyVelocities.Clear();

        foreach (var enemy in activeEnemies)
        {
            if (enemy != null && !enemy.IsDead)
            {
                enemyPositions.Add(enemy.transform.position);
                var rb = enemy.GetComponent<Rigidbody2D>();
                enemyVelocities.Add(rb ? rb.linearVelocity : Vector2.zero);
            }
        }
    }

    protected override void Init()
    {
        InitializeBaseStats(); // 初始化基础属性表
        InitializeScalingOverrides();
        _currentSpawnInterval = scalingConfig.initialSpawnInterval;

        EventQueueManager.AddStateEvent(GameState.Battle, () =>
        {
            ApplyWaveScaling(WaveCounter.Instance.CurrentWave);
        }, 4);

        EnemyEvent.OnSpawned += OnEnemySpawned;
        EnemyEvent.OnDeath += OnEnemyDeath;
    }

    #region 增长配置
    /// <summary>
    /// 初始化敌人基础属性表
    /// </summary>
    private void InitializeBaseStats()
    {
        foreach (var prefab in EnemySpawner.Instance.EnemyPrefabs)
        {
            var core = prefab.GetComponent<EnemyCore>();
            if (core == null || core.EnemyData == null) continue;

            var type = core.EnemyData.enemyType;
            if (_baseStatsTable.ContainsKey(type)) continue;

            var data = core.EnemyData;
            _baseStatsTable[type] = new EnemyBaseStats
            {
                maxHealth = data.maxHealth,
                bulletDamage = data.shootingConfig?.bulletDamage ?? 0,
                collisionDamage = data.collisionDamage
            };
        }
    }

    /// <summary>
    /// 差异化增长配置
    /// </summary>
    private void InitializeScalingOverrides()
    {
        _typeOverrides = new Dictionary<EnemyType, EnemyTypeScalingOverride>();
        if (scalingConfig.typeOverrides != null)
        {
            foreach (var overrideConfig in scalingConfig.typeOverrides)
            {
                _typeOverrides[overrideConfig.enemyType] = overrideConfig;
            }
        }
    }

    /// <summary>
    /// 特殊处理Boss弹道增长
    /// </summary>
    /// <param name="type"></param>
    /// <param name="wave"></param>
    /// <returns></returns>
    private int caculateBossProjectileCountBonus(EnemyType type, int wave)
    {
        int projectileBonus = 0;
        if (type == EnemyType.Boss)
        {
            projectileBonus = Mathf.Min(
                wave / WaveCounter.Instance.BossInterval * scalingConfig.bossProjectilesPerSpawn,
                scalingConfig.maxBossProjectiles
            );
        }
        return projectileBonus;
    }
    #endregion

    #region EnemyEvent事件
    private void OnEnemySpawned(EnemyCore enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    private void OnEnemyDeath(EnemyCore enemy, DamageSource source)
    {
        activeEnemies.Remove(enemy);
        Debug.Log($"敌人死亡，当前剩余数量: {activeEnemies.Count}");
    }
    #endregion

    /// <summary>
    /// 应用敌人属性成长
    /// </summary>
    /// <param name="wave"></param>
    private void ApplyWaveScaling(int wave)
    {
        // 更新生成间隔
        _currentSpawnInterval = Mathf.Max(
            scalingConfig.minSpawnInterval,
            scalingConfig.initialSpawnInterval - (wave * scalingConfig.intervalReductionPerWave)
        );

        //更新bonus增长数值表
        //达到随波次增加怪越强
        foreach (EnemyType type in Enum.GetValues(typeof(EnemyType))) 
        {
            if (!_bonusStatsTable.ContainsKey(type))
            {
                _bonusStatsTable[type] = new EnemyBonusStats();
            }

            // 获取成长系数
            float healthMultiplier = scalingConfig.healthPerWave;
            float damageMultiplier = scalingConfig.damagePerWave;
            float healthLinear = scalingConfig.healthLinearPerWave;
            float damageLinear = scalingConfig.damageLinearPerWave;

            if (_typeOverrides.TryGetValue(type, out var overrideConfig))
            {
                healthMultiplier = overrideConfig.healthPerWave;
                damageMultiplier = overrideConfig.damagePerWave;
                healthLinear = overrideConfig.healthLinearPerWave;
                damageLinear = overrideConfig.damageLinearPerWave;
            }

            var baseStats = GetBaseStatsForType(type); // 获取基础值
            var currentBonus = _bonusStatsTable[type];

            if(scalingConfig.growthMode == GrowthMode.Percentage)
            {
                //百分比增长
                //更新公式：xxbonus +=（EnemySo基础值+xxbonus）* 系数
                //或        xxbonus = （EnemySo基础值+xxbonus）* (1+系数) - 基础值
                _bonusStatsTable[type] = new EnemyBonusStats
                {
                    maxHealthBonus = (baseStats.maxHealth + currentBonus.maxHealthBonus) * (1 + healthMultiplier) - baseStats.maxHealth,
                    bulletDamageBonus = (baseStats.bulletDamage + currentBonus.bulletDamageBonus) * (1 + damageMultiplier) - baseStats.bulletDamage,
                    collisionDamageBonus = (baseStats.collisionDamage + currentBonus.collisionDamageBonus) * (1 + damageMultiplier) - baseStats.collisionDamage,
                    projectileCountBonus = caculateBossProjectileCountBonus(type, wave)
                };
            }
            else
            {
                // 线性固定值增长
                _bonusStatsTable[type] = new EnemyBonusStats
                {
                    maxHealthBonus = currentBonus.maxHealthBonus + healthLinear,
                    bulletDamageBonus = currentBonus.bulletDamageBonus + damageLinear,
                    collisionDamageBonus = currentBonus.collisionDamageBonus + damageLinear,
                    projectileCountBonus = caculateBossProjectileCountBonus(type, wave)
                };
            }
        }
        //内部初始化：data.xx + core.xxbonus
        //下面不需要操作，在内部组件初始化可以完成
    }

    /// <summary>
    /// 获取指定类型的基础属性
    /// </summary>
    private EnemyBaseStats GetBaseStatsForType(EnemyType type)
    {
        return _baseStatsTable.TryGetValue(type, out var stats) ? stats : new EnemyBaseStats();
    }

    #region 外部调用方法

    /// <summary>
    /// 亵渎技能
    /// </summary>
    /// <param name="origin">技能中心点</param>
    /// <param name="radius">作用半径</param>
    public void ChainKill(Vector2 origin, float radius, float damage, int maxChains)
    {
        if (PauseManager.Instance.IsPaused) return;

        int chainCount = 0;
        int totalKills = 0;

        while (chainCount < maxChains)
        {
            var hits = Physics2D.OverlapCircleAll(origin, radius, LayerMask.GetMask("Enemy"));
            bool killedByThisChain = false;

            // 造成伤害并检测死亡
            foreach (var hit in hits)
            {
                var enemy = hit.GetComponent<EnemyCore>();
                if (enemy == null || enemy.IsDead) continue;

                enemy.TakeDamage(damage, DamageSource.ChainKill);

                // 检测是否被当前技能击杀
                if (enemy.IsDead && enemy.LastDamageSource == DamageSource.ChainKill)
                {
                    killedByThisChain = true;
                    totalKills++;
                }
            }

            if (!killedByThisChain) break;
            chainCount++;
        }

        Debug.Log($"亵渎技能生效，连锁{chainCount}次，有效击杀{totalKills}个敌人");
    }

    public int GetActiveEnemyCount()
    {
        return activeEnemies.Count;
    }

    public List<EnemyCore> GetActiveEnemies()
    {
        return new List<EnemyCore>(activeEnemies);
    }

    public EnemyCore GetActiveBoss()
    {
        return activeEnemies.Find(e => e.EnemyData.isBoss);
    }

    public float GetCurrentSpawnInterval()
    {
        return Mathf.Max(
            scalingConfig.minSpawnInterval,
            _currentSpawnInterval
        )/ spawnRateReductionFactor;
    }

    public EnemyBonusStats GetBonusStats(EnemyType type)
    {
        return _bonusStatsTable.TryGetValue(type, out var stats) ? stats : new EnemyBonusStats();
    }

    public void ReduceSpawnRate(float reductionFactor)
    {
        spawnRateReductionFactor *= reductionFactor;
        Debug.Log($"敌人生成频率降低至原来的{spawnRateReductionFactor * 100}%");
    }

    public List<Vector2> GetEnemyPositions() => new List<Vector2>(enemyPositions);
    public List<Vector2> GetEnemyVelocities() => new List<Vector2>(enemyVelocities);
    public int GetEnemyIndex(EnemyCore self)
    {
        return activeEnemies.IndexOf(self);
    }
    public float GetBoidNeighborRadius()
    {
        return boidNeighborRadius;
    }

    #endregion
}

/// <summary>
/// 增长数值表
/// </summary>
[System.Serializable]
public struct EnemyBonusStats
{
    public float maxHealthBonus;
    public float bulletDamageBonus;
    public float collisionDamageBonus;
    public int projectileCountBonus;//boss特有
}

/// <summary>
/// 基础数值表(和增长数值表同步）
/// </summary>
[System.Serializable]
public struct EnemyBaseStats
{
    public float maxHealth;
    public float bulletDamage;
    public float collisionDamage;
}