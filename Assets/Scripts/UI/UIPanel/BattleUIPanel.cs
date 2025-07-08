using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIPanel : UIFormBase
{
    [Header("玩家战斗UI")]
    [SerializeField] private Image healthFill;
    [SerializeField] private TextMeshPro healthText;
    [SerializeField] private Image playerPicture;

    [Header("当前技能冷却")]
    [SerializeField] private TextMeshPro skillCooldownText;
    [SerializeField] private Image skillCooldownFill;

    //关卡倒计时，当前波次
    [SerializeField][Header("倒计时")] private TextMeshPro timerText;
    [SerializeField][Header("当前波次")] private TextMeshPro waveText;

    [Header("boss血量")]
    [SerializeField] private Image bossHealthFill;
    [SerializeField] private TextMeshPro bossHealthText;

    private PlayerCore _player;
    private WaveTimer _waveTimer;
    private WaveCounter _waveCounter;

    protected override void Init()
    {
        // 获取关键组件
        _player = FindObjectOfType<PlayerCore>();
        _waveTimer = WaveTimer.Instance;
        _waveCounter = WaveCounter.Instance;

        // 初始化UI
        UpdateHealthUI();
        UpdateSkillCooldownUI();
        UpdateTimerUI();
        UpdateWaveUI();
        UpdateBossHealthUI();

        // 注册事件
        EventBus.OnWaveChanged += OnWaveChanged;
        EventBus.OnBossWaveStarted += OnBossWaveStarted;
        EventBus.OnBossWaveEnded += OnBossWaveEnded;
    }

    private void Update()
    {
        // 实时更新UI
        UpdateTimerUI();
        UpdateHealthUI();
        UpdateSkillCooldownUI();
    }

    /// <summary>
    /// 更新玩家血量UI
    /// </summary>
    private void UpdateHealthUI()
    {
        if (_player == null || _player.Health == null) return;

        healthFill.fillAmount = _player.Health.CurrentHealth / _player.Health.MaxHealth;
        healthText.text = $"{_player.Health.CurrentHealth:F0}/{_player.Health.MaxHealth:F0}";
    }

    /// <summary>
    /// 更新玩家技能冷却
    /// </summary>
    private void UpdateSkillCooldownUI()
    {
        if (_player == null || _player.Abilities == null) return;

        int currentWave = _waveCounter.CurrentWave;
        int nextAvailable = _player.Abilities.nextAvailableWave;

        if (currentWave >= nextAvailable)
        {
            skillCooldownFill.fillAmount = 1f;
            skillCooldownText.text = "就绪";
        }
        else
        {
            float progress = 1f - (float)(nextAvailable - currentWave) / _player.Abilities.GetCooldownWaves();
            skillCooldownFill.fillAmount = progress;
            skillCooldownText.text = $"{nextAvailable - currentWave}波";
        }
    }

    /// <summary>
    /// 更新时间UI
    /// </summary>
    private void UpdateTimerUI()
    {
        if (_waveTimer == null) return;

        float currentTime = _waveTimer.CurrentTime;
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    /// <summary>
    /// 更新波次UI
    /// </summary>
    private void UpdateWaveUI()
    {
        if (_waveCounter == null) return;

        if (_waveCounter.IsEndlessMode)
        {
            waveText.text = $"波次: {_waveCounter.CurrentWave}∞";
        }
        else
        {
            waveText.text = $"波次: {_waveCounter.CurrentWave}/{_waveCounter.TotalWaves}";
        }
    }

    /// <summary>
    /// 波次转换事件
    /// </summary>
    /// <param name="wave"></param>
    private void OnWaveChanged(int wave)
    {
        UpdateWaveUI();
        UpdateSkillCooldownUI();
    }

    private void UpdateBossHealthUI()
    {

    }


    private void OnBossWaveStarted()
    {
        // TODO: 实际实现时替换为真正的Boss数据
        bossHealthFill.fillAmount = 1f;
    }

    private void OnBossWaveEnded()
    {

    }

    // 更新Boss血量 (由Boss系统调用)
    public void UpdateBossHealth(float currentHealth, float maxHealth)
    {
        bossHealthFill.fillAmount = currentHealth / maxHealth;
    }
}
