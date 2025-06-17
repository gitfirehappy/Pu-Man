using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStatusPolicer : SingletonMono<LevelStatusPolicer>
{
    private GameState currentState;

    protected override void Init()
    {
        RegisterEvents();
        ChangeState(GameState.Menu);
    }

    private void RegisterEvents()
    {
        EventBus.OnStartGame += () => ChangeState(GameState.Prepare);
        EventBus.OnSelectCharacterDone += () => ChangeState(GameState.Battle);
        EventBus.OnBattleFinished += () => ChangeState(GameState.SelectBuff);
        EventBus.OnPlayerDead += () => ChangeState(GameState.GameOver);
        EventBus.OnPlayerPaused += () => ChangeState(GameState.Paused);
    }

    private void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log("切换到状态: " + newState);

        switch (currentState)
        {
            case GameState.GameOver:
            case GameState.Paused:
                EventBus.TriggerPlayerDisabled(); // 禁用玩家
                break;

            case GameState.Battle:
                EventBus.TriggerPlayerEnabled(); // 启用玩家
                break;
        }

        // 状态切换后广播出去，其他系统监听
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
            case GameState.Paused:
                EventBus_Paused();
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
