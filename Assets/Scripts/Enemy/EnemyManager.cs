using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : SingletonMono<EnemyManager>
{
    [Header("波次成长配置")]
    [SerializeField] private WaveScalingConfig scalingConfig;

    private Dictionary<EnemyType, EnemyTypeScalingOverride> _typeOverrides;

    private List<EnemyCore> activeEnemies = new List<EnemyCore>();

    protected override void Init()
    {
        InitializeScalingOverrides();
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
    public void ChainKill(Vector2 origin, float radius, float chainKillDamage)
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            var enemy = activeEnemies[i];
            if (enemy == null || enemy.IsDead) continue;

            float distance = Vector2.Distance(origin, enemy.transform.position);
            if (distance <= radius)
            {
                //来源标记为ChainKill
                enemy.TakeDamage(chainKillDamage, DamageSource.ChainKill);
            }
        }

        Debug.Log($"亵渎技能生效，杀死了{activeEnemies.Count}个敌人");
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
}