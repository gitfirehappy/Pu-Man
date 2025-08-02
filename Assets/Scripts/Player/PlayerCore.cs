using UnityEngine.Pool;
using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class PlayerCore : MonoBehaviour
{
    [Header("玩家配置数据SO")]
    [SerializeField] private PlayerSO playerData;

    // 组件引用缓存
    private PlayerHealth health;
    private PlayerShooting shooting;
    private PlayerMovement movement;
    private PlayerAbilities abilities;
    private PlayerAnimatorController animatorController;
    public PlayerInput playerInput; // 统一管理输入
    private Collider2D playerCollider;

    #region 生命周期
    private void Awake()
    {
        // 初始化输入系统
        playerInput = new PlayerInput();
    }

    private void OnDestroy()
    {
        // 注销事件
        if (health != null) health.OnDeath -= HandlePlayerDeath;

        // 销毁前停止所有技能
        abilities?.DisableAbilities();


        // 清理输入系统
        playerInput.Dispose();

        PlayerManager.Instance?.ClearPlayer(this);
    }
    #endregion

    #region 初始化
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
        playerCollider = GetComponent<CircleCollider2D>();

        // 初始化所有组件
        health.Initialize(playerData);
        shooting.Initialize(playerData);
        movement.Initialize(playerData);
        abilities.Initialize(playerData);
        animatorController.Initialize();

        RegisterEventHandlers();
    }

    /// <summary>
    /// 获取或添加组件（确保组件存在）
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns>获取到的组件</returns>
    private T GetOrAddComponent<T>() where T : Component
    {
        var component = GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }
    #endregion

    #region 事件处理
    /// <summary>
    /// 注册Player内部事件和外部事件
    /// </summary>
    private void RegisterEventHandlers()
    {
        // 注册内部组件事件
        health.OnDeath += HandlePlayerDeath;

        EventQueueManager.AddStateEvent(GameState.Battle, EnableAllComponents, 0);
        EventQueueManager.AddStateEvent(GameState.SelectBuff, DisableAllComponents, 2);
    }

    /// <summary>
    /// 处理死亡事件
    /// </summary>
    private void HandlePlayerDeath()
    {   
        //通知总线游戏结束
        EventBus.TriggerChangeState(GameState.GameOver);

        // 销毁自身
        Destroy(gameObject);
    }
    #endregion

    #region 启用禁用组件
    /// <summary>
    /// 启用Player组件，总线广播，切换状态时调用
    /// </summary>
    private void EnableAllComponents()
    {
        // 启用输入（核心统一管理）
        playerInput.Enable();

        if (playerCollider != null)
            playerCollider.enabled = true;

        // 调用各组件内部的启用逻辑
        health?.EnableHealth();
        shooting?.EnableShooting();
        movement?.EnableMovement();
        abilities?.EnableAbilities();
    }

    /// <summary>
    /// 禁用Player组件，总线广播，切换状态时调用
    /// </summary>
    private void DisableAllComponents()
    {
        // 禁用输入（核心统一管理）
        playerInput.Disable();

        if (playerCollider != null)
            playerCollider.enabled = false;

        // 调用各组件内部的禁用逻辑
        health?.DisableHealth();
        shooting?.DisableShooting();
        movement?.DisableMovement();
        abilities?.DisableAbilities();
    }
    #endregion

    #region 外部接口
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

    /// <summary>
    /// 设置玩家配置数据并初始化
    /// </summary>
    /// <param name="data">玩家配置SO</param>
    public void SetPlayerData(PlayerSO data)
    {
        playerData = data;
        InitializeComponents();
    }

    #region 组件访问器
    public PlayerHealth Health => health;
    public PlayerShooting Shooting => shooting;
    public PlayerMovement Movement => movement;
    public PlayerAbilities Abilities => abilities;
    #endregion

    public Dictionary<BuffID, BuffSO> GetAcquiredBuffs()
    {
        return BuffManager.Instance.GetAcquiredBuffs();
    }

    public PlayerType GetPlayerType() => playerData.playerType;

    public PlayerSO PlayerData => playerData;
    #endregion
}