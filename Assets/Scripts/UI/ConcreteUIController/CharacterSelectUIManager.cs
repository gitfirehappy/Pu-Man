using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CharacterSelectUIManager : MonoBehaviour, IUIController
{
    private List<PlayerSO> allCharacters;
    private SelectCharacterPanel selectCharacterPanel;

    [Header("玩家生成位置")]
    [SerializeField] private Transform spawnTransform;

    public async void OnEnterState()
    {
        try
        {
            // 显示选择面板
            UIManager.Instance.ShowUIForm<SelectCharacterPanel>();
            UIManager.Instance.ShowUIForm<CharacterInfoPanel>();

            selectCharacterPanel = UIManager.Instance.GetForm<SelectCharacterPanel>();

            if (selectCharacterPanel == null)
            {
                Debug.LogError("无法获取SelectCharacterPanel!");
                return;
            }

            // 等待PlayerSO加载完成
            while (!DataManager.Instance.IsPlayerDataLoaded)
            {
                await Task.Yield();
            }

            // 从DataManager获取角色数据
            allCharacters = DataManager.Instance.GetAllPlayerSOs();

            // 初始化面板
            if (allCharacters != null && allCharacters.Count > 0)
            {
                selectCharacterPanel.Initialize(allCharacters);
            }
            else
            {
                Debug.LogError("没有加载到任何角色数据!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"进入角色选择状态时出错: {e.Message}");
        }
    }

    public void OnExitState()
    {
        UIManager.Instance.HideUIForm<SelectCharacterPanel>();

        // 清除动态生成的角色卡片
        UIManager.Instance.ClearDynamicFormsInGroup(UIGroupID.CHARACTER_CARDS);
    }

    /// <summary>
    /// 生成玩家角色
    /// </summary>
    public void SpawnPlayer(PlayerSO characterData)
    {
        // 1. 检查预制体是否存在
        if (characterData.playerPrefab == null)
        {
            Debug.LogError($"角色 {characterData.playerName} 的预制体未设置!");
            return;
        }

        // 2. 销毁现有玩家
        var existingPlayer = PlayerManager.Instance.Player;
        if (existingPlayer != null)
        {
            Destroy(existingPlayer.gameObject);
        }

        // 3. 实例化预制体
        var playerObj = Instantiate(characterData.playerPrefab, spawnTransform.position, Quaternion.identity);

        // 4. 获取PlayerCore组件并设置数据
        var playerCore = playerObj.GetComponent<PlayerCore>();
        if (playerCore == null)
        {
            Debug.LogError("玩家预制体缺少PlayerCore组件!");
            Destroy(playerObj);
            return;
        }

        // 5. 设置SO数据并初始化
        playerCore.SetPlayerData(characterData);

        // 6. 设置PlayerManager
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.RegisterPlayer(playerCore);
        }
        else
        {
            Debug.LogWarning("PlayerManager实例未初始化");
        }
    }
}

