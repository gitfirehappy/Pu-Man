using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerAbilities;

[CreateAssetMenu(fileName = "NewPlayer", menuName = "Player/Player")]
public class PlayerSO : ScriptableObject
{
    public PlayerType playerType;
    public string playerName;
    public Sprite playerSprite;
    [Header("玩家预制体")] public GameObject playerPrefab;

    [TextArea] public string description;

    public HealthConfig healthConfig;
    public ShootingConfig shootingConfig;
    public MovementConfig movementConfig;
    public AbilitiesConfig abilitiesConfig;

    [System.Serializable]
    public class HealthConfig
    {
        [Header("血量上限")] public float maxHealth;
        [Header("护甲值")] public float armor;
        [Header("生命恢复")] public float healthRegen;
        [Header("闪避率")] public float dodgeChance;
        [Header("碰撞伤害")] public float collisionDamage;
        [Header("碰撞受击无敌时间")] public float collisionImmunityDuration;

        [Header("名刀无敌时间")] public float cheatDeathInvincibleTime;
        [Header("是否有名刀")] public bool hasCheatDeath;

    }

    [System.Serializable]
    public class ShootingConfig
    {
        [Header("子弹伤害")] public float damage;
        [Header("射速")] public float fireRate;
        [Header("击退距离")] public float knockback;
        [Header("弹道数量")] public int projectileCount;
        [Header("子弹大小")] public float projectileSize;
        [Header("范围伤害")] public bool isAoeDamage;
        [Header("飞行速度")] public float projectileSpeed;
        [Header("生命周期")] public float projectileLifeTime;

        [Header("玩家子弹预制体")] public GameObject bulletPrefab;

    }

    [System.Serializable]
    public class MovementConfig
    {
        [Header("移动速度")]public float runSpeed;
    }

    [System.Serializable]
    public class AbilitiesConfig
    {
        [Header("初始技能")] public AbilityType startingAbility;

        [Header("经典技能配置")]
        public int classicCooldownWaves;
        public float classicDuration;

        [Header("狂暴技能配置")]
        public int berserkCooldownWaves;
        public float berserkDuration;
        public float berserkFireRateMultiplier;

        [Header("额外刷新技能配置")]
        public int extraRefreshChancesPerWave;

        [Header("亵渎技能配置")]
        public int chainkillCooldownWaves;

    }

}


public enum PlayerType
{
    Classic,    // 经典吐豆人：获得无敌
    Berserk,    // 狂暴吐豆人：短时间大幅提升攻速
    Skilled,    // 会玩的吐豆人：刷新次数+1
}
