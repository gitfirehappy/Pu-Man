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

    private void CompleteWave()
    {
        _currentTime = 0;
        EventBus.TriggerChangeState(GameState.SelectBuff);
    }

}
