using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LevelStatusPolicer : SingletonMono<LevelStatusPolicer>
{
    private GameStateMachine _stateMachine;
    private GameState stateBeforePause; // 记录暂停前的状态

    public GameState CurrentState => _stateMachine.CurrentState;

    protected override async void Init()
    {
        // 初始化状态机
        _stateMachine = new GameStateMachine(GameState.Menu);

        // 等待所有SO数据加载完成
        while (!DataManager.Instance.IsPlayerDataLoaded || !DataManager.Instance.IsAnimationDataLoaded || !DataManager.Instance.IsBuffDataLoaded)
        {
            await Task.Yield();
        }

        RegisterEvents();

        EventBus.TriggerChangeState(GameState.Menu);
    }

    private void RegisterEvents()
    {
        EventBus.OnChangeState += (newState) =>
        {
            GameState oldState = _stateMachine.CurrentState;
            if (_stateMachine.TryChangeState(newState))
            {
                Debug.Log($"状态切换: {oldState} -> {newState}");
                EventQueueManager.ExecuteStateEvents(newState);
            }
        };

        EventBus.OnPause += () =>
        {
            if (_stateMachine.CurrentState == GameState.Battle)
            {
                stateBeforePause = _stateMachine.CurrentState;
                EventQueueManager.ExecutePauseEvents();
                Debug.Log($"游戏暂停，之前状态: {stateBeforePause}");
            }
        };

        EventBus.OnResumed += () =>
        {
            if (stateBeforePause == GameState.Battle)
            {
                EventQueueManager.ExecuteResumeEvents();
                Debug.Log($"游戏继续，当前状态: {_stateMachine.CurrentState}");
            }
        };
    }


}