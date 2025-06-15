using System;
using UnityEngine;

/// <summary>
/// 敌人事件系统 | 双击日志可定位触发对象 | 编辑器右键菜单打印监听器
/// </summary>
public static class EnemyEvent
{
    #region 核心事件
    public static event Action<EnemyCore> OnDied;
    public static event Action<EnemyCore> OnSpawned;
    public static event Action<EnemyCore, float> OnDamaged;
    public static event Action<EnemyCore, bool> OnReturnedToPool;
    #endregion

    #region 触发方法
    public static void TriggerDied(EnemyCore enemy)
    {
        if (!IsValid(enemy)) return;
        SafeTrigger(nameof(OnDied), enemy, () => OnDied?.Invoke(enemy));
    }

    private static bool IsValid(EnemyCore enemy)
    {
        if (enemy == null || enemy.Equals(null))
        {
            Debug.LogWarning("尝试触发事件的敌人已销毁");
            return false;
        }
        return true;
    }

    public static void TriggerSpawned(EnemyCore enemy)
        => SafeTrigger(nameof(OnSpawned), enemy, () => OnSpawned?.Invoke(enemy));

    public static void TriggerDamaged(EnemyCore enemy, float damage)
        => SafeTrigger(nameof(OnDamaged), enemy, () => OnDamaged?.Invoke(enemy, damage), damage.ToString());

    public static void TriggerReturnedToPool(EnemyCore enemy, bool killedByPlayer)
        => SafeTrigger(nameof(OnReturnedToPool), enemy, () => OnReturnedToPool?.Invoke(enemy, killedByPlayer), $"killed:{killedByPlayer}");
    #endregion

    #region 安全调用与调试
    private static void SafeTrigger(string eventName, EnemyCore enemy, Action triggerAction, string extraInfo = "")
    {
        try
        {
            triggerAction();
            DebugEvent(eventName, enemy, extraInfo);
        }
        catch (Exception e)
        {
            Debug.LogError($"EnemyEvent {eventName} Error: {e.Message}\nEnemy: {enemy.name}", enemy.gameObject);
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private static void DebugEvent(string eventName, EnemyCore enemy, string extraInfo)
    {
        var stackTrace = new System.Diagnostics.StackTrace(2, true);
        Debug.Log($"[EnemyEvent] {eventName} - {enemy.GetEnemyType()}\n" +
                 $"Pos: {enemy.transform.position}\n" +
                 $"Extra: {extraInfo}\n" +
                 $"StackTrace:\n{stackTrace}",
                 enemy.gameObject);
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Debug/EnemyEvent/Print Listeners")]
#endif
    public static void PrintListeners()
    {
#if UNITY_EDITOR
        Debug.Log($"=== EnemyEvent Listeners ===\n" +
                 $"{nameof(OnDied)}: {OnDied?.GetInvocationList().Length ?? 0}\n" +
                 $"{nameof(OnSpawned)}: {OnSpawned?.GetInvocationList().Length ?? 0}\n" +
                 $"{nameof(OnDamaged)}: {OnDamaged?.GetInvocationList().Length ?? 0}\n" +
                 $"{nameof(OnReturnedToPool)}: {OnReturnedToPool?.GetInvocationList().Length ?? 0}");
#endif
    }
    #endregion
}