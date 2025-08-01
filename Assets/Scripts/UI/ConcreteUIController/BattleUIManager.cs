using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoBehaviour,IUIController
{
    public void OnEnterState()
    {
        // 确保先获取Player再显示UI，避免UI初始化时Player为空
        PlayerManager.Instance.GetPlayerAsync(player =>
        {
            if (player != null && player.PlayerData != null)
            {
                UIManager.Instance.ShowUIForm<BattleUIPanel>();
            }
            else
            {
                Debug.LogError("获取Player失败，无法显示战斗UI");
            }
        });
    }

    public void OnExitState()
    {
        UIManager.Instance.HideUIForm<BattleUIPanel>();
    }

}
