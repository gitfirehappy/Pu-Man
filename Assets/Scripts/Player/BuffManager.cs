using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : SingletonMono<BuffManager>
{
    [Header("已获得Buff")]
    [SerializeField]private Dictionary<BuffID, BuffSO> _acquiredBuffs = new Dictionary<BuffID, BuffSO>();

    protected override void Init()
    {
        // 注册GameOver状态事件，在游戏结束时清空Buff列表
        EventQueueManager.AddStateEvent(GameState.GameOver, ClearBuffs, 0);
    }

    /// <summary>
    /// 应用Buff（外部调用，如 UI 选择 Buff 时）
    /// </summary>
    /// <param name="buffData"></param>
    public void ApplyBuff(BuffSO buffData)
    {
        // 通过 PlayerManager 获取当前玩家（避免直接依赖 PlayerCore）
        var player = PlayerManager.Instance.Player;
        if (player == null)
        {
            Debug.LogWarning("No player registered! Buff not applied.");
            return;
        }

        // 检查唯一性 Buff
        if (buffData.isUnique && _acquiredBuffs.ContainsKey(buffData.buffID))
        {
            Debug.Log($"已获得唯一buff: {buffData.buffID}, 无法重复获取");
            return;
        }

        // 记录 Buff
        _acquiredBuffs[buffData.buffID] = buffData;

        // 应用 Buff 效果
        ApplyBuffEffect(buffData, player);
    }

    private void ApplyBuffEffect(BuffSO buffData, PlayerCore player)
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
                GameUIManager.Instance.GetSubUIManager<SelectBuffUIManager>().AddExtraBuffChoices(buffData.extraBuffChoices);
                Debug.Log($"下回合将增加{buffData.extraBuffChoices}次Buff选择机会");
                break;

            case BuffID.ExtraRefreshChance:
                GameUIManager.Instance.GetSubUIManager<SelectBuffUIManager>().AddRefreshCount(buffData.extraRefreshChance);
                Debug.Log($"增加{buffData.extraRefreshChance}次刷新机会");
                break;

            case BuffID.ReduceAbilityCooldown:
                player.Abilities.ReduceAbilityCooldown(buffData.reduceAbilityCooldown);
                break;

            // ========== 史诗增益 ==========
            case BuffID.CheatDeath:
                player.Health.SetCheatDeath(buffData.cheatDeathInvisibleTime);
                break;

            case BuffID.AoeShot:
                player.Shooting.SetAoeDamage();
                break;

            case BuffID.ChainKillSkill:
                var chainKillData = new AbilityData(AbilityType.ChainKill)
                {
                    cooldownWaves = buffData.chainKillCooldown // 从 BuffSO 读取冷却时间
                };
                player.Abilities.ChangeAbility(chainKillData);
                break;

            case BuffID.ChangeSkill:
                player.Abilities.RandomizeAbility();
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
                // 减少敌人生成频率
                EnemyManager.Instance.ReduceSpawnRate(buffData.reduceEnemySpawn);
                var NoneAbilityData = new AbilityData(AbilityType.None);
                player.Abilities.ChangeAbility(NoneAbilityData); // 禁用技能
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

    #region case调用

    /// <summary>
    /// 提升全属性
    /// </summary>
    /// <param name="player"></param>
    /// <param name="buffData"></param>
    private void AddAllNormalBuff(PlayerCore player, BuffSO buffData)
    {
        float smallBoost = buffData.allNormalBuffModifier;
        player.Health.AddMaxHealth(1 * smallBoost);
        player.Health.AddArmor(1 * smallBoost);
        player.Health.AddHealthRegen(1 * smallBoost);
        player.Health.AddDodgeChance(0.1f * smallBoost);
        player.Health.AddCollitionDamage(1 * smallBoost);

        player.Shooting.AddDamage(1 * smallBoost);
        player.Shooting.AddFireRate(1 * smallBoost);
        player.Shooting.AddKnockback(1 * smallBoost);
        player.Shooting.AddProjectileCount((int)(1 * smallBoost));
        player.Shooting.AddProjectileSize(0.1f * smallBoost);

        player.Movement.AddSpeed(1 * smallBoost);
    }

    /// <summary>
    /// 应用狂暴增益
    /// </summary>
    /// <param name="player"></param>
    /// <param name="buffData"></param>
    private void ApplyBerserkMode(PlayerCore player, BuffSO buffData)
    {
        player.Shooting.AddDamage(buffData.amorToDamageModifier);
        player.Shooting.AddFireRate(buffData.amorToDamageModifier);
        player.Health.AddArmor(-buffData.amorToDamageModifier);
    }

    /// <summary>
    /// 应用血转护甲
    /// </summary>
    /// <param name="player"></param>
    /// <param name="buffData"></param>
    private void ApplyHealthToArmor(PlayerCore player, BuffSO buffData)
    {
        float healthReduction = player.Health.MaxHealth - 1;
        player.Health.AddMaxHealth(-healthReduction);
        player.Health.AddArmor(healthReduction);
    }

    #endregion case调用


    public void ClearBuffs()
    {
        _acquiredBuffs.Clear();
    }

    public int GetBuffCount(Rarity rarity)
    {
        int count = 0;
        foreach (var buff in _acquiredBuffs.Values)
        {
            if (buff.rarity == rarity)
            {
                count++;
            }
        }
        return count;
    }

    //提供给 UI 查询已获得的 Buff
    public Dictionary<BuffID, BuffSO> GetAcquiredBuffs() => new Dictionary<BuffID, BuffSO>(_acquiredBuffs);
}
