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

    [Header("角色卡片模板")]
    public GameObject[] characterCardTemplates;

    [Header("Buff卡片模板")]
    public GameObject[] buffCardTemplates;
}