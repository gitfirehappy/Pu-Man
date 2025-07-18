using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySettingsPanel : UIFormBase
{
    [SerializeField] private Slider spawnIntervalSlider;
    [SerializeField] private Slider difficultySlider;
    [SerializeField] private Button closeButton;

    protected override void Init()
    {
        // 初始化滑块值
        var config = EnemyManager.Instance.ScalingConfig;
        spawnIntervalSlider.minValue = config.minSpawnInterval;
        spawnIntervalSlider.maxValue = config.initialSpawnInterval;
        spawnIntervalSlider.value = config.initialSpawnInterval;

        difficultySlider.minValue = 0.5f;
        difficultySlider.maxValue = 2f;
        difficultySlider.value = config.difficultyMultiplier;

        // 添加监听
        spawnIntervalSlider.onValueChanged.AddListener(OnSpawnIntervalChanged);
        difficultySlider.onValueChanged.AddListener(OnDifficultyChanged);
        closeButton.onClick.AddListener(ClosePanel);
    }

    private void OnSpawnIntervalChanged(float value)
    {
        var config = EnemyManager.Instance.ScalingConfig;
        config.initialSpawnInterval = value;
    }

    private void OnDifficultyChanged(float value)
    {
        EnemyManager.Instance.SetDifficultyMultiplier(value);
    }

    private void ClosePanel()
    {
        UIManager.Instance.HideUIForm<DifficultySettingsPanel>();
    }
}
