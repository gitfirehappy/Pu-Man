using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveScalingConfig
{
    [Header("生成间隔配置")]
    [Tooltip("初始生成间隔(秒)")]
    public float initialSpawnInterval = 2f;

    [Tooltip("每波减少的生成间隔(秒)")]
    public float intervalReductionPerWave = 0.1f;

    [Tooltip("最小生成间隔(秒)")]
    public float minSpawnInterval = 0.2f;

    [Header("基础增长")]
    [Tooltip("每波血量增长百分比(0.1=10%)")]
    public float healthPerWave = 0.1f;

    [Tooltip("每波伤害增长百分比")]
    public float damagePerWave = 0.05f;

    [Header("Boss弹道增长")]
    [Tooltip("每次Boss出现增加的弹道数")]
    public int bossProjectilesPerSpawn = 3;

    [Tooltip("Boss最大弹道数")]
    public int maxBossProjectiles = 12;

    [Header("差异化配置")]
    [Tooltip("特殊增长配置(可选)")]
    public List<EnemyTypeScalingOverride> typeOverrides;
}

[System.Serializable]
public class EnemyTypeScalingOverride
{
    public EnemyType enemyType;
    public float healthPerWave = 0.1f;   // 类型特有血量增长系数
    public float damagePerWave = 0.05f;   // 类型特有伤害增长系数
}