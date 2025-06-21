using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenuPanel : UIFormBase
{
    [SerializeField][Header("开始游戏按钮")] private Button startButton;
    [SerializeField][Header("游戏设置按钮")] private Button settingsButton;
    [SerializeField][Header("退出游戏按钮")] private Button exitButton;

    [SerializeField] private AudioMixer mixer;

    protected override void Init()
    {
        //初始化AudioMixer
        AudioVolumeService.Init(mixer);

        startButton.onClick.AddListener(OnStartClick);
        settingsButton.onClick.AddListener(OnSettingsClick);
        exitButton.onClick.AddListener(OnExitClick);
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    private void OnStartClick()
    {
        EventBus.TriggerGameStateChanged(GameState.Prepare);//切换游戏状态
    }

    /// <summary>
    /// 打开设置面板
    /// </summary>
    private void OnSettingsClick()
    {
        UIManager.Instance.ShowUIForm<SettingsPanel>();
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
