using System;

/// <summary>
/// 全局事件总线系统，用于模块间通信
/// </summary>
public static class EventBus
{
    #region 游戏流程事件
    /// <summary>
    /// 游戏状态切换时触发
    /// </summary>
    public static event Action<GameState> OnGameStateChanged;

    /// <summary>
    /// 当角色选择完成时触发
    /// </summary>
    public static event Action OnCharacterSelected;

    /// <summary>
    /// 当选择buff结束，战斗开始时触发
    /// </summary>
    public static event Action OnBattleStart;

    /// <summary>
    /// 当战斗结束，开始选择buff时触发
    /// </summary>
    public static event Action OnBuffSelected;

    /// <summary>
    /// 当游戏暂停时触发
    /// </summary>
    public static event Action OnGamePaused;

    /// <summary>
    /// 当游戏继续时触发
    /// </summary>
    public static event Action OnGameResumed;

    #endregion

    #region 玩家状态事件

    private static event Action _onPlayerDeath;

    /// <summary>
    /// 当玩家死亡时触发
    /// </summary>
    public static event Action OnPlayerDeath
    {
        add => _onPlayerDeath += value;
        remove => _onPlayerDeath -= value;
    }

    /// <summary>
    /// 当死亡，暂停，选择buff时触发
    /// </summary>
    public static event Action OnPlayerDisabled;

    /// <summary>
    /// 当继续，开始战斗时触发
    /// </summary>
    public static event Action OnPlayerEnabled;

    #endregion

    #region 触发方法
    public static void TriggerGameStateChanged(GameState state) => OnGameStateChanged?.Invoke(state);

    public static void TriggerBattleStart() => OnBattleStart?.Invoke();
    public static void TriggerCharacterSelected() => OnCharacterSelected?.Invoke();
    public static void TriggerBuffSelected() => OnBuffSelected?.Invoke();
    public static void TriggerGamePaused() => OnGamePaused?.Invoke();
    public static void TriggerGameResumed() => OnGameResumed?.Invoke();
    public static void TriggerPlayerDeath() => _onPlayerDeath?.Invoke();
    public static void TriggerPlayerDisabled() => OnPlayerDisabled?.Invoke();
    public static void TriggerPlayerEnabled() => OnPlayerEnabled?.Invoke();

    #endregion
}