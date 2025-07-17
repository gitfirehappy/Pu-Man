using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterPanel : UIFormBase, IPointerClickHandler
{
    [Header("角色图片")]public Image characterPicture;
    [Header("角色名称")]public TextMeshProUGUI characterName;
    [Header("最高波次")] public TextMeshProUGUI historicalBest;

    private PlayerSO playerSO;
    private Action<PlayerSO> onClickCallback;

    public void Setup(PlayerSO data,Action<PlayerSO> onClick)
    {
        playerSO = data;
        onClickCallback = onClick;

        characterPicture.sprite = data.playerSprite;
        characterName.text = data.playerName;

        // 显示最高波次
        int highestWave = CharacterDataManager.Instance.GetHighestWave(data.playerType);
        historicalBest.text = $"最高记录: {highestWave}波";
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        onClickCallback?.Invoke(playerSO);
    }
}
