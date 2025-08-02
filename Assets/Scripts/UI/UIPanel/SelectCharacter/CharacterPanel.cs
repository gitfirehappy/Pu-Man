using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterPanel : UIFormBase, IPointerClickHandler
{
    [Header("角色图片")] public Image characterPicture;
    [Header("角色名称")] public TextMeshProUGUI characterName;
    [Header("最高波次")] public TextMeshProUGUI historicalBest;

    private CanvasGroup canvasGroup;
    private bool isSelected;
    private const float SelectedAlpha = 0.5f; // 选中时的透明度
    private const float NormalAlpha = 1f; // 正常时的透明度

    private PlayerSO playerSO;
    private Action<PlayerSO> onClickCallback;

    protected override void Init()
    {
        // 添加CanvasGroup组件控制透明度
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = NormalAlpha;
    }

    public void Setup(PlayerSO data,Action<PlayerSO> onClick)
    {
        playerSO = data;
        onClickCallback = onClick;

        characterPicture.sprite = data.playerSprite;
        characterName.text = data.playerName;

        // 显示最高波次
        int highestWave = DataManager.Instance.GetHighestWave(data.playerType);
        historicalBest.text = $"最高记录: {highestWave}波";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClickCallback?.Invoke(playerSO);
        SetSelected(true); // 设置当前角色为选中状态

        // 设置同组内其他卡片的透明度
        UIManager.Instance.SetGroupPanelsAlpha(DynamicGroupID, this, NormalAlpha, SelectedAlpha);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        canvasGroup.alpha = isSelected ? SelectedAlpha : NormalAlpha;
    }
}
