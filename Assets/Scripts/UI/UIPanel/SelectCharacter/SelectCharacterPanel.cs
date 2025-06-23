using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;

public class SelectCharacterPanel : UIFormBase
{
    [Header("角色卡片容器")]
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardPrefab;

    [Header("选中时展示的信息")]
    [SerializeField] private CharacterInfoPanel characterInfoPanel;

    [SerializeField][Header("开始游戏按钮")] private Button startGameButton;
    [SerializeField][Header("返回菜单按钮")] private Button backButton;

    [Header("玩家生成设置")]
    [SerializeField] private Vector3 spawnPosition = Vector3.zero; // 默认在原点

    private List<PlayerSO> allCharacters;
    private PlayerSO selectedCharacter;

    protected override void Init()
    {
        LoadCharactersAsync();  // 异步加载角色

        startGameButton.onClick.AddListener(OnStartGame);
        backButton.onClick.AddListener(OnBackToMenu);

        startGameButton.interactable = false; // 默认禁用开始按钮
    }

    /// <summary>
    /// 读取PlayerSO数据
    /// </summary>
    private async void LoadCharactersAsync()
    {
        try
        {
            // 加载所有带 "PlayerSO" Label 的 ScriptableObject
            var loadHandle = Addressables.LoadAssetsAsync<PlayerSO>("PlayerSO", null);
            await loadHandle.Task;

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                allCharacters = new List<PlayerSO>(loadHandle.Result);

                foreach (var character in allCharacters)
                {
                    var cardObj = Instantiate(cardPrefab, cardContainer);
                    var card = cardObj.GetComponent<CharacterPanel>();
                    card.Setup(character, OnCharacterSelected);
                }
            }
            else
            {
                Debug.LogError("角色加载失败！");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"加载角色时出错: {e.Message}");
        }
    }


    /// <summary>
    /// 选中角色显示信息
    /// </summary>
    /// <param name="character"></param>
    private void OnCharacterSelected(PlayerSO character)
    {
        selectedCharacter = character;
        characterInfoPanel.ShowCharacterData(character);
        startGameButton.interactable = true;
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    private void OnStartGame()
    {
        if (selectedCharacter == null) return;

        // 生成玩家角色
        SpawnPlayer(selectedCharacter);

        EventBus.TriggerGameStateChanged(GameState.Battle);
    }

    /// <summary>
    /// 返回菜单
    /// </summary>
    private void OnBackToMenu()
    {
        EventBus.TriggerGameStateChanged(GameState.Menu);
    }

    /// <summary>
    /// 根据选择的角色SO生成玩家
    /// </summary>
    private void SpawnPlayer(PlayerSO characterData)
    {
        // 1. 检查预制体是否存在
        if (characterData.playerPrefab == null)
        {
            Debug.LogError($"角色 {characterData.playerName} 的预制体未设置!");
            return;
        }

        // 2. 销毁现有玩家
        var existingPlayer = FindObjectOfType<PlayerCore>();
        if (existingPlayer != null)
        {
            Destroy(existingPlayer.gameObject);
        }

        // 3. 实例化预制体
        var playerObj = Instantiate(characterData.playerPrefab, spawnPosition, Quaternion.identity);

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
    }

}
