using System;
using UnityEditor.Timeline.Actions;
using UnityEngine;


public class PlayerBuff : MonoBehaviour,IBuffEffect
{
    public void Apply(BuffSO buffData, PlayerCore player)
    {
        switch (buffData.buffID)
        {
            // ========== 普通增益 ==========
            case BuffID.MaxHealthUp:
                player.Health.AddMaxHealth(buffData.healthModifier);
                break;

            case BuffID.ArmorUp:
                player.Health.AddArmor(buffData.armorModifier);
                break;

            case BuffID.HealthRegenUp:
                player.Health.AddHealthRegen(buffData.healthRegenModifier);
                break;

            case BuffID.DogeChanceUp:
                player.Health.AddDodgeChance(buffData.dodgeChanceModifier);
                break;

            case BuffID.CollitionDamageUp:
                player.Health.AddCollitionDamage(buffData.collisionDamageModifier);
                break;

            case BuffID.FireDamageUp:
                player.Shooting.AddDamage(buffData.damageModifier);
                break;

            case BuffID.FireRateUp:
                player.Shooting.AddFireRate(buffData.fireRateModifier);
                break;

            case BuffID.KnockbackUp:
                player.Shooting.AddKnockback(buffData.knockbackModifier);
                break;

            case BuffID.ProjectileCountUp:
                player.Shooting.AddProjectileCount(buffData.projectileCountModifier);
                break;

            case BuffID.ProjectileSizeUp:
                player.Shooting.AddProjectileSize(buffData.projectileSizeModifier);
                break;

            case BuffID.SpeedUp:
                player.Movement.AddSpeed(buffData.speedModifier);
                break;

            // ========== 稀有增益 ==========
            case BuffID.ExtraBuff:
                // 需要在关卡管理器中实现
                // GameManager.Instance.AddExtraBuffChoices(buffData.extraBuff);
                break;

            case BuffID.ExtraRefreshChance:
                // 需要在UI系统中实现
                // BuffSelectionUI.Instance.AddRefreshChances(buffData.extraRefreshChance);
                break;

            case BuffID.ReduceAbilityCooldown:
                player.Abilities.ReduceAbilityCooldown(buffData.reduceAbilityCooldown);
                break;


            // ========== 史诗增益 ==========
            case BuffID.CheatDeath:
                player.Health.SetCheatDeath(buffData.grantCheatDeath);
                break;

            case BuffID.AoeShot:
                player.Shooting.SetAoeDamage(buffData.aoeShot);
                break;

            case BuffID.ChainKillSkill:
                if (buffData.replaceAbilityWithChainKill)
                {
                    player.Abilities.ChangeAbility(AbilityType.ChainKill);
                }
                break;

            case BuffID.ChangeSkill:
                if (buffData.randomizeAbility)
                {
                    player.Abilities.RandomizeAbility();
                }
                break;

            case BuffID.AllNormalBuff:
                // 小幅度提升所有属性
                AddAllNormalBuff(player, buffData);
                break;

            // ========== 传说增益 ==========
            case BuffID.HealthToArmor:
                ApplyHealthToArmor(player, buffData);
                break;

            case BuffID.ReduceEnemy:
                if (buffData.reduceEnemy)
                {
                    // 需要在敌人生成器中实现
                    // EnemySpawner.Instance.ReduceSpawnRate();
                    player.Abilities.ChangeAbility(AbilityType.None); // 禁用技能
                }
                break;

            case BuffID.Barserk:
                // 狂暴模式：伤害翻倍，护甲减少
                ApplyBerserkMode(player, buffData);
                break;

            default:
                Debug.LogWarning($"未实现的Buff类型: {buffData.buffID}");
                break;
        }
    }

    public void Remove(BuffSO buffData, PlayerCore player)
    {
        // 反向操作（如减少血量等）
    }

    #region case调用

    /// <summary>
    /// 提升全属性
    /// </summary>
    /// <param name="player"></param>
    /// <param name="buffData"></param>
    private void AddAllNormalBuff(PlayerCore player, BuffSO buffData)
    {
        float smallBoost = buffData.allNormalBuffModifier;
        player.Health.AddMaxHealth(player.Health.MaxHealth * smallBoost);
        player.Health.AddArmor(player.Health.Armor * smallBoost);
        player.Health.AddHealthRegen(player.Health.HealthRegen * smallBoost);
        player.Health.AddDodgeChance(player.Health.DodgeChance * smallBoost);
        player.Health.AddCollitionDamage(player.Health.CollisionDamage * smallBoost);

        player.Shooting.AddDamage(player.Shooting.Damage * smallBoost);
        player.Shooting.AddFireRate(player.Shooting.FireRate * smallBoost);
        player.Shooting.AddKnockback(player.Shooting.Knockback * smallBoost);
        player.Shooting.AddProjectileCount(player.Shooting.ProjectileCount);
        player.Shooting.AddProjectileSize(player.Shooting.ProjectileSize * smallBoost);

        player.Movement.AddSpeed(player.Movement.RunSpeed * smallBoost);
    }

    /// <summary>
    /// 应用狂暴增益
    /// </summary>
    /// <param name="player"></param>
    /// <param name="buffData"></param>
    private void ApplyBerserkMode(PlayerCore player, BuffSO buffData)
    {
        if (buffData.barserkMode)
        {
            player.Shooting.AddDamage(player.Shooting.Damage);
            player.Shooting.AddFireRate(player.Shooting.FireRate);
            player.Health.AddArmor(-player.Health.Armor * 0.9f);
        }
    }

    /// <summary>
    /// 应用血转护甲
    /// </summary>
    /// <param name="player"></param>
    /// <param name="buffData"></param>
    private void ApplyHealthToArmor(PlayerCore player, BuffSO buffData)
    {
        if (buffData.healthToArmor)
        {
            float healthReduction = player.Health.MaxHealth - 1;
            player.Health.AddMaxHealth(-healthReduction);
            player.Health.AddArmor(healthReduction);
        }
    }

    #endregion

}