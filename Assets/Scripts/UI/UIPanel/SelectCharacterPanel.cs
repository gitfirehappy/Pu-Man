using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacterPanel : UIFormBase
{
    [Header("角色卡片容器")]
    //[SerializeField] private Transform cardContainer;
    [Header("选中时展示的信息")]
    [SerializeField] private TextMeshPro characterStats;


    [SerializeField][Header("开始游戏按钮")] private Button startGameButton;
    [SerializeField][Header("返回菜单按钮")] private Button backButton;

    protected override void Init()
    {
        //角色面板和信息

        startGameButton.onClick.AddListener(OnStartGame);
        backButton.onClick.AddListener(OnBackToMenu);

        startGameButton.interactable = false; // 默认禁用开始按钮
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    private void OnStartGame()
    {
        EventBus.TriggerGameStateChanged(GameState.Battle);
    }

    /// <summary>
    /// 返回菜单
    /// </summary>
    private void OnBackToMenu()
    {
        EventBus.TriggerGameStateChanged(GameState.Menu);
    }

}
