using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemies/Enemy")]
public class EnemySO : ScriptableObject
{
    public EnemyType enemyType;
    public GameObject enemyPrefab;

    [TextArea] public string description;

    [Tooltip("生成权重"), Range(0.1f, 10f)]
    public float spawnWeight = 1f;

    [Tooltip("解锁该敌人的最低波次")]
    public int unlockWave = 0;

    [Tooltip("Boss敌人必须设为true")]
    public bool isBoss = false;

    [Tooltip("大型敌人必须设为true")]
    public bool isLargeEnemy = false;

    [Header("移动速度")] public float moveSpeed;
    [Header("血量上限")] public float maxHealth;
    [Header("碰撞伤害")] public float collisionDamage;
    [Header("受击无敌")] public float collisionImmunityDuration;
    [Header("敌人碰撞检测半径")] public float attackRadius;
    [Header("检测间隔（秒）")] public float detectionInterval;

    // 按需显示的配置
    public ShootingConfig shootingConfig;
    public ClashConfig clashConfig;
    public RewardConfig rewardConfig;



    // 条件显示的数据块
    [System.Serializable]
    public class ShootingConfig
    {
        [Header("敌人子弹预制体")]public GameObject bulletPrefab;
        [Header("子弹伤害")]public float bulletDamage;
        [Header("子弹速度")] public float bulletSpeed;
        [Header("子弹大小")] public float bulletSize;
        [Header("射速")] public float shootRate;
        [Header("开始射击距离")] public float shootRadius;
        [Header("弹道数量")] public int projectileCount;
        [Header("生命周期")] public float bulletLifeTime;
    }

    [System.Serializable]
    public class ClashConfig
    {
        [Header("发起冲撞距离")] public float seekRadius;
        [Header("冲撞速度")] public float clashSpeed;
        [Header("冲撞冷却")] public float clashCooldown;
    }

    [System.Serializable]
    public class RewardConfig
    {
        [Header("奖励配置")]
        public bool hasHealthReward = false;
        [Header("回血")]
        public float healthUp;

        public bool hasRefreshReward = false;
        [Header("增加刷新次数")]
        public int extraRefreshChance;
    }

}

public enum EnemyType
{
    Base,
    Reward,
    Remote,
    Clash,
    BigBase,
    BigRemote,
    BigClash,
    BigReward,
    Boss,
}
