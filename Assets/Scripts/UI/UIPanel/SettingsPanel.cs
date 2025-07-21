using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : UIFormBase
{

    [Header("音量控制")]
    [SerializeField][Header("主音量滑块")] private Slider masterSlider;
    [SerializeField][Header("音乐音量滑块")] private Slider musicSlider;
    [SerializeField][Header("音效音量滑块")] private Slider sfxSlider;

    [SerializeField][Header("主音量数值")] private TextMeshProUGUI masterValueText;
    [SerializeField][Header("音乐音量数值")] private TextMeshProUGUI musicValueText;
    [SerializeField][Header("音效音量数值")] private TextMeshProUGUI sfxValueText;

    [SerializeField][Header("关闭界面按钮")] private Button closeButton;
    [SerializeField][Header("重置记录按钮按钮")] private Button resetDataButton;
    [SerializeField][Header("打开难度控制面板")] private Button difficultyButton;

    protected override void Init()
    {
        // 初始化音量滑块数值显示
        InitSliderWithText(masterSlider, VolumeType.Master, masterValueText);
        InitSliderWithText(musicSlider, VolumeType.Music, musicValueText);
        InitSliderWithText(sfxSlider, VolumeType.SFX, sfxValueText);

        closeButton.onClick.AddListener(CloseSettingsPanel);
        resetDataButton.onClick.AddListener(ResetHighestWaveRecords);
        difficultyButton.onClick.AddListener(OpenDifficultySettings);
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
            UpdateVolumeText(value, valueText);
        });

        // 初始化文本显示
        UpdateVolumeText(slider.value, valueText);
    }

    /// <summary>
    /// 更新音量百分比文本
    /// </summary>
    private void UpdateVolumeText(float value, TextMeshProUGUI textComponent)
    {
        // 转换为百分比整数 (0% - 100%)
        int percentage = Mathf.RoundToInt(value * 100);
        textComponent.text = $"{percentage}%";
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
        CharacterDataManager.Instance.ResetAllRecords();
        Debug.Log("已重置所有角色记录");
    }

    private void OpenDifficultySettings()
    {
        UIManager.Instance.HideUIForm<SettingsPanel>();
        UIManager.Instance.ShowUIForm<DifficultySettingsPanel>();
    }

}
