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
    [Header("角色名称")]public TextMeshPro characterName;

    private PlayerSO playerSO;
    private Action<PlayerSO> onClickCallback;

    public void Setup(PlayerSO data,Action<PlayerSO> onClick)
    {
        playerSO = data;
        onClickCallback = onClick;

        characterPicture.sprite = data.playerSprite;
        characterName.text = data.playerName;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        onClickCallback?.Invoke(playerSO);
    }
}
