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
    [SerializeField][Header("Buff图片")] private Image buffPicture;
    [SerializeField][Header("Buff名称")] private TextMeshProUGUI buffName;
    [SerializeField][Header("Buff描述")] private TextMeshProUGUI buffDiscription;
    [Header("高亮效果")]
    [SerializeField] private Image glowBorder; // 动态颜色边框
    [SerializeField] private ParticleSystem rarityParticles; // 稀有度粒子效果

    [Header("稀有度颜色")]
    [SerializeField] private Color commonColor;
    [SerializeField] private Color rareColor;
    [SerializeField] private Color epicColor;
    [SerializeField] private Color legendaryColor;

    private BuffSO currentBuff;
    private System.Action<BuffSO> onClickCallback;

    public BuffSO CurrentBuff => currentBuff;

    public void Setup(BuffSO buff, System.Action<BuffSO> onSelectedCallback)
    {
        currentBuff = buff;
        onClickCallback = onSelectedCallback;

        // 设置UI内容
        buffPicture.sprite = buff.buffPicture;
        buffName.text = buff.buffID.ToString();
        buffDiscription.text = buff.Description;

        // 根据稀有度设置边框颜色
        // 设置初始高亮状态
        SetHighlight(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClickCallback?.Invoke(currentBuff);
    }

    public void SetHighlight(bool isSelected)
    {
        if (isSelected)
        {
            // 激活粒子效果
            rarityParticles.Play();

            // 设置边框颜色为稀有度对应颜色
            glowBorder.color = GetRarityColor(currentBuff.rarity) * 1.2f; // 稍微提亮
            glowBorder.gameObject.SetActive(true);
        }
        else
        {
            rarityParticles.Stop();
            glowBorder.gameObject.SetActive(false);
        }
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
}
