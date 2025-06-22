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

    private List<PlayerSO> allCharacters;
    private PlayerSO selectedCharacter;

    protected override void Init()
    {
        LoadCharactersAsync();  // 异步加载角色

        startGameButton.onClick.AddListener(OnStartGame);
        backButton.onClick.AddListener(OnBackToMenu);

        startGameButton.interactable = false; // 默认禁用开始按钮
    }


    private async void LoadCharactersAsync()
    {
        try
        {
            // 加载所有带 "Player" Label 的 ScriptableObject
            var loadHandle = Addressables.LoadAssetsAsync<PlayerSO>("Player", null);
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

        EventBus.TriggerGameStateChanged(GameState.Battle);
    }

    /// <summary>
    /// 返回菜单
    /// </summary>
    private void OnBackToMenu()
    {
        EventBus.TriggerGameStateChanged(GameState.Menu);
    }

}
