using UnityEngine;

[CreateAssetMenu(fileName = "NewBuff", menuName = "Player/Buff")]
public class BuffSO : ScriptableObject
{
    public BuffID buffID;
    public Rarity rarity;
    public Sprite buffPicture;
    [TextArea] public string Description;

    // 所有可能的数值字段（但只会显示当前buffID需要的）
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


    [Header("史诗")]//bool类只能获得一次
    [Header("亵渎")] public bool replaceAbilityWithChainKill;
    [Header("随机技能")] public bool randomizeAbility;
    [Header("名刀")] public bool grantCheatDeath;
    [Header("范围伤害")] public bool aoeShot;
    [Header("全属性提升")] public float allNormalBuffModifier;

    [Header("传说")]
    [Header("减少出怪")] public bool reduceEnemy;
    [Header("叠甲")] public bool healthToArmor;
    [Header("狂暴")] public bool barserkMode;


    //private static readonly PlayerBuff _processor = new PlayerBuff();

    //public void Apply(PlayerCore player) => _processor.Apply(this, player);
    //public void Remove(PlayerCore player) => _processor.Remove(this, player);
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

