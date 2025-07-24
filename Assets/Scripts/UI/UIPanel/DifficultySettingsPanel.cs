using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySettingsPanel : UIFormBase
{
    [Header("生成间隔设置")]
    [SerializeField] private Slider initialSpawnIntervalSlider; // 初始生成间隔滑条
    [SerializeField] private TextMeshProUGUI initialIntervalValueText;     // 初始间隔值显示文本
    [SerializeField] private Slider intervalReductionSlider;    // 每波减少间隔滑条
    [SerializeField] private TextMeshProUGUI reductionValueText;           // 减少值显示文本

    [Header("属性增长设置")]
    [SerializeField] private Slider healthGrowthSlider;   // 血量增长滑条
    [SerializeField] private TextMeshProUGUI healthValueText;
    [SerializeField] private Slider damageGrowthSlider;    // 伤害增长滑条
    [SerializeField] private TextMeshProUGUI damageValueText;

    [Header("按钮")]
    [SerializeField] private Button closeButton;

    protected override void Init()
    {
        gameObject.AddComponent<ButtonSoundInitializer>();

        // 初始化滑块值
        var config = EnemyManager.Instance.ScalingConfig;

        // 初始生成间隔设置 (1-5秒)
        initialSpawnIntervalSlider.minValue = 1f;
        initialSpawnIntervalSlider.maxValue = 5f;
        initialSpawnIntervalSlider.value = config.initialSpawnInterval;
        initialSpawnIntervalSlider.onValueChanged.AddListener(OnInitialIntervalChanged);
        UpdateInitialIntervalText(config.initialSpawnInterval);

        // 每波减少间隔设置 (0-0.2秒)
        intervalReductionSlider.minValue = 0f;
        intervalReductionSlider.maxValue = 0.2f;
        intervalReductionSlider.value = config.intervalReductionPerWave;
        intervalReductionSlider.onValueChanged.AddListener(OnReductionChanged);
        UpdateReductionText(config.intervalReductionPerWave);

        // 初始化血量增长滑条 (0% - 50%)
        healthGrowthSlider.minValue = 0f;
        healthGrowthSlider.maxValue = 0.5f;
        healthGrowthSlider.value = config.healthPerWave;
        healthGrowthSlider.onValueChanged.AddListener(OnHealthGrowthChanged);
        UpdateHealthText(config.healthPerWave);

        // 初始化伤害增长滑条 (0% - 30%)
        damageGrowthSlider.minValue = 0f;
        damageGrowthSlider.maxValue = 0.3f;
        damageGrowthSlider.value = config.damagePerWave;
        damageGrowthSlider.onValueChanged.AddListener(OnDamageGrowthChanged);
        UpdateDamageText(config.damagePerWave);

        closeButton.onClick.AddListener(ClosePanel);
    }

    /// <summary>
    /// 更新初始生成间隔文本
    /// </summary>
    private void UpdateInitialIntervalText(float value)
    {
        initialIntervalValueText.text = $"{value:F1}秒";
    }

    /// <summary>
    /// 更新减少间隔文本
    /// </summary>
    private void UpdateReductionText(float value)
    {
        reductionValueText.text = $"-{value:F2}秒/波";
    }

    /// <summary>
    /// 更新血量增长文本
    /// </summary>
    private void UpdateHealthText(float value)
    {
        healthValueText.text = $"+{value * 100:F0}%/波";
    }

    /// <summary>
    /// 更新伤害增长文本
    /// </summary>
    private void UpdateDamageText(float value)
    {
        damageValueText.text = $"+{value * 100:F0}%/波";
    }

    private void OnInitialIntervalChanged(float value)
    {
        EnemyManager.Instance.ScalingConfig.initialSpawnInterval = value;
        UpdateInitialIntervalText(value);
        Debug.Log($"初始生成间隔调整为: {value:F1}秒");
    }

    private void OnReductionChanged(float value)
    {
        EnemyManager.Instance.ScalingConfig.intervalReductionPerWave = value;
        UpdateReductionText(value);
        Debug.Log($"每波减少间隔调整为: {value:F2}秒");
    }

    /// <summary>
    /// 每波血量增长调整
    /// </summary>
    /// <param name="value"></param>
    private void OnHealthGrowthChanged(float value)
    {
        EnemyManager.Instance.ScalingConfig.healthPerWave = value;
        Debug.Log($"血量增长率调整为: {value * 100}%");
    }

    /// <summary>
    /// 每波伤害增长调整
    /// </summary>
    /// <param name="value"></param>
    private void OnDamageGrowthChanged(float value)
    {
        EnemyManager.Instance.ScalingConfig.damagePerWave = value;
        Debug.Log($"伤害增长率调整为: {value * 100}%");
    }

    private void ClosePanel()
    {
        UIManager.Instance.HideUIForm<DifficultySettingsPanel>();
    }
}
