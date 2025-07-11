using UnityEngine;
using static EnemySO;

public class EnemyReward : MonoBehaviour
{
    private PlayerCore _playerCore;
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
            _playerCore = GetComponent<PlayerCore>();
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
        if (_playerCore == null || _rewardConfig == null) return;

        //应用回血效果
        if (_rewardConfig.hasHealthReward && _rewardConfig.healthUp > 0)
        {
            _playerCore.Health.AddCurrentHealth(_rewardConfig.healthUp);
            Debug.Log($"玩家获得 {_rewardConfig.healthUp} 点生命恢复", this);
        }

        // 刷新次数增加逻辑
        if (_rewardConfig.hasRefreshReward && _rewardConfig.extraRefreshChance > 0)
        {
            var buffUIManager = GameUIManager.Instance?.GetSubUIManager<SelectBuffUIManager>();
            if (buffUIManager != null)
            {
                buffUIManager.AddRefreshCount(_rewardConfig.extraRefreshChance);
            }
        }
    }
}