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

    [SerializeField][Header("生成位置")] private Vector3 spawnPosition;

    private PlayerSO selectedCharacter;
    private CharacterSelectUIManager uiManager;

    protected override void Init()
    {
        uiManager = GetComponentInParent<CharacterSelectUIManager>();

        startGameButton.onClick.AddListener(OnStartGame);
        backButton.onClick.AddListener(OnBackToMenu);

        startGameButton.interactable = false; // 默认禁用开始按钮
    }

    /// <summary>
    /// 初始化面板，创建角色卡片
    /// </summary>
    public void Initialize(List<PlayerSO> characters)
    {
        // 清空现有卡片
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        // 创建新卡片
        foreach (var character in characters)
        {
            var cardObj = Instantiate(cardPrefab, cardContainer);
            var card = cardObj.GetComponent<CharacterPanel>();
            card.Setup(character, OnCharacterSelected);
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

        // 通知UIManager生成玩家
        uiManager.SpawnPlayer(selectedCharacter, spawnPosition);

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
