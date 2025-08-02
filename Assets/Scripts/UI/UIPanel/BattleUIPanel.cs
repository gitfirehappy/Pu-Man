using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIPanel : UIFormBase
{
    [Header("玩家战斗UI")]
    public Image healthFill;
    public TextMeshProUGUI healthText;
    public Image playerPicture;

    [Header("当前技能冷却")]
    public TextMeshProUGUI skillCooldownText;
    public Image skillCooldownFill;

    //关卡倒计时，当前波次
    [Header("倒计时")] public TextMeshProUGUI timerText;
    [Header("当前波次")] public TextMeshProUGUI waveText;

    [Header("boss血量")]
    public GameObject bossUIGroup; // 整个Boss UI的父对象（空物体）
    public Image bossHealthFill;
    public TextMeshProUGUI bossHealthText;

    private PlayerCore _player;
    private WaveTimer _waveTimer;
    private WaveCounter _waveCounter;
    private EnemyCore _currentBoss;
    private Coroutine _bossUICoroutine;

    private void OnDestroy()
    {
        // 清理事件注册

        EnemyEvent.OnBossStateChanged -= HandleBossStateChange;

        if (_bossUICoroutine != null)
            StopCoroutine(_bossUICoroutine);
        _currentBoss = null;
    }

    private void Update()
    {
        // 实时更新UI
        UpdateTimerUI();
        UpdateHealthUI();

        // 实时更新Boss血量
        if (bossUIGroup.activeSelf && _currentBoss != null)
        {
            UpdateBossHealthUI();
        }
    }

    protected override void Init()
    {
        // 获取关键组件

        _waveTimer = WaveTimer.Instance;
        _waveCounter = WaveCounter.Instance;

        _player = PlayerManager.Instance.Player;
        UpdatePlayerPicture(); 
        UpdateHealthUI();      
        UpdateSkillCooldownUI();

        UpdateTimerUI();
        UpdateWaveUI();
        UpdateBossHealthUI();  
        bossUIGroup.SetActive(false);// 初始隐藏Boss UI

        // 注册事件
        EventQueueManager.AddStateEvent(GameState.Battle, () => 
        {
            OnWaveChanged(WaveCounter.Instance.CurrentWave);
        }, 12);

        EventQueueManager.AddStateEvent(GameState.GameOver, ForceHideBossUI, 1);

        EnemyEvent.OnBossStateChanged += HandleBossStateChange;
    }

    #region 更新玩家UI
    /// <summary>
    /// 更新玩家图片头像
    /// </summary>
    private void UpdatePlayerPicture()
    {
        if (_player == null || _player.PlayerData == null || playerPicture == null)
        {
            Debug.LogWarning("无法更新头像：Player或头像组件为空");
            playerPicture.sprite = null; // 清空无效头像
            return;
        }

        // 从PlayerData中获取头像并赋值
        playerPicture.sprite = _player.PlayerData.playerSprite;
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
    public void UpdateSkillCooldownUI()
    {
        if (_player == null || _waveCounter == null)
        {
            skillCooldownText.text = "-";
            return;
        }

        if (_player.Abilities == null)
        {
            skillCooldownText.text = "无技能";
            return;
        }

        if(_player.Abilities.CurrentAbilityData.isPassive == true)
        {
            skillCooldownText.text = "被动技能";
            return;
        }

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
    #endregion

    #region 更新关卡进度UI
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

        if (_waveCounter.IsInEndlessMode)
        {
            waveText.text = $"波次: {_waveCounter.CurrentWave}∞";
        }
        else
        {
            waveText.text = $"波次: {_waveCounter.CurrentWave}/{_waveCounter.TotalWaves}";
        }
    }
    #endregion

    /// <summary>
    /// 波次转换事件
    /// </summary>
    /// <param name="wave"></param>
    private void OnWaveChanged(int wave)
    {
        _player = PlayerManager.Instance.Player;
        UpdateWaveUI();
        UpdateSkillCooldownUI();
        UpdatePlayerPicture();
    }

    #region BossUI

    private void HandleBossStateChange(EnemyEvent.BossState state, EnemyCore boss)
    {
        switch (state)
        {
            case EnemyEvent.BossState.WaveStarted:
                OnBossWaveStarted();
                break;
            case EnemyEvent.BossState.Spawned:
                OnBossSpawned(boss);
                break;
            case EnemyEvent.BossState.WaveEnded:
                OnBossWaveEnded();
                break;
        }
    }

    /// <summary>
    /// 事件1：Boss波次开始时触发（此时Boss还未生成）
    /// </summary>
    private void OnBossWaveStarted()
    {
        // 显示Boss UI框架但无具体数据
        bossUIGroup.SetActive(true);
        bossHealthText.text = "BOSS INCOMING...";
        bossHealthFill.fillAmount = 1f;
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

        Debug.Log($"Boss {boss.name} 已绑定到UI");
    }


    /// <summary>
    /// 更新Boss血量UI
    /// </summary>
    private void UpdateBossHealthUI()
    {
        if (_currentBoss == null || _currentBoss.IsDead || bossHealthFill == null || bossHealthText == null)
            return;

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

        if (gameObject.activeInHierarchy && enabled)
        {
            // 仅在对象激活时启动协程
            _bossUICoroutine = StartCoroutine(HideBossUIDelayed(0.5f));
        }
        else
        {
            // 非激活状态下直接清理，不启动协程
            bossUIGroup?.SetActive(false);
            _currentBoss = null;
        }
    }

    private IEnumerator HideBossUIDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (bossUIGroup != null)
            bossUIGroup.SetActive(false);
        _currentBoss = null;
    }

    /// <summary>
    /// 强制关闭Boss UI（用于GameOver等特殊状态切换）
    /// </summary>
    private void ForceHideBossUI()
    {
        // 停止可能正在运行的协程
        if (_bossUICoroutine != null)
        {
            StopCoroutine(_bossUICoroutine);
            _bossUICoroutine = null;
        }

        // 直接隐藏UI并清空引用
        if (bossUIGroup != null)
            bossUIGroup.SetActive(false);
        _currentBoss = null;
    }

    #endregion
}
