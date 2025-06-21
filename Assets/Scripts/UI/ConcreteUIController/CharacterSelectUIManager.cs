using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectUIManager : MonoBehaviour, IUIController
{
    public void OnEnterState()
    {
        UIManager.Instance.ShowUIForm<SelectCharacterPanel>();
    }

    public void OnExitState()
    {
        UIManager.Instance.HideUIForm<SelectCharacterPanel>();
    }
}
