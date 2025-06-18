using System;
using UnityEngine;

/// <summary>
/// 敌人专用事件系统 | 双击日志定位事件触发点
/// </summary>
public static class EnemyEvent
{
    #region 核心事件
    public static event Action<EnemyCore> OnSpawned;
    public static event Action<EnemyCore> OnDeath; // Enemy实例
    public static event Action<EnemyCore, float, float> OnHealthChanged; // 当前血量, 最大血量
    #endregion

    #region 触发方法
    public static void TriggerSpawned(EnemyCore enemy)
    {
        if (!ValidateEnemy(enemy)) return;
        SafeTrigger(nameof(OnSpawned), () => OnSpawned?.Invoke(enemy));
    }

    public static void TriggerDeath(EnemyCore enemy)
    {
        if (!ValidateEnemy(enemy)) return;
        SafeTrigger(nameof(OnDeath), () => OnDeath?.Invoke(enemy));
    }

    public static void TriggerHealthChanged(EnemyCore enemy, float currentHealth, float maxHealth)
    {
        if (!ValidateEnemy(enemy)) return;
        SafeTrigger(nameof(OnHealthChanged),
            () => OnHealthChanged?.Invoke(enemy, currentHealth, maxHealth),
            $"HP: {currentHealth}/{maxHealth}");
    }
    #endregion

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
    #endregion

    #region 编辑器工具
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Enemy/Print Event Listeners")]
    public static void PrintListeners()
    {
        Debug.Log($"=== 敌人事件监听数 ===\n" +
                 $"{nameof(OnSpawned)}: {OnSpawned?.GetInvocationList().Length ?? 0}\n" +
                 $"{nameof(OnDeath)}: {OnDeath?.GetInvocationList().Length ?? 0}\n" +
                 $"{nameof(OnHealthChanged)}: {OnHealthChanged?.GetInvocationList().Length ?? 0}");
    }
#endif
    #endregion
}