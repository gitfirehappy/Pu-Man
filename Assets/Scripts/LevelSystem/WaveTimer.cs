using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTimer : SingletonMono<WaveTimer>
{
    private float _currentTime;
    private float _timeLimit;
    private bool _isRunning;

    public float CurrentTime => _currentTime;
    public bool IsRunning => _isRunning;


    protected override void Init()
    {
        EventBus.OnGamePaused += PauseTimer;
        EventBus.OnGameResumed += ResumeTimer;

    }

    private void OnDestroy()
    {
        EventBus.OnGamePaused -= PauseTimer;
        EventBus.OnGameResumed -= ResumeTimer;

    }

    private void Update()
    {
        if (!_isRunning) return;

        _currentTime -= Time.deltaTime;


        if (_currentTime <= 0)
        {
            _currentTime = 0;
            _isRunning = false;
            EventBus.TriggerTimeOut();
        }
    }

    public void StartTimer(float timeLimit)
    {
        _timeLimit = timeLimit;
        _currentTime = _timeLimit;
        _isRunning = true;
    }

    private void CompleteWave()
    {
        // 计时结束，切换到选Buff状态
        EventBus.TriggerGameStateChanged(GameState.SelectBuff);
    }

    public void PauseTimer() => _isRunning = false;
    public void ResumeTimer() => _isRunning = true;

    public void AddTime(float seconds)
    {
        _currentTime += seconds;

    }
}
