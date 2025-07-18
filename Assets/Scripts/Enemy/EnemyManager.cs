using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : SingletonMono<EnemyManager>
{
    [Header("波次成长配置")]
    [SerializeField] private WaveScalingConfig scalingConfig;

    private Dictionary<EnemyType, EnemyTypeScalingOverride> _typeOverrides;

    private List<EnemyCore> activeEnemies = new List<EnemyCore>();

    private float _currentSpawnInterval;
    public WaveScalingConfig ScalingConfig => scalingConfig;

    protected override void Init()
    {
        InitializeScalingOverrides();
        _currentSpawnInterval = scalingConfig.initialSpawnInterval;

        EventBus.OnWaveChanged += ApplyWaveScaling;

        EnemyEvent.OnSpawned += OnEnemySpawned;
        EnemyEvent.OnDeath += OnEnemyDeath;
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
    /// 应用敌人属性成长TODO：属性增长逻辑不对
    /// </summary>
    /// <param name="wave"></param>
    private void ApplyWaveScaling(int wave)
    {
        // 更新生成间隔
        _currentSpawnInterval = Mathf.Max(
            scalingConfig.minSpawnInterval,
            scalingConfig.initialSpawnInterval - (wave * scalingConfig.intervalReductionPerWave)
        );

        foreach (var enemy in activeEnemies)
        {
            if (enemy == null || enemy.IsDead) continue;

            // 获取敌人类型特定的成长系数
            var enemyType = enemy.EnemyType;
            float healthMultiplier = 1f;
            float damageMultiplier = 1f;

            if (_typeOverrides.TryGetValue(enemyType, out var overrideConfig))
            {
                healthMultiplier = overrideConfig.healthPerWave;
                damageMultiplier = overrideConfig.damagePerWave;
            }
            else
            {
                healthMultiplier = scalingConfig.healthPerWave;
                damageMultiplier = scalingConfig.damagePerWave;
            }

            // 应用成长
            enemy.ApplyWaveScaling(
                wave,
                healthMultiplier,
                damageMultiplier,
                scalingConfig.bossProjectilesPerSpawn,
                scalingConfig.maxBossProjectiles
            );
        }
    }

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
            _currentSpawnInterval * scalingConfig.difficultyMultiplier
        );
    }

    public void SetDifficultyMultiplier(float multiplier)
    {
        scalingConfig.difficultyMultiplier = Mathf.Clamp(multiplier, 0.5f, 2f);
    }
}