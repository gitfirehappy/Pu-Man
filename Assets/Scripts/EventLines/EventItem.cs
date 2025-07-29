using System;

/// <summary>
/// 事件项类，包含函数和优先级
/// </summary>
public class EventItem
{
    public Action Action { get; }
    public int Priority { get; }

    public EventItem(Action action, int priority)
    {
        Action = action;
        Priority = priority;
    }
}