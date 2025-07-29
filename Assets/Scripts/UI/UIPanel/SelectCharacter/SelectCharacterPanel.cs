using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;

public class SelectCharacterPanel : UIFormBase
{
    [Header("角色卡片模板")]
    [SerializeField] private GameObject cardPrefab;

    [Header("卡片生成位置容器")]
    [SerializeField] private Transform[] cardPositions; // 在Inspector中拖入空物体

    [Header("选中时展示的信息")]
    [SerializeField] private CharacterInfoPanel characterInfoPanel;

    [Header("按钮")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button backButton;

    private PlayerSO selectedCharacter;
    private CharacterSelectUIManager characterSelectUIManager;

    protected override void Init()
    {
        gameObject.AddComponent<ButtonSoundInitializer>();

        characterSelectUIManager = GetComponentInParent<CharacterSelectUIManager>();

        startGameButton.onClick.AddListener(OnStartGame);
        backButton.onClick.AddListener(OnBackToMenu);

        startGameButton.interactable = false; // 默认禁用开始按钮
    }

    /// <summary>
    /// 初始化角色卡片
    /// </summary>
    public void Initialize(List<PlayerSO> characters)
    {
        if (cardPrefab == null || cardPositions == null || cardPositions.Length == 0)
        {
            Debug.LogError("卡片预制体或位置未设置!");
            return;
        }

        // 清除现有卡片
        UIManager.Instance.ClearDynamicFormsInGroup(UIGroupID.CHARACTER_CARDS);

        // 确保不超过定位点数量
        int cardCount = Mathf.Min(characters.Count, cardPositions.Length);

        for (int i = 0; i < cardCount; i++)
        {
            var card = UIManager.Instance.CreateDynamicForm<CharacterPanel>(
                cardPrefab,
                UIGroupID.CHARACTER_CARDS,
                cardPositions[i],
                onCreated: card => card.Setup(characters[i], OnCharacterSelected)
            );

            if (card != null)
            {
                card.Setup(characters[i], OnCharacterSelected);

                UIManager.Instance.ShowDynamicForm(card);
            }
            else
            {
                Debug.LogError("卡片预制体缺少CharacterPanel组件!");
            }
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

        // 通知Manager生成玩家
        characterSelectUIManager.SpawnPlayer(selectedCharacter);

        EventBus.TriggerChangeState(GameState.Battle);
    }

    /// <summary>
    /// 返回菜单
    /// </summary>
    private void OnBackToMenu()
    {
        EventBus.TriggerChangeState(GameState.Menu);
    }

}
