using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuff", menuName = "Player/Buff")]
public class BuffSO : ScriptableObject
{
    public BuffID buffID;
    public Rarity rarity;
    [TextArea] public string Description;

    [SerializeField]
    private PlayerBuff _effectData;

    // 添加这个属性方便访问
    public PlayerBuff EffectData => _effectData;

    public void Apply(PlayerCore player) => _effectData?.Apply(player);
    public void Remove(PlayerCore player) => _effectData?.Remove(player);

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
    RandomNormalBuff,//随机两个普通buff

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

