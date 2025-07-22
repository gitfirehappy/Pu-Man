using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIManager : MonoBehaviour, IUIController
{
    public void OnEnterState()
    {
        //显示Menu
        UIManager.Instance.ShowUIForm<MainMenuPanel>();
    }

    public void OnExitState()
    {
        //清理所有菜单UI
        UIManager.Instance.HideUIForm<MainMenuPanel>();
        UIManager.Instance.HideUIForm<SettingsPanel>();
        UIManager.Instance.HideUIForm<CreditsPanel>();
    }
}
