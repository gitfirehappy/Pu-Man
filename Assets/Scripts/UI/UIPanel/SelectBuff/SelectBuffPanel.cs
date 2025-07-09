using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class SelectBuffPanel : UIFormBase
{
    [Header("Buff卡片容器")]
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardPrefab;

    [SerializeField][Header("确定Buff按钮")] private Button applyBuffButton;
    [SerializeField][Header("刷新Buff按钮")] private Button refreshBuffButton;
    [SerializeField][Header("剩余刷新次数")] private TextMeshPro refreshCountText;
    [SerializeField][Header("剩余选择次数")] private TextMeshPro remainingChoicesText;

    private BuffSO selectedBuff;
    private System.Action<BuffSO> onApplyCallback;
    private System.Action onRefreshCallback;


    protected override void Init()
    {
        // 初始化按钮状态
        applyBuffButton.interactable = false;
        refreshBuffButton.interactable = false; // 初始状态由ShowBuffOptions设置
        refreshCountText.text = "刷新次数: 0";
        remainingChoicesText.text = "剩余选择: 0";
    }

    /// <summary>
    /// 显示Buff选项
    /// </summary>
    public void ShowBuffOptions(List<BuffSO> buffs,
        System.Action<BuffSO> onSelected,
        System.Action<BuffSO> onApply,
        System.Action onRefresh,
        int initialRefreshCount) // 添加初始刷新次数参数
    {
        ResetSelection();

        // 清空现有卡片
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        // 存储回调
        this.onApplyCallback = onApply;
        this.onRefreshCallback = onRefresh;

        // 创建新卡片
        foreach (var buff in buffs)
        {
            var cardObj = Instantiate(cardPrefab, cardContainer);
            var card = cardObj.GetComponent<BuffPanel>();
            card.Setup(buff, (selected) =>
                {
                    selectedBuff = selected;
                    onSelected?.Invoke(selected);
                    applyBuffButton.interactable = true;
                });
        }

        // 重置按钮状态
        applyBuffButton.interactable = false;
        applyBuffButton.onClick.RemoveAllListeners();
        applyBuffButton.onClick.AddListener(() => onApplyCallback?.Invoke(selectedBuff));

        refreshBuffButton.onClick.RemoveAllListeners();
        refreshBuffButton.onClick.AddListener(() => onRefreshCallback?.Invoke());

        // 更新刷新次数显示
        UpdateRefreshCount(initialRefreshCount);
    }

    /// <summary>
    /// 更新刷新次数显示
    /// </summary>
    public void UpdateRefreshCount(int count)
    {
        refreshCountText.text = $"刷新次数: {count}";
        refreshBuffButton.interactable = count > 0;
    }

    public void SetRemainingChoices(int count)
    {
        remainingChoicesText.text = $"剩余选择: {count}";  
    }

    public void ResetSelection()
    {
        selectedBuff = null;
        applyBuffButton.interactable = false;
    }
}
