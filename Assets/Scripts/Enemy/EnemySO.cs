using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemies/Enemy")]
public class EnemySO : ScriptableObject
{
    public EnemyType enemyType;

    [TextArea] public string description;

    [Header("基础属性")]
    [Header("移动速度")] public float moveSpeed;
    [Header("血量上限")] public float maxHealth;
    [Header("碰撞伤害")] public float collisionDamage;
    [Header("受击无敌")] public float collisionImmunityDuration;
    [Header("敌人碰撞检测半径")] public float attackRadius;
    [Header("检测间隔（秒）")] public float detectionInterval;


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
        [Header("回血")] public float healthUp;
        [Header("增加刷新次数")] public int extraRefreshChance;
    }


    // 按需显示的配置
    public ShootingConfig shootingConfig;
    public ClashConfig clashConfig;
    public RewardConfig rewardConfig;

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
