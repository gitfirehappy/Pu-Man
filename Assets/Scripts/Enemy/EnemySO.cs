using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemies/Enemy")]
public class EnemySO : ScriptableObject
{
    public EnemyType enemyType;
    public Rarity rarity;
    [TextArea] public string description;

    [Header("基础属性")]
    [Header("血量上限")] public float maxHealth;
    [Header("移动速度")] public float moveSpeed;
    [Header("碰撞伤害")] public float collisionDamage;
    [Header("受击无敌")] public float collisionImmunityDuration;
    [Tooltip("敌人碰撞检测半径")]
    public float attackRadius;
    [Tooltip("检测间隔（秒）")]
    public float detectionInterval;

    [Header("远程敌人属性")]
    [Header("子弹伤害")]public float bulletDamage;
    [Header("子弹速度")] public float bulletSpeed;
    [Header("子弹速度")] public float bulletSize;

    [Header("冲撞敌人属性")]
    [Header("发起冲撞距离")]public float seekRadius;
    [Header("冲撞速度")]public float clashSpeed;

    [Header("Boss敌人属性")]

    [Header("弹道数量")] public int projectileCount;//之后会随波次增加（在Boss逻辑会写）

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
