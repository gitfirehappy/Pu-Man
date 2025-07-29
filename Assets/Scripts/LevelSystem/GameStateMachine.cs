using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine
{
    // 状态节点类
    public class StateNode
    {
        public GameState State { get; }
        public List<StateNode> Transitions { get; } = new List<StateNode>();

        public StateNode(GameState state)
        {
            State = state;
        }
    }

    // 状态图
    private readonly Dictionary<GameState, StateNode> _graph = new Dictionary<GameState, StateNode>();
    public GameState CurrentState { get; private set; }

    public GameStateMachine(GameState initialState)
    {
        CurrentState = initialState;

        // 初始化所有状态节点
        foreach (GameState state in System.Enum.GetValues(typeof(GameState)))
        {
            _graph[state] = new StateNode(state);
        }

        // 配置合法状态转换
        AddTransition(GameState.Menu, GameState.Prepare);
        AddTransition(GameState.Prepare, GameState.Battle);
        AddTransition(GameState.Prepare, GameState.Menu);
        AddTransition(GameState.Battle, GameState.SelectBuff);
        AddTransition(GameState.Battle, GameState.GameOver);
        AddTransition(GameState.SelectBuff, GameState.Battle);
        AddTransition(GameState.GameOver, GameState.Menu);
    }

    private void AddTransition(GameState from, GameState to)
    {
        if (_graph.TryGetValue(from, out var fromNode) &&
            _graph.TryGetValue(to, out var toNode))
        {
            fromNode.Transitions.Add(toNode);
        }
    }

    public bool CanTransitionTo(GameState newState)
    {
        if (!_graph.TryGetValue(CurrentState, out var currentNode))
            return false;

        return currentNode.Transitions.Exists(node => node.State == newState);
    }

    public bool TryChangeState(GameState newState)
    {
        if (CanTransitionTo(newState))
        {
            CurrentState = newState;
            return true;
        }

        Debug.LogError($"非法状态转换: {CurrentState} -> {newState}");
        return false;
    }
}