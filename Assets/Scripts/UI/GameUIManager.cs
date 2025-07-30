using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class GameUIManager : SingletonMono<GameUIManager>
{
    [Header("UI资源配置")]
    [SerializeField] private UIResourceConfigSO uiResourceConfig;
    [Header("子控制器")]
    [SerializeField] private Dictionary<GameState, IUIController> uiControllers = new Dictionary<GameState, IUIController>();
    [Header("当前游戏状态")]
    [SerializeField]private GameState currentState;

    protected override async void Init()
    {
        // 等待所有SO数据加载完成
        while (!DataManager.Instance.IsPlayerDataLoaded || !DataManager.Instance.IsAnimationDataLoaded || !DataManager.Instance.IsBuffDataLoaded)
        {
            await Task.Yield();
        }

        // 初始化UI管理器
        UIManager.Instance.Initialize(uiResourceConfig);

        // 注册控制器
        RegisterControllers();

        // 为每个状态添加UI切换事件
        foreach (GameState state in Enum.GetValues(typeof(GameState)))
        {
            EventQueueManager.AddStateEvent(state, () => OnGameStateChanged(state), 10);
        }

        Debug.Log("GameUIManager初始化完成");
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

    /// <summary>
    /// 获取子UI管理器的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetSubUIManager<T>() where T : IUIController
    {
        foreach (var controller in uiControllers.Values)
        {
            if (controller is T)
            {
                return (T)controller;
            }
        }
        return default;
    }
}

// UI分组常量管理类
public static class UIGroupID
{
    public const string CHARACTER_CARDS = "CharacterCards";
    public const string BUFF_CARDS = "BuffCards";
}