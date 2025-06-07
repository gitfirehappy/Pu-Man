using System;
using UnityEditor.Timeline.Actions;
using UnityEngine;


public class PlayerBuff : ScriptableObject,IBuffEffect
{
    public BuffID buffID;

    [Header("数值效果")]
    // 通用属性
    [Header("增加生命")] public float healthModifier;
    [Header("增加护甲")] public float armorModifier;
    [Header("提升生命恢复")] public float healthRegenModifier;
    [Header("增加闪避率")] public float dodgeChanceModifier;
    [Header("增加碰撞伤害")] public float collisionDamageModifier;

    // 射击属性
    [Header("增加子弹伤害")] public float damageModifier;
    [Header("提升射速")] public float fireRateModifier;
    [Header("增强击退")] public float knockbackModifier;
    [Header("增加弹道")] public int projectileCountModifier;
    [Header("子弹变大")] public float projectileSizeModifier;
    [Header("范围伤害")] public bool setAoeDamage;

    // 移动属性
    [Header("增加移速")] public float speedModifier;

    // UI相关
    [Header("额外选择次数")] public int extraSelectionCount;
    [Header("刷新次数")] public int refreshChanceBonus;

    // 特殊效果
    [Header("名刀")] public bool grantCheatDeath;
    [Header("减少出怪")] public bool reduceEnemySpawn;
    [Header("叠甲")] public bool convertHealthToArmor;
    [Header("亵渎")] public bool replaceAbilityWithChainKill;
    [Header("随机技能")] public bool randomizeAbility;
    [Header("重置技能冷却")] public bool resetAbilityCooldown;

    public void Apply(PlayerCore player)
    {
        switch (buffID)
        {
            // 血量类
            case BuffID.MaxHealthUp:
                player.Health.AddMaxHealth(healthModifier);
                break;
            case BuffID.ArmorUp:
                player.Health.AddArmor(armorModifier);
                break;

            // 射击类
            case BuffID.FireDamageUp:
                player.Shooting.AddDamage(damageModifier);
                break;
            case BuffID.ProjectileCountUp:
                player.Shooting.AddProjectileCount(projectileCountModifier);
                break;

            // 特殊效果
            case BuffID.CheatDeath:
                player.Health.SetCheatDeath(true);
                break;
            case BuffID.HealthToArmor:
                float healthReduction = player.Health.maxHealth - 1;
                player.Health.AddMaxHealth(-healthReduction);
                player.Health.AddArmor(healthReduction);
                break;

            // 更多case...
            default:
                Debug.LogWarning($"未实现的BuffID: {buffID}");
                break;
        }
    }

    public void Remove(PlayerCore player)
    {
        switch (buffID)
        {
            // 反向操作
            case BuffID.MaxHealthUp:
                player.Health.AddMaxHealth(-healthModifier);
                break;
                // 其他移除逻辑...
        }
    }
}