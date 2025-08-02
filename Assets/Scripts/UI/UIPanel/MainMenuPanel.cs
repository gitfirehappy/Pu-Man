using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenuPanel : UIFormBase
{
    [Header("开始游戏按钮")] public Button startButton;
    [Header("游戏设置按钮")] public Button settingsButton;
    [Header("退出游戏按钮")] public Button exitButton;
    [Header("制作人员按钮")] public Button creditsButton;

    protected override void Init()
    {
        gameObject.AddComponent<ButtonSoundInitializer>();

        startButton.onClick.AddListener(OnStartClick);
        settingsButton.onClick.AddListener(OnSettingsClick);
        exitButton.onClick.AddListener(OnExitClick);
        creditsButton.onClick.AddListener(OnCreditsClick);
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    private void OnStartClick()
    {
        EventBus.TriggerChangeState(GameState.Prepare);//切换游戏状态
    }

    /// <summary>
    /// 打开设置面板
    /// </summary>
    private void OnSettingsClick()
    {
        UIManager.Instance.ShowUIForm<SettingsPanel>();
    }

    /// <summary>
    /// 打开制作人员面板
    /// </summary>
    private void OnCreditsClick()
    {
        UIManager.Instance.ShowUIForm<CreditsPanel>();
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    private void OnExitClick()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
