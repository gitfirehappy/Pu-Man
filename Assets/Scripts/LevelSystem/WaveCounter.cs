using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveCounter : SingletonMono<WaveCounter>
{
    [Header("时间设置")]
    [Tooltip("初始时间")][SerializeField] private float initialTime = 15f;
    [Tooltip("每波增长时间")][SerializeField] private float timeIncreasePerWave = 5f;

    [Header("通关后无尽波次时间")]
    [SerializeField] private float endlessModeTime = 45f;

    [Header("波次设置")]
    [Tooltip("总波次")][SerializeField] private int totalWaves = 10;
    [Tooltip("每x波出现Boss")][SerializeField] private int bossInterval = 5; // 每N波出现Boss

    [Header("当前状态")]
    [SerializeField] private bool _isInEndlessMode;
    [SerializeField] private int _currentWave;
    [SerializeField] private float _currentTimeLimit; // 当前波次时间上限

    #region 公共属性
    public bool EnableEndless { get; set; } = false; // 现在可以通过设置面板修改
    public bool IsInEndlessMode => _isInEndlessMode;

    public int CurrentWave => _currentWave;
    public float TimeIncrease => timeIncreasePerWave;
    public float CurrentTimeLimit => _currentTimeLimit;
    public int TotalWaves => totalWaves;

    public int BossInterval => bossInterval;
    #endregion

    protected override void Init()
    {
        EventQueueManager.AddStateEvent(GameState.Menu, ResetCounter, 2);
        EventQueueManager.AddStateEvent(GameState.Battle, StartNewWaveCycle, 2);
        Debug.Log("WaveCounter初始化完成");
    }

    /// <summary>
    /// 重置波次
    /// </summary>
    public void ResetCounter()
    {
        _currentWave = 0;
        _isInEndlessMode = false;
        _currentTimeLimit = initialTime; // 初始时间15秒
    }

    private void StartNewWaveCycle()
    {
        NextWave();
    }

    private void NextWave()
    {
        _currentWave++;
        Debug.Log($"当前波次：{_currentWave}");

        // 更新波次时间上限
        if (!IsInEndlessMode)
        {
            _currentTimeLimit = Mathf.Min(60f, _currentTimeLimit + timeIncreasePerWave);

            //是否完成所有波次
            if (_currentWave >= totalWaves)
            {
                if (EnableEndless)
                {
                    EnterEndlessMode();
                    return;
                }
            }
        }

        //boss事件
        if (_currentWave % bossInterval == 0)
        {
            EnemyEvent.TriggerBossState(EnemyEvent.BossState.WaveStarted);
        }

        //重置玩家状态
        if (PlayerManager.Instance.Player != null)
        {
            PlayerManager.Instance.Player.ResetState();
        }
    }

    /// <summary>
    /// 进入无尽模式
    /// </summary>
    private void EnterEndlessMode()
    {
        _isInEndlessMode = true;
        _currentTimeLimit = endlessModeTime; // 无尽模式固定45秒
    }
}