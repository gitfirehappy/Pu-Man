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
    [SerializeField][Header("默认n选1")] private int defaultBuffChoices = 3;
    [SerializeField][Header("默认刷新次数")] private int defaultRefreshCount = 1;

    private SelectBuffPanel selectBuffPanel;
    private int remainingRefreshCount;
    private List<BuffSO> allBuffs = new List<BuffSO>();
    private List<BuffSO> currentBuffOptions = new List<BuffSO>();

    private Dictionary<Rarity, int> rarityWeights = new Dictionary<Rarity, int>()
    {
        { Rarity.Common, 70 },
        { Rarity.Rare, 20 },
        { Rarity.Epic, 8 },
        { Rarity.Legendary, 2 }
    };

    public async void OnEnterState()
    {
        remainingRefreshCount = defaultRefreshCount;

        // 加载所有Buff数据
        await LoadAllBuffsAsync();

        // 显示选择面板
        UIManager.Instance.ShowUIForm<SelectBuffPanel>();
        selectBuffPanel = UIManager.Instance.GetForm<SelectBuffPanel>();

        // 生成初始Buff选项
        GenerateBuffOptions();
    }

    public void OnExitState()
    {
        UIManager.Instance.HideUIForm<SelectBuffPanel>();
    }


    /// <summary>
    /// 异步加载所有Buff数据
    /// </summary>
    private async Task LoadAllBuffsAsync()
    {
        try
        {
            var loadHandle = Addressables.LoadAssetsAsync<BuffSO>("BuffSO", null);
            await loadHandle.Task;

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                allBuffs = new List<BuffSO>(loadHandle.Result);
            }
            else
            {
                Debug.LogError("Buff加载失败！");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载Buff时出错: {e.Message}");
        }
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
            selectBuffPanel.ShowBuffOptions(currentBuffOptions, 
                OnBuffSelected, OnApplyBuff, OnRefreshBuff,remainingRefreshCount);
        }
    }

    /// <summary>
    /// 根据权重随机获取一个Buff
    /// </summary>
    private BuffSO GetRandomBuffByRarity()
    {
        if (allBuffs.Count == 0) return null;

        // 根据权重随机选择稀有度
        int totalWeight = 0;
        foreach (var weight in rarityWeights.Values)
        {
            totalWeight += weight;
        }

        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        Rarity selectedRarity = Rarity.Common;

        foreach (var kvp in rarityWeights)
        {
            if (randomValue < kvp.Value)
            {
                selectedRarity = kvp.Key;
                break;
            }
            randomValue -= kvp.Value;
        }

        // 从对应稀有度的Buff中随机选择一个
        var eligibleBuffs = allBuffs.FindAll(b => b.rarity == selectedRarity);
        if (eligibleBuffs.Count == 0) return null;

        return eligibleBuffs[UnityEngine.Random.Range(0, eligibleBuffs.Count)];
    }

    /// <summary>
    /// 选中Buff回调
    /// </summary>
    private void OnBuffSelected(BuffSO buff)
    {
        // 可以在这里处理选中逻辑，如高亮显示等
    }

    /// <summary>
    /// 应用Buff回调
    /// </summary>
    private void OnApplyBuff(BuffSO selectedBuff)
    {
        if (selectedBuff == null) return;

        // 应用Buff效果
        var player = FindObjectOfType<PlayerCore>();
        if (player != null)
        {
            var buffProcessor = player.GetComponent<PlayerBuff>();
            if (buffProcessor != null)
            {
                buffProcessor.Apply(selectedBuff, player);
            }
        }

        // 返回游戏
        EventBus.TriggerGameStateChanged(GameState.Battle);
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
}
