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

    [Header("增长模式设置")]
    [SerializeField] private Toggle percentageToggle;
    [SerializeField] private Toggle linearToggle;

    [Header("按钮")]
    [SerializeField] private Button closeButton;

    protected override void Init()
    {
        gameObject.AddComponent<ButtonSoundInitializer>();

        // 初始化滑块值
        var config = EnemyManager.Instance.ScalingConfig;

        // 初始化增长模式切换
        percentageToggle.isOn = config.growthMode == GrowthMode.Percentage;
        linearToggle.isOn = config.growthMode == GrowthMode.Linear;

        percentageToggle.onValueChanged.AddListener(isOn => {
            if (isOn) OnGrowthModeChanged(GrowthMode.Percentage);
        });

        linearToggle.onValueChanged.AddListener(isOn => {
            if (isOn) OnGrowthModeChanged(GrowthMode.Linear);
        });

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

        // 初始化属性UI
        UpdateUIForCurrentMode();

        closeButton.onClick.AddListener(ClosePanel);
    }

    private void UpdateUIForCurrentMode()
    {
        var config = EnemyManager.Instance.ScalingConfig;
        bool isPercentage = config.growthMode == GrowthMode.Percentage;

        // 设置血量UI
        healthGrowthSlider.minValue = isPercentage ? 0f : 0f;
        healthGrowthSlider.maxValue = isPercentage ? 0.5f : 50f;
        healthGrowthSlider.value = isPercentage ? config.healthPerWave : config.healthLinearPerWave;
        healthGrowthSlider.onValueChanged.RemoveAllListeners();
        healthGrowthSlider.onValueChanged.AddListener(OnHealthGrowthChanged);
        UpdateHealthText();

        // 设置伤害UI
        damageGrowthSlider.minValue = isPercentage ? 0f : 0f;
        damageGrowthSlider.maxValue = isPercentage ? 0.3f : 30f;
        damageGrowthSlider.value = isPercentage ? config.damagePerWave : config.damageLinearPerWave;
        damageGrowthSlider.onValueChanged.RemoveAllListeners();
        damageGrowthSlider.onValueChanged.AddListener(OnDamageGrowthChanged);
        UpdateDamageText();
    }

    private void OnGrowthModeChanged(GrowthMode newMode)
    {
        var config = EnemyManager.Instance.ScalingConfig;
        config.growthMode = newMode;
        UpdateUIForCurrentMode();
        Debug.Log($"增长模式切换为: {newMode}");
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

    private void UpdateHealthText()
    {
        var config = EnemyManager.Instance.ScalingConfig;
        if (config.growthMode == GrowthMode.Percentage)
            healthValueText.text = $"+{config.healthPerWave * 100:F0}%/波";
        else
            healthValueText.text = $"+{config.healthLinearPerWave:F0}点/波";
    }

    private void UpdateDamageText()
    {
        var config = EnemyManager.Instance.ScalingConfig;
        if (config.growthMode == GrowthMode.Percentage)
            damageValueText.text = $"+{config.damagePerWave * 100:F0}%/波";
        else
            damageValueText.text = $"+{config.damageLinearPerWave:F0}点/波";
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

    private void OnHealthGrowthChanged(float value)
    {
        var config = EnemyManager.Instance.ScalingConfig;
        if (config.growthMode == GrowthMode.Percentage)
            config.healthPerWave = value;
        else
            config.healthLinearPerWave = value;

        UpdateHealthText();
    }

    private void OnDamageGrowthChanged(float value)
    {
        var config = EnemyManager.Instance.ScalingConfig;
        if (config.growthMode == GrowthMode.Percentage)
            config.damagePerWave = value;
        else
            config.damageLinearPerWave = value;

        UpdateDamageText();
    }

    private void ClosePanel()
    {
        UIManager.Instance.HideUIForm<DifficultySettingsPanel>();
    }
}
