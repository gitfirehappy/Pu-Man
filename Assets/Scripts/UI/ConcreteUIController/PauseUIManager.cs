using UnityEngine;

public class PauseUIManager : MonoBehaviour
{
    private bool isPaused = false;

    private void Awake()
    {
        EventBus.OnPauseUIRequested += ShowPauseUI;
        EventBus.OnResumeUIRequested += HidePauseUI;
    }

    private void OnDestroy()
    {
        EventBus.OnPauseUIRequested -= ShowPauseUI;
        EventBus.OnResumeUIRequested -= HidePauseUI;
    }

    private void Update()
    {
        // 检测ESC按键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (isPaused)
        {
            EventBus.TriggerGameResumed();
        }
        else
        {
            EventBus.TriggerGamePaused();
        }
        isPaused = !isPaused;
    }

    public void ShowPauseUI()
    {
        UIManager.Instance.ShowUIForm<PausePanel>();
    }

    public void HidePauseUI()
    {
        UIManager.Instance.HideUIForm<PausePanel>();
    }
}