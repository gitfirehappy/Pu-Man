using UnityEngine.Pool;
using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class PlayerCore : MonoBehaviour
{
    [SerializeField] private PlayerSO playerData;

    // 组件引用缓存
    private PlayerHealth health;
    private PlayerShooting shooting;
    private PlayerMovement movement;
    private PlayerAbilities abilities;
    private PlayerAnimatorController animatorController;
    public PlayerInput playerInput; // 统一管理输入


    private void Awake()
    {
        // 初始化输入系统
        playerInput = new PlayerInput();

    }

    /// <summary>
    /// Player组件初始化
    /// </summary>
    private void InitializeComponents()
    {
        if (playerData == null)
        {
            Debug.LogError("PlayerSO not assigned!");
            return;
        }

        // 确保所有必须组件存在并获取引用
        health = GetOrAddComponent<PlayerHealth>();
        shooting = GetOrAddComponent<PlayerShooting>();
        movement = GetOrAddComponent<PlayerMovement>();
        abilities = GetOrAddComponent<PlayerAbilities>();
        animatorController = GetOrAddComponent<PlayerAnimatorController>();

        // 初始化所有组件
        health.Initialize(playerData);
        shooting.Initialize(playerData);
        movement.Initialize(playerData);
        abilities.Initialize(playerData);
        animatorController.Init();

        RegisterEventHandlers();
    }

    private T GetOrAddComponent<T>() where T : Component
    {
        var component = GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }

    /// <summary>
    /// 注册Player事件
    /// </summary>
    private void RegisterEventHandlers()
    {
        // 注册内部组件事件
        health.OnDeath += HandlePlayerDeath;

        EventQueueManager.AddStateEvent(GameState.Battle, EnableAllComponents, 0);
        EventQueueManager.AddStateEvent(GameState.SelectBuff, DisableAllComponents, 2);
    }

    private void OnDestroy()
    {
        // 注销事件
        if (health != null) health.OnDeath -= HandlePlayerDeath;

        // 清理输入系统
        playerInput.Dispose();

        PlayerManager.Instance?.ClearPlayer(this);
    }

    /// <summary>
    /// 处理死亡事件
    /// </summary>
    private void HandlePlayerDeath()
    {
        // 保存记录
        DataManager.Instance.UpdateRecord(
            playerData.playerType,
            WaveCounter.Instance.CurrentWave - 1 // CurrentWave从1开始
        );
        
        //通知总线游戏结束
        EventBus.TriggerChangeState(GameState.GameOver);

        // 销毁自身
        Destroy(gameObject);
    }

    /// <summary>
    /// 禁用Player组件，总线广播，切换状态时调用
    /// </summary>
    private void DisableAllComponents()
    {
        // 禁用输入（核心统一管理）
        playerInput.Disable();

        // 调用各组件内部的禁用逻辑
        health?.DisableHealth();
        shooting?.DisableShooting();
        movement?.DisableMovement();
        abilities?.DisableAbilities();
    }

    /// <summary>
    /// 启用Player组件，总线广播，切换状态时调用
    /// </summary>
    private void EnableAllComponents()
    {
        // 启用输入（核心统一管理）
        playerInput.Enable();

        // 调用各组件内部的启用逻辑
        health?.EnableHealth();
        shooting?.EnableShooting();
        movement?.EnableMovement();
        abilities?.EnableAbilities();
    }

    /// <summary>
    /// 恢复状态,每波开始前调用
    /// </summary>
    public void ResetState()
    {
        // 重置状态（给每波开始前可以调用，主要是回血和重置临时增益）
        health.ResetToBaseStats();
        shooting.ResetToBaseStats();
        movement.ResetToBaseStats();
        abilities.ResetToBaseStats();
        animatorController?.ResetToBaseStats();
    }

    public void SetPlayerData(PlayerSO data)
    {
        playerData = data;
        InitializeComponents();
    }

    public PlayerType GetPlayerType() => playerData.playerType;

    #region 提供给外部访问的接口
    public PlayerHealth Health => health;
    public PlayerShooting Shooting => shooting;
    public PlayerMovement Movement => movement;
    public PlayerAbilities Abilities => abilities;

    public Dictionary<BuffID, BuffSO> GetAcquiredBuffs()
    {
        return BuffManager.Instance.GetAcquiredBuffs();
    }

    #endregion
}