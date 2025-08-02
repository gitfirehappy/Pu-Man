using UnityEngine;
using UnityEngine.UI;

public class CreditsPanel : UIFormBase
{
    [Header("关闭按钮")] public Button closeButton;

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