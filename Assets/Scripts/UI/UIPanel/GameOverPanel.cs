using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : UIFormBase
{
    [SerializeField][Header("通过波次")] private TextMeshPro throughWaves;
    [SerializeField][Header("返回菜单按钮")] private Button backButton;

    protected override void Init()
    {
       backButton.onClick.AddListener(OnBackToMenu);
    }

    private void OnBackToMenu()
    {
        EventBus.TriggerGameStateChanged(GameState.Menu);
    }

}
