using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterInfoPanel : UIFormBase
{
    [Header("角色图片")] public Image characterPicture;
    [Header("角色名称")] public TextMeshProUGUI characterName;

    [Header("血量上限")] public TextMeshProUGUI maxHealth;
    [Header("护甲值")] public TextMeshProUGUI armor;
    [Header("生命恢复")] public TextMeshProUGUI healthRegen;
    [Header("闪避率")] public TextMeshProUGUI dodgeChance;
    [Header("碰撞伤害")] public TextMeshProUGUI collisionDamage;

    [Header("子弹伤害")] public TextMeshProUGUI damage;
    [Header("射速")] public TextMeshProUGUI fireRate;

    [Header("移动速度")] public TextMeshProUGUI runSpeed;

    [Header("初始技能")] public TextMeshProUGUI startingAbility;

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
