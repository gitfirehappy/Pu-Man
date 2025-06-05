using UnityEngine;

[CreateAssetMenu(fileName = "NewBuff", menuName = "Player/Buff")]
public class PlayerBuff : ScriptableObject
{
    public enum BuffType { Health, Shooting, Movement, Ability, UI }
    public enum Rarity { Common, Rare, Epic, Legendary } // 更新稀有度分级

    [Header("基础信息")]
    [Header("名称")] public string buffName;
    public Sprite icon;
    public BuffType buffType;
    [Header("稀有度")] public Rarity rarity;
    public bool isStackable;
    public bool isUnique;

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
        switch (buffType)
        {
            case BuffType.Health:
                ApplyHealthEffects(player);
                break;
            case BuffType.Shooting:
                ApplyShootingEffects(player);
                break;
            case BuffType.Movement:
                ApplyMovementEffects(player);
                break;
            case BuffType.Ability:
                ApplyAbilityEffects(player);
                break;
            case BuffType.UI:
                ApplyUIEffects(player);
                break;
        }

        ApplySpecialEffects(player);
    }

    public void Remove(PlayerCore player)
    {
        switch (buffType)
        {
            case BuffType.Health:
                RemoveHealthEffects(player);
                break;
            case BuffType.Shooting:
                RemoveShootingEffects(player);
                break;
            case BuffType.Movement:
                RemoveMovementEffects(player);
                break;
                // UI和能力类buff通常是永久性的，不需要移除
        }
    }

    private void ApplyHealthEffects(PlayerCore player)
    {
        player.Health.AddMaxHealth(healthModifier);
        player.Health.AddArmor(armorModifier);
        player.Health.AddHealthRegen(healthRegenModifier);
        player.Health.AddDodgeChance(dodgeChanceModifier);
        player.Health.AddCollitionDamage(collisionDamageModifier);
    }

    private void RemoveHealthEffects(PlayerCore player)
    {
        player.Health.AddMaxHealth(-healthModifier);
        player.Health.AddArmor(-armorModifier);
        player.Health.AddHealthRegen(-healthRegenModifier);
        player.Health.AddDodgeChance(-dodgeChanceModifier);
        player.Health.AddCollitionDamage(-collisionDamageModifier);
    }

    private void ApplyShootingEffects(PlayerCore player)
    {
        player.Shooting.AddDamage(damageModifier);
        player.Shooting.AddFireRate(fireRateModifier);
        player.Shooting.AddKnockback(knockbackModifier);
        player.Shooting.AddProjectileCount(projectileCountModifier);
        player.Shooting.AddProjectileSize(projectileSizeModifier);

        if (setAoeDamage)
        {
            player.Shooting.SetAoeDamage(true);
        }
    }

    private void RemoveShootingEffects(PlayerCore player)
    {
        player.Shooting.AddDamage(-damageModifier);
        player.Shooting.AddFireRate(-fireRateModifier);
        player.Shooting.AddKnockback(-knockbackModifier);
        player.Shooting.AddProjectileCount(-projectileCountModifier);
        player.Shooting.AddProjectileSize(-projectileSizeModifier);
    }

    private void ApplyMovementEffects(PlayerCore player)
    {
        player.Movement.AddSpeed(speedModifier);
    }

    private void RemoveMovementEffects(PlayerCore player)
    {
        player.Movement.AddSpeed(-speedModifier);
    }

    private void ApplyAbilityEffects(PlayerCore player)
    {
        if (resetAbilityCooldown)
        {
            // 需要在PlayerAbilities中实现重置冷却方法
            // player.Abilities.ResetAbilityCooldown();
        }
    }

    private void ApplyUIEffects(PlayerCore player)
    {
        // UI效果需要在游戏管理器中实现
        // GameManager.Instance.AddExtraSelections(extraSelectionCount);
        // GameManager.Instance.AddRefreshChances(refreshChanceBonus);
    }

    private void ApplySpecialEffects(PlayerCore player)
    {
        //名刀
        if (grantCheatDeath)
            player.Health.SetCheatDeath(true);

        //叠甲
        if (convertHealthToArmor)
        {
            float healthReduction = player.Health.maxHealth - 1;
            player.Health.AddMaxHealth(-healthReduction);
            player.Health.AddArmor(healthReduction);
        }

        // 其他特殊效果需要在游戏管理器或生成系统中实现
        // if (reduceEnemySpawn) EnemySpawner.ReduceSpawnRate();//减少生成
        // if (replaceAbilityWithChainKill) player.Abilities.ReplaceWithChainKill();//亵渎
        // if (randomizeAbility) player.Abilities.RandomizeAbility();//随机技能
    }
}