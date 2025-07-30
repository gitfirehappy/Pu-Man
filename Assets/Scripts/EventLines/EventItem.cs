using System;

/// <summary>
/// 事件项类，包含函数和优先级
/// </summary>
public class EventItem
{
    public Action Action { get; }
    public int Priority { get; }
    public string MethodName { get; }  // 方法名记录

    public EventItem(Action action, int priority)
    {
        Action = action;
        Priority = priority;
        MethodName = GetMethodName(action);
    }

    // 获取方法名的辅助方法
    private static string GetMethodName(Action action)
    {
        if (action == null) return "Null Action";

        var method = action.Method;
        if (method.DeclaringType == null) return method.Name;

        return $"{method.DeclaringType.FullName}.{method.Name}";
    }
}