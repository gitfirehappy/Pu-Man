using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : UIFormBase
{
    [Header("音量控制")]
    [SerializeField][Header("主音量滑块")] private Slider masterSlider;
    [SerializeField][Header("音乐音量滑块")] private Slider musicSlider;
    [SerializeField][Header("音效音量滑块")] private Slider sfxSlider;

    [SerializeField][Header("关闭界面按钮")] private Button closeButton;

    [SerializeField][Header("重置记录按钮按钮")] private Button resetDataButton;

    //TODO:难度控制
    [SerializeField][Header("难度控制滑块")] private Slider difficultySlider;

    protected override void Init()
    {
        // 自动绑定音量滑块（无需手动赋值）
        TryBindSlider(masterSlider, VolumeType.Master);
        TryBindSlider(musicSlider, VolumeType.Music);
        TryBindSlider(sfxSlider, VolumeType.SFX);

        closeButton.onClick.AddListener(CloseSettingsPanel);
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
    public void ResetHighestWaveRecords()
    {
        CharacterDataManager.Instance.ResetAllRecords();
        Debug.Log("已重置所有角色记录");
    }

}
