using UnityEngine;

public class TestUIShowHide : MonoBehaviour
{
    [Header("UI 测试设置")]
    [Tooltip("开启 UI 测试模式")]
    public bool enableTesting = true;

    [Tooltip("显示主菜单面板的按键")]
    public KeyCode toggleMainMenuKey = KeyCode.F1;

    [Tooltip("显示角色选择面板的按键")]
    public KeyCode toggleCharacterSelectKey = KeyCode.F2;

    [Tooltip("显示战斗面板的按键")]
    public KeyCode toggleBattlePanelKey = KeyCode.F3;

    [Tooltip("显示Buff选择面板的按键")]
    public KeyCode toggleBuffSelectKey = KeyCode.F4;

    [Tooltip("显示暂停面板的按键")]
    public KeyCode togglePausePanelKey = KeyCode.F5;

    [Tooltip("显示游戏结束面板的按键")]
    public KeyCode toggleGameOverPanelKey = KeyCode.F6;

    private void Update()
    {
        if (!enableTesting) return;

        // 切换主菜单面板
        if (Input.GetKeyDown(toggleMainMenuKey))
        {
            TogglePanel<MainMenuPanel>();
        }

        // 切换角色选择面板
        if (Input.GetKeyDown(toggleCharacterSelectKey))
        {
            TogglePanel<SelectCharacterPanel>();
        }

        // 切换战斗面板
        if (Input.GetKeyDown(toggleBattlePanelKey))
        {
            TogglePanel<BattleUIPanel>();
        }

        // 切换Buff选择面板
        if (Input.GetKeyDown(toggleBuffSelectKey))
        {
            TogglePanel<SelectBuffPanel>();
        }

        // 切换暂停面板
        if (Input.GetKeyDown(togglePausePanelKey))
        {
            TogglePanel<PausePanel>();
        }

        // 切换游戏结束面板
        if (Input.GetKeyDown(toggleGameOverPanelKey))
        {
            TogglePanel<GameOverPanel>();
        }
    }

    /// <summary>
    /// 切换指定UI面板的显示状态
    /// </summary>
    /// <typeparam name="T">UI面板类型</typeparam>
    private void TogglePanel<T>() where T : UIFormBase
    {
        // 检查UI管理器是否已初始化
        if (UIManager.Instance == null)
        {
            Debug.LogError("UIManager 未初始化！");
            return;
        }

        // 尝试获取面板实例
        T panel = UIManager.Instance.GetForm<T>();

        if (panel != null)
        {
            if (panel.IsOpen)
            {
                UIManager.Instance.HideUIForm<T>();
                Debug.Log($"已关闭 {typeof(T).Name}");
            }
            else
            {
                UIManager.Instance.ShowUIForm<T>();
                Debug.Log($"已显示 {typeof(T).Name}");
            }
        }
        else
        {
            // 如果面板不存在，则创建并显示
            UIManager.Instance.ShowUIForm<T>();
            Debug.Log($"创建并显示 {typeof(T).Name}");
        }
    }

    /// <summary>
    /// 在编辑器中显示帮助信息
    /// </summary>
    private void OnGUI()
    {
        if (!enableTesting) return;

        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.yellow }
        };

        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        GUILayout.Label("UI 面板测试控制", style);
        GUILayout.Label($"主菜单面板: {toggleMainMenuKey}", style);
        GUILayout.Label($"角色选择面板: {toggleCharacterSelectKey}", style);
        GUILayout.Label($"战斗面板: {toggleBattlePanelKey}", style);
        GUILayout.Label($"Buff选择面板: {toggleBuffSelectKey}", style);
        GUILayout.Label($"暂停面板: {togglePausePanelKey}", style);
        GUILayout.Label($"游戏结束面板: {toggleGameOverPanelKey}", style);
        GUILayout.EndArea();
    }
}