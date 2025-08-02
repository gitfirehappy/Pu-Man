using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : UIFormBase
{
    [Header("音量控制")]
    [Header("主音量滑块")] public Slider masterSlider;
    [Header("音乐音量滑块")] public Slider musicSlider;
    [Header("音效音量滑块")] public Slider sfxSlider;

    [Header("主音量数值")] public TextMeshProUGUI masterValueText;
    [Header("音乐音量数值")] public TextMeshProUGUI musicValueText;
    [Header("音效音量数值")] public TextMeshProUGUI sfxValueText;

    [Header("关闭界面按钮")] public Button closeButton;
    [Header("重置记录按钮按钮")] public Button resetDataButton;
    [Header("打开难度控制面板")] public Button difficultyButton;

    [Header("无尽模式按钮")] public Button endlessModeButton;
    [Header("无尽模式状态文本")] public TextMeshProUGUI endlessModeStatusText;
    [Header("无尽模式提示文本")] public TextMeshProUGUI endlessModeHintText;

    protected override void Init()
    {
        gameObject.AddComponent<ButtonSoundInitializer>();

        // 初始化音量滑块数值显示
        InitSliderWithText(masterSlider, VolumeType.Master, masterValueText);
        InitSliderWithText(musicSlider, VolumeType.Music, musicValueText);
        InitSliderWithText(sfxSlider, VolumeType.SFX, sfxValueText);

        closeButton.onClick.AddListener(CloseSettingsPanel);
        resetDataButton.onClick.AddListener(ResetHighestWaveRecords);
        difficultyButton.onClick.AddListener(OpenDifficultySettings);

        // 初始化无尽模式开关
        InitEndlessModeButton();
    }

    /// <summary>
    /// 初始化带数值显示的音量滑块
    /// </summary>
    private void InitSliderWithText(Slider slider, VolumeType type, TextMeshProUGUI valueText)
    {
        // 自动绑定音量控制
        TryBindSlider(slider, type);

        // 设置滑块范围（0-1对应0%-100%）
        slider.minValue = 0f;
        slider.maxValue = 1f;

        // 添加数值变更监听
        slider.onValueChanged.AddListener((value) => {
            UpdateVolumeText(value, valueText, type);
        });

        // 初始化文本显示
        UpdateVolumeText(slider.value, valueText, type);
    }

    /// <summary>
    /// 更新音量百分比文本
    /// </summary>
    private void UpdateVolumeText(float value, TextMeshProUGUI textComponent, VolumeType type)
    {
        string prefix = type switch
        {
            VolumeType.Master => "主音量: ",
            VolumeType.Music => "音乐: ",
            VolumeType.SFX => "音效: ",
            _ => ""
        };

        // 转换为百分比整数 (0% - 100%)
        int percentage = Mathf.RoundToInt(value * 100);
        textComponent.text = $"{prefix}{percentage}%";
    }

    private void TryBindSlider(Slider slider, VolumeType type)
    {
        if (slider != null)
        {
            if (!slider.TryGetComponent<VolumeSliderBinder>(out var binder))
            {
                binder = slider.gameObject.AddComponent<VolumeSliderBinder>();
                binder.volumeType = type;
            }
        }
    }

    /// <summary>
    /// 关闭设置面板
    /// </summary>
    private void CloseSettingsPanel()
    {
        UIManager.Instance.HideUIForm<SettingsPanel>();
    }

    /// <summary>
    /// 重置最高记录
    /// </summary>
    private void ResetHighestWaveRecords()
    {
        DataManager.Instance.ResetAllRecords();
        Debug.Log("已重置所有角色记录");
    }

    private void OpenDifficultySettings()
    {
        UIManager.Instance.HideUIForm<SettingsPanel>();
        UIManager.Instance.ShowUIForm<DifficultySettingsPanel>();
    }

    /// <summary>
    /// 初始化无尽模式按钮
    /// </summary>
    private void InitEndlessModeButton()
    {
        // 设置初始状态
        UpdateEndlessModeUI();

        // 添加点击监听
        endlessModeButton.onClick.AddListener(() => {
            WaveCounter.Instance.EnableEndless = !WaveCounter.Instance.EnableEndless;
            UpdateEndlessModeUI();
        });
    }

    private void UpdateEndlessModeUI()
    {
        bool isEndlessEnabled = WaveCounter.Instance.EnableEndless;

        // 更新按钮颜色和文本
        endlessModeButton.image.color = isEndlessEnabled ? Color.green : Color.red;
        endlessModeStatusText.text = isEndlessEnabled ? "ON" : "OFF";
        endlessModeStatusText.color = isEndlessEnabled ? Color.blue : Color.black;

        endlessModeHintText.text = isEndlessEnabled
            ? "无尽模式已开启: 完成所有波次后将进入无尽挑战"
            : "无尽模式已关闭: 完成所有波次后游戏结束";
    }

}
