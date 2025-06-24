using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBuffUIManager : MonoBehaviour, IUIController
{
    [Header("Buff配置")]
    [SerializeField][Header("默认n选1")] private int defaultBuffChoices = 3;
    [SerializeField][Header("默认刷新次数")] private int defaultRefreshCount = 1;

    private SelectBuffPanel selectBuffPanel;
    private int remainingRefreshCount;
    private List<BuffSO> currentBuffOptions = new List<BuffSO>();
    private Dictionary<Rarity, int> rarityWeights = new Dictionary<Rarity, int>()
    {
        { Rarity.Common, 70 },
        { Rarity.Rare, 20 },
        { Rarity.Epic, 8 },
        { Rarity.Legendary, 2 }
    };

    public void OnEnterState()
    {
        remainingRefreshCount = defaultRefreshCount;
        GenerateBuffOptions();
        UIManager.Instance.ShowUIForm<SelectBuffPanel>();
    }

    public void OnExitState()
    {
        UIManager.Instance.HideUIForm<SelectBuffPanel>();
        UIManager.Instance.HideUIForm<BuffPanel>();//关掉全部
    }

    /// <summary>
    /// 抽取buff
    /// </summary>
    private void GenerateBuffOptions()
    {
        
    }
}
