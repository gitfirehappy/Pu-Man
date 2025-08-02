using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : UIFormBase
{
    [Header("角色属性面板")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI regenText;
    public TextMeshProUGUI dodgeText;
    public TextMeshProUGUI collisionText;
    public TextMeshProUGUI bulletdamageText;
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI knockbackText;
    public TextMeshProUGUI projectileCountText;
    public TextMeshProUGUI projectileSizeText;
    public TextMeshProUGUI speedText;

    [Header("Buff统计")]
    public TextMeshProUGUI commonBuffCountText;
    public TextMeshProUGUI rareBuffCountText;
    public TextMeshProUGUI epicBuffCountText;
    public TextMeshProUGUI legendaryBuffCountText;

    [Header("按钮")]
    [Header("返回游戏按钮")] public Button resumeButton;
    [Header("结束游戏按钮")] public Button overGameButton;

    protected override void Init()
    {
        gameObject.AddComponent<ButtonSoundInitializer>();

        resumeButton.onClick.AddListener(OnResume);
        overGameButton.onClick.AddListener(OverGame);
    }

    #region 更新文本
    public void UpdatePlayerStats(string health, string armor, string regen, string dodge,
                                string collision, string bulletDamage, string fireRate,
                                string knockback, string projectileCount, string projectileSize,
                                string speed)
    {
        healthText.text = "生命值: " + health;
        armorText.text = "护甲值: " + armor;
        regenText.text = "生命恢复: " + regen;
        dodgeText.text = "闪避率: " + dodge;
        collisionText.text = "碰撞伤害: " + collision;
        bulletdamageText.text = "子弹伤害: " + bulletDamage;
        fireRateText.text = "射速: " + fireRate;
        knockbackText.text = "击退强度: " + knockback;
        projectileCountText.text = "子弹数量: " + projectileCount;
        projectileSizeText.text = "子弹大小: " + projectileSize;
        speedText.text = "移动速度: " + speed;
    }

    public void UpdateBuffCounts(int common, int rare, int epic, int legendary)
    {
        commonBuffCountText.text = "普通Buff: " + common.ToString();
        rareBuffCountText.text = "稀有Buff: " + rare.ToString();
        epicBuffCountText.text = "史诗Buff: " + epic.ToString();
        legendaryBuffCountText.text = "传说Buff: " + legendary.ToString();
    }
    #endregion

    private void OnResume()
    {
        // 直接通过PauseManager恢复游戏
        PauseManager.Instance.TogglePause();
    }

    private void OverGame()
    {
        // 自杀结束游戏
        PauseManager.Instance.TogglePause();
        PlayerManager.Instance.Player.Health.TakeDamage(float.MaxValue);
    }

}
