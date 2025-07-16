using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : UIFormBase
{
    [Header("角色属性面板")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI regenText;
    [SerializeField] private TextMeshProUGUI dodgeText;
    [SerializeField] private TextMeshProUGUI collisionText;
    [SerializeField] private TextMeshProUGUI bulletdamageText;
    [SerializeField] private TextMeshProUGUI fireRateText;
    [SerializeField] private TextMeshProUGUI knockbackText;
    [SerializeField] private TextMeshProUGUI projectileCountText;
    [SerializeField] private TextMeshProUGUI projectileSizeText;
    [SerializeField] private TextMeshProUGUI speedText;

    [Header("Buff统计")]
    [SerializeField] private TextMeshProUGUI commonBuffCountText;
    [SerializeField] private TextMeshProUGUI rareBuffCountText;
    [SerializeField] private TextMeshProUGUI epicBuffCountText;
    [SerializeField] private TextMeshProUGUI legendaryBuffCountText;

    [Header("按钮")]
    [SerializeField][Header("返回游戏按钮")] private Button resumeButton;
    [SerializeField][Header("结束游戏按钮")] private Button overGameButton;

    protected override void Init()
    {
        resumeButton.onClick.AddListener(OnResume);
        overGameButton.onClick.AddListener(OverGame);
    }

    private void OnResume()
    {
        EventBus.TriggerGameResumed();
    }

    private void OverGame()
    {
        // 自杀结束游戏
        EventBus.TriggerGameResumed();
        //playerCore.Health.TakeDamage(float.MaxValue);
    }

}
