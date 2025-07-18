using UnityEngine;

[CreateAssetMenu(fileName = "UIResourceConfig", menuName = "UI/Resource Config")]
public class UIResourceConfigSO : ScriptableObject
{
    [System.Serializable]
    public class UIResourceTag
    {
        public string labelName;
        public bool preloadOnStart = true;
    }

    [Header("AB包标签配置")]
    public UIResourceTag[] uiTags;

    [Header("手动注册的UI预制体")]
    public GameObject[] manualUIForms;

    [Header("额外预加载的UI预制体（角色卡片、Buff卡片等）")]
    public GameObject[] additionalPreloadForms;
}