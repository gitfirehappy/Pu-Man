using UnityEngine;
using System.Collections.Generic;

public class GameUIManager : SingletonMono<GameUIManager>
{
    private Dictionary<GameState, IUIController> uiControllers = new Dictionary<GameState, IUIController>();
    private GameState currentState;

    protected override void Init()
    {
        base.Init();

        // 注册所有UI控制器
        RegisterControllers();

        // 监听状态变化
        EventBus.OnGameStateChanged += OnGameStateChanged;
    }

    private void RegisterControllers()
    {
        // 手动注册或自动查找
        uiControllers.Add(GameState.Menu, new MenuUIManager());
        uiControllers.Add(GameState.Prepare, new CharacterSelectUIManager());
        uiControllers.Add(GameState.Battle, new BattleUIManager());
        uiControllers.Add(GameState.SelectBuff, new SelectBuffUIManager());
        uiControllers.Add(GameState.GameOver, new GameOverUIManager());
        uiControllers.Add(GameState.Paused, new PauseUIManager());
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
