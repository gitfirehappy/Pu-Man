using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUIManager : MonoBehaviour, IUIController
{

    public void OnEnterState()
    {
        UIManager.Instance.ShowUIForm<GameOverPanel>();
    }

    public void OnExitState()
    {
        UIManager.Instance.HideUIForm<GameOverPanel>();
    }
}
