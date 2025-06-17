using System;

public static class EventBus
{
    public static event Action OnStartGame;
    public static event Action OnSelectCharacterDone;
    public static event Action OnBattleFinished;
    public static event Action OnPlayerPaused;

    // 提供安全触发方法
    public static void SafeInvoke(Action action)
    {
        action?.Invoke();
    }

    private static event Action _onPlayerDead;
    public static event Action OnPlayerDead
    {
        add => _onPlayerDead += value;
        remove => _onPlayerDead -= value;
    }

    // 新增玩家状态事件
    public static event Action OnPlayerDisabled; // 玩家被禁用（死亡/暂停等）
    public static event Action OnPlayerEnabled;  // 玩家恢复（复活/取消暂停）

    // 提供安全的触发方法
    public static void TriggerPlayerDisabled() => OnPlayerDisabled?.Invoke();
    public static void TriggerPlayerEnabled() => OnPlayerEnabled?.Invoke();
    public static void TriggerPlayerDeath() => _onPlayerDead?.Invoke();
}
