using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectUIManager : MonoBehaviour, IUIController
{
    public void OnEnterState()
    {
        UIManager.Instance.ShowUIForm<SelectCharacterPanel>();
        // CharacterPanel和CharacterInfoPanel是SelectCharacterPanel子组件
    }

    public void OnExitState()
    {
        UIManager.Instance.HideUIForm<SelectCharacterPanel>();

    }
}
