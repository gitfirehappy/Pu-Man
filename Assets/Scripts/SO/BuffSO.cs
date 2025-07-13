using UnityEngine;

[CreateAssetMenu(fileName = "NewBuff", menuName = "Player/Buff")]
public class BuffSO : ScriptableObject
{
    public BuffID buffID;
    public Rarity rarity;
    public Sprite buffPicture;
    [TextArea] public string Description;

    [Header("是否为唯一buff(只能获得一次)")]
    public bool isUnique;

    // 所有可能的数值字段
    [Header("血量类")]
    [Header("增加生命")] public float healthModifier;
    [Header("增加护甲")] public float armorModifier;
    [Header("提升生命恢复")] public float healthRegenModifier;
    [Header("增加闪避率")] public float dodgeChanceModifier;
    [Header("增加碰撞伤害")] public float collisionDamageModifier;

    [Header("射击类")]
    [Header("增加子弹伤害")] public float damageModifier;
    [Header("提升射速")] public float fireRateModifier;
    [Header("增强击退")] public float knockbackModifier;
    [Header("增加弹道")] public int projectileCountModifier;
    [Header("子弹变大")] public float projectileSizeModifier;

    [Header("移动类")]
    [Header("增加移速")] public float speedModifier;

    [Header("稀有")]
    [Header("额外选择次数")] public int extraBuffChoices;
    [Header("刷新次数")] public int extraRefreshChance;
    [Header("减少技能冷却")] public int reduceAbilityCooldown;

    [Header("史诗")]
    [Header("全属性提升百分比")] public float allNormalBuffModifier;
    [Header("名刀时间")] public float cheatDeathInvisibleTime;
    [Header("亵渎冷却")] public int chainKillCooldown;

    [Header("传说")]
    [Header("减少出怪量")] public float reduceEnemySpawn;
    [Header("护甲转伤害比例")] public float amorToDamageModifier;
}


public enum Rarity { Common, Rare, Epic, Legendary }

public enum BuffID
{
    None,
    //普通

    //血量
    MaxHealthUp,
    ArmorUp,
    HealthRegenUp,
    DogeChanceUp,
    CollitionDamageUp,

    //射击
    FireDamageUp,
    FireRateUp,
    KnockbackUp,
    ProjectileCountUp,
    ProjectileSizeUp,

    //移动
    SpeedUp,

    //稀有
    ExtraBuff,//下次获得额外buff
    ExtraRefreshChance,//额外刷新机会
    ReduceAbilityCooldown,//减少技能冷却

    //史诗
    CheatDeath,//名刀
    AoeShot,//范围伤害
    AllNormalBuff,//全属性提升
    ChangeSkill,//换技能
    ChainKillSkill,//亵渎

    //传说
    HealthToArmor,//血量转护甲
    ReduceEnemy,//减少出怪
    Barserk,//狂暴
    
}

