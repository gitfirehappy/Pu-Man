using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffPanel : UIFormBase, IPointerClickHandler
{
    [Header("Buff图片")] public Image buffPicture;
    [Header("Buff名称")] public TextMeshProUGUI buffName;
    [Header("Buff描述")] public TextMeshProUGUI buffDiscription;
    [Header("Buff稀有度")] public TextMeshProUGUI buffRarityText;

    [Header("高亮效果")]
    public Image glowBorder; // 动态颜色边框
    public ParticleSystem rarityParticles; // 稀有度粒子效果

    [Header("稀有度颜色")]
    public Color commonColor;
    public Color rareColor;
    public Color epicColor;
    public Color legendaryColor;

    private CanvasGroup canvasGroup;
    private bool isSelected;
    private const float SelectedAlpha = 0.5f; // 选中时的透明度
    private const float NormalAlpha = 1f; // 正常时的透明度

    private BuffSO currentBuff;
    private System.Action<BuffSO> onClickCallback;

    public BuffSO CurrentBuff => currentBuff;

    protected override void Init()
    {
        // 添加CanvasGroup组件控制透明度
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = NormalAlpha;

        // 初始设置glowborder为常开
        if (glowBorder != null)
        {
            glowBorder.gameObject.SetActive(true);
        }
    }

    public void Setup(BuffSO buff, System.Action<BuffSO> onSelectedCallback)
    {
        currentBuff = buff;
        onClickCallback = onSelectedCallback;

        // 设置UI内容
        buffPicture.sprite = buff.buffPicture;
        buffName.text = buff.buffID.ToString();
        buffDiscription.text = buff.Description;

        // 设置稀有度文字和颜色
        buffRarityText.text = GetRarityName(buff.rarity);
        buffRarityText.color = GetRarityColor(buff.rarity);

        // 设置glowBorder颜色
        if (glowBorder != null)
        {
            glowBorder.color = GetRarityColor(buff.rarity);
        }

        // 设置初始粒子状态
        if (rarityParticles != null)
        {
            rarityParticles.Play();
            var mainModule = rarityParticles.main;
            mainModule.startColor = GetRarityColor(buff.rarity);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClickCallback?.Invoke(currentBuff);
        SetSelected(true); // 设置当前Buff为选中状态

        // 设置同组内其他卡片的透明度
        UIManager.Instance.SetGroupPanelsAlpha(DynamicGroupID, this, NormalAlpha, SelectedAlpha);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        canvasGroup.alpha = isSelected ? SelectedAlpha : NormalAlpha;
    }

    private Color GetRarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return commonColor;
            case Rarity.Rare: return rareColor;
            case Rarity.Epic: return epicColor;
            case Rarity.Legendary: return legendaryColor;
            default: return commonColor;
        }
    }

    private string GetRarityName(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return "普通";
            case Rarity.Rare: return "稀有";
            case Rarity.Epic: return "史诗";
            case Rarity.Legendary: return "传说";
            default: return "未知";
        }
    }
}
