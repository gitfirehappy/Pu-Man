using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveCounter : SingletonMono<WaveCounter>
{
    [Header("时间设置")]
    [SerializeField] private float initialTime = 15f;
    [SerializeField] private float timeIncreasePerWave = 5f;

    [Header("通关后选择无尽波次时间")]
    [SerializeField] private float endlessModeTime = 45f;

    [Header("波次设置")]
    [SerializeField] private int totalWaves = 10;
    [SerializeField] private int bossInterval = 5; // 每N波出现Boss

    [Header("无尽模式")]
    [SerializeField] private bool enableEndless = false;

    private int _currentWave;
    private bool _isEndlessMode;
    private float _currentTimeLimit; // 当前波次时间上限

    #region 公共属性
    public bool IsInEndlessMode => _isEndlessMode;
    public int CurrentWave => _currentWave;
    public bool IsEndlessMode => _isEndlessMode;
    public float TimeIncrease => timeIncreasePerWave;
    public float CurrentTimeLimit => _currentTimeLimit;
    public int TotalWaves => totalWaves;
    #endregion

    protected override void Init()
    {
        ResetCounter();
        EventBus.OnBattleStart += StartNewWaveCycle;
        EventBus.OnBuffSelected += StartNewWaveCycle;
    }

    private void OnDestroy()
    {
        EventBus.OnBattleStart -= StartNewWaveCycle;
        EventBus.OnBuffSelected -= StartNewWaveCycle;
    }

    /// <summary>
    /// 重置波次
    /// </summary>
    public void ResetCounter()
    {
        _currentWave = 0;
        _isEndlessMode = false;
        _currentTimeLimit = initialTime; // 初始时间15秒
    }

    private void StartNewWaveCycle()
    {
        NextWave();
        WaveTimer.Instance.StartTimer(_currentTimeLimit);
    }

    public void NextWave()
    {
        _currentWave++;

        // 更新波次时间上限
        if (!_isEndlessMode)
        {
            _currentTimeLimit = Mathf.Min(60f, _currentTimeLimit + timeIncreasePerWave);

            //是否完成所有波次
            if (_currentWave >= totalWaves)
            {
                if (enableEndless)
                {
                    EnterEndlessMode();
                    return;
                }
                else
                {
                    EventBus.TriggerAllWavesCompleted();
                }
            }
        }
        EventBus.TriggerWaveChanged(_currentWave);

        //boss事件
        if (_currentWave % bossInterval == 0)
        {
            EventBus.TriggerBossWaveStarted();
        }
    }

    /// <summary>
    /// 进入无尽模式
    /// </summary>
    private void EnterEndlessMode()
    {
        _isEndlessMode = true;
        _currentTimeLimit = endlessModeTime; // 无尽模式固定45秒
        EventBus.TriggerEndlessModeActivated();
    }
}