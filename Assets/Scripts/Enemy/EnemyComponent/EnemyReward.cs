using UnityEngine;
using static EnemySO;

public class EnemyReward : MonoBehaviour
{
    private PlayerHealth _playerHealth;
    private RewardConfig _rewardConfig;
    private EnemyCore _core;

    /// <summary>
    /// 敌人奖励系统初始化
    /// </summary>
    public void Initialize(EnemySO data)
    {
        _rewardConfig = data.rewardConfig;
        _core = GetComponent<EnemyCore>();

        _core.OnEnemyDeath += HandleEnemyDeath;

        // 获取玩家引用
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    private void OnDestroy()
    {
        // 注销事件
        if (_core != null)
        {
            _core.OnEnemyDeath -= HandleEnemyDeath;
        }
    }

    /// <summary>
    /// 重置奖励系统状态
    /// </summary>
    public void ResetToBaseStats()
    {
        // 目前没有需要重置的状态变量
    }

    private void HandleEnemyDeath()
    {
        ApplyReward();
    }

    /// <summary>
    /// 应用奖励
    /// </summary>
    private void ApplyReward()
    {
        if (_playerHealth == null || _rewardConfig == null) return;

        // 应用回血效果
        if (_rewardConfig.healthUp > 0)
        {
            _playerHealth.AddCurrentHealth(_rewardConfig.healthUp);
            Debug.Log($"玩家获得 {_rewardConfig.healthUp} 点生命恢复", this);
        }

        // TODO: 刷新次数增加逻辑
        // if (_rewardConfig.extraRefreshChance > 0) {...}
    }
}