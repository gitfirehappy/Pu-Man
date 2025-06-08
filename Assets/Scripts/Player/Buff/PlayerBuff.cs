using System;
using UnityEditor.Timeline.Actions;
using UnityEngine;


public class PlayerBuff : IBuffEffect
{
    public void Apply(BuffSO buffData, PlayerCore player)
    {
        switch (buffData.buffID)
        {
            case BuffID.MaxHealthUp:
                player.Health.AddMaxHealth(buffData.healthModifier);
                break;

            case BuffID.ArmorUp:
                player.Health.AddArmor(buffData.armorModifier);
                break;

            case BuffID.CheatDeath:
                player.Health.SetCheatDeath(buffData.grantCheatDeath);
                break;

                // 其他效果...
        }
    }

    public void Remove(BuffSO buffData, PlayerCore player)
    {
        // 反向操作（如减少血量等）
    }
}