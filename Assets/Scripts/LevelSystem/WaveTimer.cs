using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTimer : SingletonMono<WaveTimer>
{
    private float _currentTime;
    private float _timeLimit;
    private bool _isPaused;
    private Coroutine _timerCoroutine;

    public float CurrentTime => _currentTime;

    protected override void Init()
    {
        EventQueueManager.AddStateEvent(GameState.Battle, () => 
        {
            if (WaveCounter.Instance != null)
            {
                StartTimer(WaveCounter.Instance.CurrentTimeLimit);
            }
        }, 3);
        EventQueueManager.AddPauseEvent(OnPaused, 1); // 注册暂停事件
        EventQueueManager.AddResumeEvent(OnResumed, 1); // 注册恢复事件

        EventQueueManager.AddStateEvent(GameState.GameOver, StopTimer, 1);

        Debug.Log("WaveTimer初始化完成");
    }

    private void OnPaused() => _isPaused = true;
    private void OnResumed() => _isPaused = false;


    public void StartTimer(float timeLimit)
    {
        _timeLimit = timeLimit;
        _currentTime = _timeLimit;
        _timerCoroutine = StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        while (_currentTime > 0)
        {
            if (!_isPaused) // 检测事件更新的暂停状态
            {
                _currentTime -= Time.deltaTime;
            }
            yield return null;
        }
        CompleteWave();
    }

    public void StopTimer()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
            _currentTime = 0;
            Debug.Log("计时器已在游戏结束时停止");
        }
    }

    private void CompleteWave()
    {
        _currentTime = 0;
        var waveCounter = WaveCounter.Instance;
        // 判断是否是最后一波且未启用无尽模式
        if (waveCounter.CurrentWave >= waveCounter.TotalWaves && !waveCounter.EnableEndless && !waveCounter.IsInEndlessMode)
        {
            if (PlayerManager.Instance.Player != null)
            {
                Destroy(PlayerManager.Instance.Player.gameObject);
                PlayerManager.Instance.ClearPlayer(PlayerManager.Instance.Player);
            }

            // 最后一波结束，触发游戏结束
            EventBus.TriggerChangeState(GameState.GameOver);
        }
        else
        {
            // 非最后一波，继续选择buff
            EventBus.TriggerChangeState(GameState.SelectBuff);
        }
    }

}
