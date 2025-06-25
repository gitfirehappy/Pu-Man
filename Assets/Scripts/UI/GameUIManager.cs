using UnityEngine;
using System.Collections.Generic;

public class GameUIManager : SingletonMono<GameUIManager>
{
    [Header("需要预加载的UI预制体")]
    [SerializeField] private GameObject[] uiPrefabsToPreload;

    private Dictionary<GameState, IUIController> uiControllers = new Dictionary<GameState, IUIController>();
    private GameState currentState;

    protected override void Init()
    {
        base.Init();

        // 1. 预加载所有UI预制体
        PreloadAllUIForms();

        // 2. 注册控制器
        RegisterControllers();

        // 3. 监听状态变化
        EventBus.OnGameStateChanged += OnGameStateChanged;
    }

    /// <summary>
    /// 预加载UIPanel预制体
    /// </summary>
    private void PreloadAllUIForms()
    {
        if (uiPrefabsToPreload != null && uiPrefabsToPreload.Length > 0)
        {
            UIManager.Instance.PreLoadForms(uiPrefabsToPreload);
        }
    }

    /// <summary>
    /// 注册控制器
    /// </summary>
    private void RegisterControllers()
    {
        // 清空字典以防重复注册
        uiControllers.Clear();

        // 自动获取子物体上的控制器组件
        uiControllers.Add(GameState.Menu, GetComponentInChildren<MenuUIManager>());
        uiControllers.Add(GameState.Prepare, GetComponentInChildren<CharacterSelectUIManager>());
        uiControllers.Add(GameState.Battle, GetComponentInChildren<BattleUIManager>());
        uiControllers.Add(GameState.SelectBuff, GetComponentInChildren<SelectBuffUIManager>());
        uiControllers.Add(GameState.GameOver, GetComponentInChildren<GameOverUIManager>());

        // 验证所有控制器都已找到
        foreach (var kvp in uiControllers)
        {
            if (kvp.Value == null)
            {
                Debug.LogError($"未找到 {kvp.Key} 对应的UI控制器");
            }
        }
    }

    /// <summary>
    /// 通知各UI管理器状态切换
    /// </summary>
    /// <param name="newState"></param>
    private void OnGameStateChanged(GameState newState)
    {
        if (currentState == newState) return;

        // 通知旧状态控制器退出
        if (uiControllers.TryGetValue(currentState, out var oldController))
        {
            oldController.OnExitState();
        }

        currentState = newState;

        // 通知新状态控制器进入
        if (uiControllers.TryGetValue(newState, out var newController))
        {
            newController.OnEnterState();
        }
    }

    private void OnDestroy()
    {
        EventBus.OnGameStateChanged -= OnGameStateChanged;
    }
}
