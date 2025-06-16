using System;

public static class EventBus
{
    public static Action OnStartGame;
    public static Action OnSelectCharacterDone;
    public static Action OnBattleFinished;
    public static Action OnPlayerDead;
    public static Action OnPlayerPaused;
}
