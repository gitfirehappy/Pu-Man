using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStatusPolicer : SingletonMono<LevelStatusPolicer>
{
    private GameState currentState;
    private GameState stateBeforePause; // 记录暂停前的状态

    protected override void Init()
    {
        RegisterEvents();
        ChangeState(GameState.Menu);
    }

    private void RegisterEvents()
    {
        EventBus.OnCharacterSelected += () => ChangeState(GameState.Prepare);
        EventBus.OnBattleStart += () => ChangeState(GameState.Battle);
        EventBus.OnBuffSelected += () => ChangeState(GameState.SelectBuff);
        EventBus.OnPlayerDeath += () => ChangeState(GameState.GameOver);

        EventBus.OnGamePaused += OnGamePaused;
        EventBus.OnGameResumed += OnGameResumed;
    }

    private void OnGamePaused()
    {
        stateBeforePause = currentState;
        EventBus.TriggerPlayerDisabled();
        EventBus_Paused();
        Debug.Log($"游戏暂停，之前状态: {stateBeforePause}");
    }

    private void OnGameResumed()
    {
        if (stateBeforePause == GameState.Battle)
        {
            EventBus.TriggerPlayerEnabled();
        }
        Debug.Log($"游戏继续，恢复到: {stateBeforePause}");
        // 不需要改变currentState，因为我们只是从暂停中恢复
    }

    /// <summary>
    /// 转换游戏状态
    /// </summary>
    /// <param name="newState"></param>
    private void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        GameState previousState = currentState;
        currentState = newState;

        Debug.Log($"状态切换: {previousState} -> {newState}");

        // 通知UI管理器状态变化
        EventBus.TriggerGameStateChanged(newState);

        UpdatePlayerState();

        TriggerStateEvents();
    }

    /// <summary>
    /// 处理玩家状态
    /// </summary>
    private void UpdatePlayerState()
    {
        switch (currentState)
        {
            case GameState.Battle:
                EventBus.TriggerPlayerEnabled();
                break;
            case GameState.GameOver:
            case GameState.Paused:
            case GameState.SelectBuff:
                EventBus.TriggerPlayerDisabled();
                break;
        }
    }

    /// <summary>
    /// 触发切换状态时广播
    /// </summary>
    private void TriggerStateEvents()
    {
        switch (currentState)
        {
            case GameState.Menu: 
                EventBus_Menu();
                break;
            case GameState.Prepare: 
                EventBus_Prepare(); 
                break;
            case GameState.Battle: 
                EventBus_Battle(); 
                break;
            case GameState.SelectBuff: 
                EventBus_SelectBuff(); 
                break;
            case GameState.GameOver: 
                EventBus_GameOver(); 
                break;
        }
    }

    private void EventBus_Menu() { /* 广播出去给UI管理器,菜单界面等 */ }
    private void EventBus_Prepare() { /* 通知角色选择模块 */ }
    private void EventBus_Battle() { /* 通知关卡计时器、波次生成等战斗开始 */ }
    private void EventBus_SelectBuff() { /* 先通知敌人生成器，计时器等战斗停止，再通知Buff抽卡系统 */ }
    private void EventBus_GameOver() { /* 通知失败UI ，通知战斗内停止*/ }
    private void EventBus_Paused() { /* 通知暂停UI ，关卡计时器，敌人生成器等战斗停止*/ }
    
}
