using UnityEngine;

[CreateAssetMenu(fileName = "NewBuff", menuName = "Player/Buff")]
public class PlayerBuff : ScriptableObject
{
    public enum BuffType { Health, Shooting, Movement, Ability }
    public enum Rarity { Common, Rare, Legendary }

    [Header("基础信息")]
    public string buffName;
    public Sprite icon;
    public BuffType buffType;
    public Rarity rarity;
    public bool isStackable;
    public bool isUnique;

    [Header("数值效果")]
    public float healthModifier;
    public float damageModifier;
    public float fireRateModifier;
    // 其他效果参数...

    public void Apply(PlayerCore player)
    {
        switch (buffType)
        {
            case BuffType.Health:
                player.Health.AddMaxHealth(healthModifier);
                break;
            case BuffType.Shooting:
                player.Shooting.AddDamage(damageModifier);
                break;
                // 其他类型处理...
        }
    }

    public void Remove(PlayerCore player)
    {
        switch (buffType)
        {
            case BuffType.Health:
                player.Health.AddMaxHealth(-healthModifier);
                break;
            case BuffType.Shooting:
                player.Shooting.AddDamage(-damageModifier);
                break;
                // 其他类型处理...
        }
    }
}