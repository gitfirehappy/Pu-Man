using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : SingletonMono<PauseManager>
{
    private bool _isPaused;

    public bool IsPaused => _isPaused;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    protected override void Init()
    {
        // 注册暂停和恢复事件
        EventQueueManager.AddPauseEvent(OnGamePaused, 0);
        EventQueueManager.AddResumeEvent(OnGameResumed, 0);
    }

    public void TogglePause()
    {
        // 仅允许在战斗状态暂停/恢复
        if (LevelStatusPolicer.Instance.CurrentState != GameState.Battle)
            return;

        if (_isPaused)
        {
            EventBus.TriggerResumed();
        }
        else
        {
            EventBus.TriggerPause();
        }
    }

    private void OnGamePaused()
    {
        _isPaused = true;
        Time.timeScale = 0;
    }

    private void OnGameResumed()
    {
        _isPaused = false;
        Time.timeScale = 1;
    }

}
