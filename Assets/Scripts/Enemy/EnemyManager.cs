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
    [SerializeField]private float _currentSpawnInterval;

    public WaveScalingConfig ScalingConfig => scalingConfig;

    protected override void Init()
    {
        InitializeBaseStats(); // 初始化基础属性表
        InitializeScalingOverrides();
        _currentSpawnInterval = scalingConfig.initialSpawnInterval;

        EventBus.OnWaveChanged += ApplyWaveScaling;

        EnemyEvent.OnSpawned += OnEnemySpawned;
        EnemyEvent.OnDeath += OnEnemyDeath;
    }

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

        // 如果死亡的是Boss，通知AudioManager更新BGM
        if (enemy.EnemyData.isBoss)
        {
            AudioManager.Instance.UpdateBGM();
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

            // 获取特定类型成长系数
            float healthMultiplier = scalingConfig.healthPerWave;
            float damageMultiplier = scalingConfig.damagePerWave;

            if (_typeOverrides.TryGetValue(type, out var overrideConfig))
            {
                healthMultiplier = overrideConfig.healthPerWave;
                damageMultiplier = overrideConfig.damagePerWave;
            }

            //更新公式：xxbonus +=（EnemySo基础值+xxbonus）* 系数
            //或        xxbonus = （EnemySo基础值+xxbonus）* (1+系数) - 基础值
            var baseStats = GetBaseStatsForType(type); // 需要实现这个方法获取基础值
            var currentBonus = _bonusStatsTable[type];

            _bonusStatsTable[type] = new EnemyBonusStats
            {
                maxHealthBonus = (baseStats.maxHealth + currentBonus.maxHealthBonus) * (1 + healthMultiplier) - baseStats.maxHealth,
                bulletDamageBonus = (baseStats.bulletDamage + currentBonus.bulletDamageBonus) * (1 + damageMultiplier) - baseStats.bulletDamage,
                collisionDamageBonus = (baseStats.collisionDamage + currentBonus.collisionDamageBonus) * (1 + damageMultiplier) - baseStats.collisionDamage,
                projectileCountBonus = caculateBossProjectileCountBonus(type, wave)
            };
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

    #region 外部调用方法

    /// <summary>
    /// 亵渎技能
    /// </summary>
    /// <param name="origin">技能中心点</param>
    /// <param name="radius">作用半径</param>
    public void ChainKill(Vector2 origin, float radius, float damage, int maxChains)
    {
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