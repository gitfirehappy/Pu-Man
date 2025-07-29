using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SelectBuffUIManager : MonoBehaviour, IUIController
{
    [Header("Buff配置")]
    [SerializeField][Header("默认n个选项")] private int defaultBuffChoices = 3;
    [SerializeField][Header("默认选择次数")] private int defaultBuffCanChoose = 1;
    private int remainingChoices;//剩余选择次数
    private int extraBuffChoices = 0; // 存储额外Buff选择次数

    [SerializeField][Header("默认刷新次数")] private int defaultRefreshCount = 1;
    private int remainingRefreshCount;

    [Header("稀有度权重 (总和自动调整为1)")]
    [Range(0f, 1f)][SerializeField] private float commonWeight = 0.7f;
    [Range(0f, 1f)][SerializeField] private float rareWeight = 0.2f;
    [Range(0f, 1f)][SerializeField] private float epicWeight = 0.08f;
    [Range(0f, 1f)][SerializeField] private float legendaryWeight = 0.02f;

    private SelectBuffPanel selectBuffPanel;
    private BuffCardSpawner buffCardSpawner;
    private BuffSO currentlySelectedBuff;

    private List<BuffSO> currentBuffOptions = new List<BuffSO>();
    private Dictionary<Rarity, float> rarityWeights;

    public async void OnEnterState()
    {
        // 检查是否是无尽模式
        if (WaveCounter.Instance != null && WaveCounter.Instance.IsInEndlessMode)
        {
            // 无尽模式直接返回战斗状态
            EventBus.TriggerChangeState(GameState.Battle);
            return;
        }

        remainingRefreshCount = defaultRefreshCount;
        InitializeWeights();//初始化权重

        // 等待Buff数据加载完成
        while (!DataManager.Instance.IsBuffDataLoaded)
        {
            await Task.Yield();
        }

        // 显示选择面板
        UIManager.Instance.ShowUIForm<SelectBuffPanel>();
        selectBuffPanel = UIManager.Instance.GetForm<SelectBuffPanel>();

        // 计算总Buff选择次数 = 默认 + 额外
        remainingChoices = defaultBuffCanChoose + extraBuffChoices;
        extraBuffChoices = 0; // 使用后重置

        // 生成初始Buff选项
        GenerateBuffOptions();
    }

    public void OnExitState()
    {
        UIManager.Instance.HideUIForm<SelectBuffPanel>();

        buffCardSpawner.ClearCards();//清理动态卡片
    }

    /// <summary>
    /// 初始化Buff权重
    /// </summary>
    private void InitializeWeights()
    {
        //计算总权重
        float totalWeight = commonWeight + rareWeight + epicWeight + legendaryWeight;

        if (totalWeight > 0)
        {
            commonWeight /= totalWeight;
            rareWeight /= totalWeight;
            epicWeight /= totalWeight;
            legendaryWeight /= totalWeight;
        }
        else
        {
            commonWeight = 0.7f;
            rareWeight = 0.2f;
            epicWeight = 0.08f;
            legendaryWeight = 0.02f;
        }

        rarityWeights = new Dictionary<Rarity, float>()
        {
            { Rarity.Common, commonWeight },
            { Rarity.Rare, rareWeight },
            { Rarity.Epic, epicWeight },
            { Rarity.Legendary, legendaryWeight }
        };
    }

    /// <summary>
    /// 根据权重随机抽取Buff
    /// </summary>
    private void GenerateBuffOptions()
    {
        currentBuffOptions.Clear();

        // 随机抽取指定数量的Buff
        for (int i = 0; i < defaultBuffChoices; i++)
        {
            var buff = GetRandomBuffByRarity();
            if (buff != null)
            {
                currentBuffOptions.Add(buff);
            }
        }

        // 更新UI显示
        if (selectBuffPanel != null)
        {
            selectBuffPanel.ShowBuffOptions(
                 currentBuffOptions,
                 OnBuffSelected,
                 OnApplyBuff,
                 OnRefreshBuff,
                 remainingRefreshCount
             );
        }
    }

    /// <summary>
    /// 根据权重随机获取一个Buff
    /// </summary>
    private BuffSO GetRandomBuffByRarity()
    {
        // 根据权重随机选择稀有度
        float randomValue = UnityEngine.Random.Range(0f, 1f);
        Rarity selectedRarity = Rarity.Common;
        float cumulativeWeight = 0f;

        foreach (var kvp in rarityWeights)
        {
            cumulativeWeight += kvp.Value;
            if (randomValue <= cumulativeWeight)
            {
                selectedRarity = kvp.Key;
                break;
            }
        }

        // 从DataManager获取对应稀有度的Buff列表
        var eligibleBuffs = DataManager.Instance.GetBuffsByRarity(selectedRarity);
        if (eligibleBuffs.Count == 0) return null;

        return eligibleBuffs[UnityEngine.Random.Range(0, eligibleBuffs.Count)];
    }

    /// <summary>
    /// 选中Buff回调
    /// </summary>
    private void OnBuffSelected(BuffSO buff)
    {
        // 取消之前选中的高亮
        if (currentlySelectedBuff != null)
        {
            // 通过缓存或查找方式获取之前的BuffPanel
            GetBuffPanelFor(currentlySelectedBuff)?.SetHighlight(false);
        }

        // 设置新选中
        currentlySelectedBuff = buff;
        GetBuffPanelFor(buff)?.SetHighlight(true);
    }

    private BuffPanel GetBuffPanelFor(BuffSO buff)
    {
        if (selectBuffPanel == null || buff == null)
            return null;

        // 调用SelectBuffPanel的GetBuffPanel方法
        return selectBuffPanel.GetBuffPanel(buff);
    }

    /// <summary>
    /// 应用Buff回调
    /// </summary>
    private void OnApplyBuff(BuffSO selectedBuff)
    {
        if (selectedBuff == null || remainingChoices <= 0)
        {
            // 添加边界情况处理
            if (remainingChoices <= 0)
            {
                EventBus.TriggerChangeState(GameState.Battle);
            }
            return;
        }

        Debug.Log($"应用Buff: {selectedBuff.buffID}, 剩余次数: {remainingChoices - 1}");

        // 应用Buff效果
        BuffManager.Instance.ApplyBuff(selectedBuff);
        remainingChoices--;// 减少选择次数

        selectBuffPanel.SetRemainingChoices(remainingChoices);

        if (remainingChoices > 0)
        {
            // 还有选择机会，重新生成选项
            selectBuffPanel.ResetSelection();
            GenerateBuffOptions();
        }
        else
        {
            // 选择次数用完，返回游戏
            EventBus.TriggerChangeState(GameState.Battle);
        }
    }

    /// <summary>
    /// 刷新Buff回调
    /// </summary>
    private void OnRefreshBuff()
    {
        if (remainingRefreshCount <= 0) return;

        remainingRefreshCount--;
        GenerateBuffOptions();

        // 更新UI刷新次数显示
        if (selectBuffPanel != null)
        {
            selectBuffPanel.UpdateRefreshCount(remainingRefreshCount);
        }
    }

    /// <summary>
    /// 增加刷新次数
    /// </summary>
    /// <param name="count">增加的次数</param>
    public void AddRefreshCount(int count)
    {
        remainingRefreshCount += count;

        // 更新UI显示
        if (selectBuffPanel != null)
        {
            selectBuffPanel.UpdateRefreshCount(remainingRefreshCount);
        }
    }

    /// <summary>
    /// 增加额外的Buff选择次数
    /// </summary>
    /// <param name="count">增加的次数</param>
    public void AddExtraBuffChoices(int count)
    {
        extraBuffChoices += count;
        Debug.Log($"已增加{count}次额外Buff选择机会，将在下次选择时生效");
    }
}