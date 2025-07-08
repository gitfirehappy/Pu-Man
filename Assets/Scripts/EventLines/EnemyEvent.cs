using System;
using UnityEngine;

/// <summary>
/// 敌人专用事件系统
/// </summary>
public static class EnemyEvent
{
    #region 核心事件

    public static event Action<EnemyCore> OnSpawned;

    public static event Action<EnemyCore, DamageSource> OnDeath; // Enemy实例

    #endregion 核心事件

    #region 触发方法

    public static void TriggerSpawned(EnemyCore enemy)
    {
        if (!ValidateEnemy(enemy)) return;
        SafeTrigger(nameof(OnSpawned), () => OnSpawned?.Invoke(enemy));
    }

    public static void TriggerDeath(EnemyCore enemy, DamageSource source)
    {
        if (!ValidateEnemy(enemy)) return;
        SafeTrigger(nameof(OnDeath), () => OnDeath?.Invoke(enemy, source));
    }

    #endregion 触发方法

    #region 安全验证与调试

    private static bool ValidateEnemy(EnemyCore enemy)
    {
        if (enemy == null || enemy.Equals(null))
        {
            Debug.LogWarning("无效的敌人实例，事件未触发");
            return false;
        }
        return true;
    }

    private static void SafeTrigger(string eventName, Action trigger, string extraInfo = "")
    {
        try
        {
            trigger();
            LogEvent(eventName, extraInfo);
        }
        catch (Exception e)
        {
            Debug.LogError($"敌人事件 {eventName} 触发失败: {e.Message}");
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private static void LogEvent(string eventName, string extraInfo)
    {
        Debug.Log($"[EnemyEvent] {eventName} | {extraInfo}");
    }

    #endregion 安全验证与调试

    #region 编辑器工具

#if UNITY_EDITOR

    [UnityEditor.MenuItem("Tools/Enemy/Print Event Listeners")]
    public static void PrintListeners()
    {
        Debug.Log($"=== 敌人事件监听数 ===\n" +
                 $"{nameof(OnSpawned)}: {OnSpawned?.GetInvocationList().Length ?? 0}\n" +
                 $"{nameof(OnDeath)}: {OnDeath?.GetInvocationList().Length ?? 0}\n");
    }

#endif

    #endregion 编辑器工具
}

public enum DamageSource
{
    SystemCleanup,  // 系统清理
    Player,         // 玩家直接造成
    ChainKill,       // 亵渎技能造成
    Enemy,          //敌人
}