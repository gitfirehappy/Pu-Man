using System;

/// <summary>
/// 全局事件总线系统，用于模块间通信
/// </summary>
public static class EventBus
{
    #region 游戏流程事件

    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnCharacterSelected;
    public static event Action OnBattleStart;//战斗开始
    public static event Action OnBuffSelected;//开始选buff
    public static event Action OnTimeOut; // 计时器归零
    public static event Action OnAllWavesCompleted; // 所有波次完成
    public static event Action OnEndlessModeActivated; // 进入无尽模式
    public static event Action<int> OnWaveChanged; // 波次变化

    #endregion

    #region 暂停相关

    public static event Action OnGamePaused;
    public static event Action OnPauseUIRequested;
    public static event Action OnGameResumed;
    public static event Action OnResumeUIRequested;

    #endregion

    #region 玩家状态事件

    private static event Action _onPlayerDeath;
    public static event Action OnPlayerDeath
    {
        add => _onPlayerDeath += value;
        remove => _onPlayerDeath -= value;
    }
    public static event Action OnPlayerDisabled;
    public static event Action OnPlayerEnabled;

    #endregion

    #region 触发方法

    public static void TriggerGameStateChanged(GameState state) => OnGameStateChanged?.Invoke(state);
    public static void TriggerBattleStart() => OnBattleStart?.Invoke();
    public static void TriggerCharacterSelected() => OnCharacterSelected?.Invoke();
    public static void TriggerBuffSelected() => OnBuffSelected?.Invoke();

    public static void TriggerPlayerDeath() => _onPlayerDeath?.Invoke();
    public static void TriggerPlayerDisabled() => OnPlayerDisabled?.Invoke();
    public static void TriggerPlayerEnabled() => OnPlayerEnabled?.Invoke();

    public static void TriggerGamePaused() => OnGamePaused?.Invoke();
    public static void TriggerGameResumed() => OnGameResumed?.Invoke();
    public static void TriggerPauseUIRequested() => OnPauseUIRequested?.Invoke();
    public static void TriggerResumeUIRequested() => OnResumeUIRequested?.Invoke();

    public static void TriggerTimeOut() => OnTimeOut?.Invoke();
    public static void TriggerAllWavesCompleted() => OnAllWavesCompleted?.Invoke();
    public static void TriggerEndlessModeActivated() => OnEndlessModeActivated?.Invoke();
    public static void TriggerWaveChanged(int wave) => OnWaveChanged?.Invoke(wave);

    #endregion
}