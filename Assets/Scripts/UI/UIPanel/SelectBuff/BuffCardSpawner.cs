using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelectBuffPanel))]
public class BuffCardSpawner : MonoBehaviour
{
    [Header("Buff卡片模板")]
    [SerializeField] private GameObject cardPrefab;

    [Header("卡片生成位置容器")]
    [SerializeField] private Transform[] cardPositions; // 在Inspector中拖入3个空物体

    private List<BuffPanel> buffPanels = new List<BuffPanel>();

    /// <summary>
    /// 生成Buff卡片
    /// </summary>
    public void SpawnBuffCards(List<BuffSO> buffs, System.Action<BuffSO> onSelectedCallback)
    {
        // 清空现有卡片
        ClearCards();

        // 确保不超过定位点数量
        int cardCount = Mathf.Min(buffs.Count, cardPositions.Length);

        for (int i = 0; i < cardCount; i++)
        {
            var card = UIManager.Instance.CreateDynamicForm<BuffPanel>(
                cardPrefab,
                 UIGroupID.BUFF_CARDS,
                cardPositions[i],
                onCreated:card => card.Setup(buffs[i], onSelectedCallback)
            );

            if (card != null)
            {
                card.Setup(buffs[i], onSelectedCallback);
                buffPanels.Add(card);

                UIManager.Instance.ShowDynamicForm(card);//显示BuffCard
            }
            else
            {
                Debug.LogError("卡片预制体缺少BuffPanel组件!");
            }
        }
    }

    /// <summary>
    /// 清空所有卡片
    /// </summary>
    public void ClearCards()
    {
        UIManager.Instance.ClearDynamicFormsInGroup(UIGroupID.BUFF_CARDS);

        buffPanels.Clear();
    }

    /// <summary>
    /// 获取指定Buff的Panel
    /// </summary>
    public BuffPanel GetBuffPanel(BuffSO buff)
    {
        foreach (var panel in buffPanels)
        {
            if (panel.CurrentBuff == buff)
                return panel;
        }
        return null;
    }
}