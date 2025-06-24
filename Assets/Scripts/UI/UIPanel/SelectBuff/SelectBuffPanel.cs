using System;
using System.Collections;
using System.Collections.Generic;
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

    private List<BuffSO> allBuff;
    private BuffSO selectedBuff;

    protected override void Init()
    {
        LoadBuffAsync();  // 异步加载buff

        refreshBuffButton.onClick.AddListener(OnRefreshBuff);
        applyBuffButton.onClick.AddListener(OnApplyBuff);

        applyBuffButton.interactable = false; // 默认禁用开始按钮
    }

    /// <summary>
    /// 读取BuffSO数据
    /// </summary>
    private async void LoadBuffAsync()//这个要改
    {
        try
        {
            // 加载所有带 "BuffSO" Label 的 ScriptableObject
            var loadHandle = Addressables.LoadAssetsAsync<BuffSO>("BuffSO", null);
            await loadHandle.Task;

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                allBuff = new List<BuffSO>(loadHandle.Result);

                foreach (var character in allBuff)
                {
                    var cardObj = Instantiate(cardPrefab, cardContainer);
                    var card = cardObj.GetComponent<BuffPanel>();
                    card.Setup(character, OnBuffSelected);
                }
            }
            else
            {
                Debug.LogError("Buff加载失败！");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"加载Buff时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 选中Buff时
    /// </summary>
    /// <param name="buff"></param>
    private void OnBuffSelected(BuffSO buff)
    {
        selectedBuff = buff;
        applyBuffButton.interactable = true;
    }

    /// <summary>
    /// 应用Buff
    /// </summary>
    private void OnApplyBuff()
    {

    }

    /// <summary>
    /// 刷新Buff
    /// </summary>
    private void OnRefreshBuff()
    {
        //让Manager重新抽取BuffPanel
    }

}
