using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundInitializer : MonoBehaviour
{
    private void Start()
    {
        // 为所有子按钮添加音效组件
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            if (button.GetComponent<ButtonSound>() == null)
            {
                button.gameObject.AddComponent<ButtonSound>();
            }
        }
    }
}