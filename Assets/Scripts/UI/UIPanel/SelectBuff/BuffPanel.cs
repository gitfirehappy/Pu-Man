using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffPanel : UIFormBase
{
    [SerializeField][Header("Buff图片")] private Image buffPicture;
    [SerializeField][Header("Buff名称")] private TextMeshPro buffName;
    [SerializeField][Header("Buff描述")] private TextMeshPro buffDiscription;
    [SerializeField][Header("稀有度底色")] private Image rarityBorder;

    [Header("稀有度颜色")]
    [SerializeField] private Color commonColor;
    [SerializeField] private Color rareColor;
    [SerializeField] private Color epicColor;
    [SerializeField] private Color legendaryColor;

    private BuffSO buffSO;
    private Action<BuffSO> onSelected;

    public void Setup(BuffSO buff, System.Action<BuffSO> onSelectedCallback)
    {
        buffSO = buff;
        onSelected = onSelectedCallback;

        // 设置UI内容
        buffName.text = buff.buffID.ToString();
        buffDiscription.text = buff.Description;

        // 根据稀有度设置边框颜色
        switch (buff.rarity)
        {
            case Rarity.Common:
                rarityBorder.color = commonColor;
                break;
            case Rarity.Rare:
                rarityBorder.color = rareColor;
                break;
            case Rarity.Epic:
                rarityBorder.color = epicColor;
                break;
            case Rarity.Legendary:
                rarityBorder.color = legendaryColor;
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onSelected?.Invoke(buffSO);
    }

}
