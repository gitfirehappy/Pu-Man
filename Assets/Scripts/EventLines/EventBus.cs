using System;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// 全局事件总线系统，用于模块间通信
/// </summary>
public static class EventBus
{
    public static event Action<GameState> OnChangeState;
    public static event Action OnPause;
    public static event Action OnResumed;

    public static void TriggerChangeState(GameState state) => OnChangeState?.Invoke(state);
    public static void TriggerPause() => OnPause?.Invoke();
    public static void TriggerResumed() => OnResumed?.Invoke();
}