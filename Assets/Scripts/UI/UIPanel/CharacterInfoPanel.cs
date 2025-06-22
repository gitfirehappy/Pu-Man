using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterInfoPanel : UIFormBase
{
    [Header("角色图片")] public Image characterPicture;
    [Header("角色名称")] public TextMeshPro characterName;

    [Header("血量上限")] public TextMeshPro maxHealth;
    [Header("护甲值")] public TextMeshPro armor;
    [Header("生命恢复")] public TextMeshPro healthRegen;
    [Header("闪避率")] public TextMeshPro dodgeChance;
    [Header("碰撞伤害")] public TextMeshPro collisionDamage;

    [Header("子弹伤害")] public TextMeshPro damage;
    [Header("射速")] public TextMeshPro fireRate;

    [Header("移动速度")] public TextMeshPro runSpeed;

    [Header("初始技能")] public TextMeshPro startingAbility;

    private PlayerSO currentCharacter;

    public void ShowCharacterData(PlayerSO data)
    {
        currentCharacter = data;
        characterPicture.sprite = data.playerSprite;
        characterName.text = data.playerName;

        maxHealth.text = $"血量上限: {data.healthConfig.maxHealth}";
        armor.text = $"护甲值: {data.healthConfig.armor}";
        healthRegen.text = $"生命恢复/秒:{data.healthConfig.healthRegen}";
        dodgeChance.text = $"闪避率:{data.healthConfig.dodgeChance}";
        collisionDamage.text = $"碰撞伤害:{data.healthConfig.collisionDamage}";

        damage.text = $"子弹伤害:{data.shootingConfig.damage}";
        fireRate.text = $"射速(数值越高越快):{data.shootingConfig.fireRate}";

        runSpeed.text = $"移动速度:{data.movementConfig.runSpeed}";

        startingAbility.text = $"初始技能:{data.description}";
    }

    public PlayerSO GetCurrentCharacter() => currentCharacter;


}
