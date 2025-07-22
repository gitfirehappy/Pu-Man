using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : SingletonMono<PauseManager>
{
    private bool _isPaused;

    public bool IsPaused => _isPaused;

    protected override void Init()
    {
        EventBus.OnGamePaused += OnGamePaused;
        EventBus.OnGameResumed += OnGameResumed;
    }

    private void OnGamePaused()
    {
        _isPaused = true;
        Time.timeScale = 0f;
    }

    private void OnGameResumed()
    {
        _isPaused = false;
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        EventBus.OnGamePaused -= OnGamePaused;
        EventBus.OnGameResumed -= OnGameResumed;
    }
}
