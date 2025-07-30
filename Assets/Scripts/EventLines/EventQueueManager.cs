using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// 事件队列管理器类
/// </summary>
public static class EventQueueManager
{
    private static  Dictionary<GameState, List<EventItem>> stateEventQueues = new Dictionary<GameState, List<EventItem>>();
    private static  List<EventItem> pauseEventQueue = new List<EventItem>();
    private static List<EventItem> resumeEventQueue = new();

    /// <summary>
    /// 添加到状态变化事件队列
    /// </summary>
    /// <param name="state">切换的状态</param>
    /// <param name="action">方法</param>
    /// <param name="priority">执行顺序</param>
    public static void AddStateEvent(GameState state, Action action, int priority)
    {
        if (!stateEventQueues.ContainsKey(state))
        {
            stateEventQueues[state] = new List<EventItem>();
        }
        stateEventQueues[state].Add(new EventItem(action, priority));
        stateEventQueues[state] = stateEventQueues[state].OrderBy(item => item.Priority).ToList();
    }

    /// <summary>
    /// 添加到暂停事件队列
    /// </summary>
    /// <param name="action">方法</param>
    /// <param name="priority">执行顺序</param>
    public static void AddPauseEvent(Action action, int priority)
    {
        pauseEventQueue.Add(new EventItem(action, priority));
        pauseEventQueue = pauseEventQueue.OrderBy(item => item.Priority).ToList();
    }

    /// <summary>
    /// 添加到继续事件队列
    /// </summary>
    /// <param name="action">方法</param>
    /// <param name="priority">执行顺序</param>
    public static void AddResumeEvent(Action action, int priority)
    {
        resumeEventQueue.Add(new EventItem(action, priority));
        resumeEventQueue = resumeEventQueue.OrderBy(item => item.Priority).ToList();
    }

    /// <summary>
    /// 执行状态变化事件队列
    /// </summary>
    /// <param name="state"></param>
    public static void ExecuteStateEvents(GameState state)
    {
        if (stateEventQueues.TryGetValue(state, out var queue))
        {
            Debug.Log($"Executing state events for: {state}");
            LogQueueContents(queue);
            foreach (var eventItem in queue.ToList())
            {
                eventItem.Action.Invoke();
            }
        }
    }

    /// <summary>
    /// 执行暂停事件队列
    /// </summary>
    public static void ExecutePauseEvents()
    {
        Debug.Log("Executing pause events");
        LogQueueContents(pauseEventQueue);
        foreach (var eventItem in pauseEventQueue.ToList())
        {
            eventItem.Action.Invoke();
        }
    }

    /// <summary>
    /// 执行恢复事件队列
    /// </summary>
    public static void ExecuteResumeEvents()
    {
        Debug.Log("Executing resume events");
        LogQueueContents(resumeEventQueue);
        foreach (var eventItem in resumeEventQueue.ToList())
        {
            eventItem.Action.Invoke();
        }
    }

    /// <summary>
    /// 队列内容输出方法
    /// </summary>
    /// <param name="queue"></param>
    private static void LogQueueContents(List<EventItem> queue)
    {
        if (queue.Count == 0)
        {
            Debug.Log("  [Empty]");
            return;
        }

        var ordered = queue.OrderBy(item => item.Priority).ToList();
        var methods = ordered.Select(item => item.MethodName);
        Debug.Log($"  Methods: {string.Join(" / ", methods)}");
    }
}