using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTimer : SingletonMono<WaveTimer>
{
    private float _currentTime;
    private float _timeLimit;

    public float CurrentTime => _currentTime;


    private void Update()
    {
        if (PauseManager.Instance.IsPaused) return;

        _currentTime -= Time.deltaTime;

        if (_currentTime <= 0)
        {
            CompleteWave();
        }
    }

    public void StartTimer(float timeLimit)
    {
        _timeLimit = timeLimit;
        _currentTime = _timeLimit;
    }

    private void CompleteWave()
    {
        _currentTime = 0;
        EventBus.TriggerChangeState(GameState.SelectBuff);
    }


    public void AddTime(float seconds)
    {
        _currentTime += seconds;

    }
}
