using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoBehaviour,IUIController
{
    public void OnEnterState()
    {
        UIManager.Instance.ShowUIForm<BattleUIPanel>();
    }

    public void OnExitState()
    {
        UIManager.Instance.HideUIForm<BattleUIPanel>();
    }

}
