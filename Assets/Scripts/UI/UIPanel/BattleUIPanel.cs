using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIPanel : UIFormBase
{
    [Header("玩家战斗UI")]
    [SerializeField] private Image healthFill;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image playerPicture;

    [Header("当前技能冷却")]
    [SerializeField] private TextMeshProUGUI skillCooldownText;
    [SerializeField] private Image skillCooldownFill;

    //关卡倒计时，当前波次
    [SerializeField][Header("倒计时")] private TextMeshProUGUI timerText;
    [SerializeField][Header("当前波次")] private TextMeshProUGUI waveText;

    [Header("boss血量")]
    [SerializeField] private GameObject bossUIGroup; // 整个Boss UI的父对象（空物体）
    [SerializeField] private Image bossHealthFill;
    [SerializeField] private TextMeshProUGUI bossHealthText;

    private PlayerCore _player;
    private WaveTimer _waveTimer;
    private WaveCounter _waveCounter;
    private EnemyCore _currentBoss;
    private Coroutine _bossUICoroutine;

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

        // 初始隐藏Boss UI
        bossUIGroup.SetActive(false);

        // 注册事件
        EventBus.OnWaveChanged += OnWaveChanged;

        EventBus.OnBossWaveStarted += OnBossWaveStarted;
        EventBus.OnBossSpawned += OnBossSpawned;
        EventBus.OnBossWaveEnded += OnBossWaveEnded;
    }

    private void OnDestroy()
    {
        // 清理事件注册
        EventBus.OnWaveChanged -= OnWaveChanged;

        EventBus.OnBossWaveStarted -= OnBossWaveStarted;
        EventBus.OnBossSpawned -= OnBossSpawned;
        EventBus.OnBossWaveEnded -= OnBossWaveEnded;

        if (_bossUICoroutine != null)
            StopCoroutine(_bossUICoroutine);
    }

    private void Update()
    {
        // 实时更新UI
        UpdateTimerUI();
        UpdateHealthUI();
        UpdateSkillCooldownUI();

        // 实时更新Boss血量
        if (bossUIGroup.activeSelf && _currentBoss != null)
        {
            UpdateBossHealthUI();
        }
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

    #region BossUI

    /// <summary>
    /// 事件1：Boss波次开始时触发（此时Boss还未生成）
    /// </summary>
    private void OnBossWaveStarted()
    {
        // 显示Boss UI框架但无具体数据
        bossUIGroup.SetActive(true);
        bossHealthText.text = "BOSS INCOMING...";
        bossHealthFill.fillAmount = 1f;

        //TODO:关掉/调到最低背景音乐
    }

    /// <summary>
    /// 事件2：Boss实体生成完成后触发
    /// </summary>
    /// <param name="boss"></param>
    private void OnBossSpawned(EnemyCore boss)
    {
        _currentBoss = boss;

        // 更新UI显示真实血量
        UpdateBossHealthUI();

        // 可以在这里播放Boss登场特效
        //TODO:播放boss战专属背景音乐
        Debug.Log($"Boss {boss.name} 已绑定到UI");
    }


    /// <summary>
    /// 更新Boss血量UI
    /// </summary>
    private void UpdateBossHealthUI()
    {
        if (_currentBoss == null || _currentBoss.IsDead) return;

        bossHealthFill.fillAmount = _currentBoss.CurrentHealth / _currentBoss.MaxHealth;
        bossHealthText.text = $"{_currentBoss.CurrentHealth:F0}/{_currentBoss.MaxHealth:F0}";
    }

    /// <summary>
    /// Boss波结束（死亡或超时）
    /// </summary>
    private void OnBossWaveEnded()
    {
        if (_bossUICoroutine != null)
            StopCoroutine(_bossUICoroutine);

        _bossUICoroutine = StartCoroutine(HideBossUIDelayed(2f));
    }

    private IEnumerator HideBossUIDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        bossUIGroup.SetActive(false);
        _currentBoss = null;
    }

    #endregion
}
