using UnityEngine;
using UnityEngine.UI;

public class CreditsPanel : UIFormBase
{
    [SerializeField][Header("关闭按钮")] private Button closeButton;

    protected override void Init()
    {
        gameObject.AddComponent<ButtonSoundInitializer>();

        closeButton.onClick.AddListener(ClosePanel);
    }

    private void ClosePanel()
    {
        UIManager.Instance.HideUIForm<CreditsPanel>();
    }
}