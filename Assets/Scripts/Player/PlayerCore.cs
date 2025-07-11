using UnityEngine.Pool;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerCore : MonoBehaviour
{
    [SerializeField] private PlayerSO playerData;

    // 组件引用缓存
    private PlayerHealth health;
    private PlayerShooting shooting;
    private PlayerMovement movement;
    private PlayerAbilities abilities;
    private PlayerBuff buff;
    public PlayerInput playerInput; // 统一管理输入


    private void Awake()
    {
        if (playerData == null)
        {
            Debug.LogError("PlayerSO not assigned!");
            return;
        }
        // 初始化输入系统
        playerInput = new PlayerInput();

        InitializeComponents();
        RegisterEventHandlers();
    }

    /// <summary>
    /// Player组件初始化
    /// </summary>
    private void InitializeComponents()
    {
        // 确保所有必须组件存在并获取引用
        health = GetOrAddComponent<PlayerHealth>();
        shooting = GetOrAddComponent<PlayerShooting>();
        movement = GetOrAddComponent<PlayerMovement>();
        abilities = GetOrAddComponent<PlayerAbilities>();
        buff = GetOrAddComponent<PlayerBuff>();

        // 初始化所有组件
        health.Initialize(playerData);
        shooting.Initialize(playerData);
        movement.Initialize(playerData);
        abilities.Initialize(playerData);
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

        // 注册外部系统事件
        EventBus.OnPlayerDisabled += DisableAllComponents;
        EventBus.OnPlayerEnabled += EnableAllComponents;
    }

    private void OnDestroy()
    {
        // 注销事件
        if (health != null) health.OnDeath -= HandlePlayerDeath;

        EventBus.OnPlayerDisabled -= DisableAllComponents;
        EventBus.OnPlayerEnabled -= EnableAllComponents;

        // 清理输入系统
        playerInput.Dispose();
    }

    /// <summary>
    /// 处理死亡事件
    /// </summary>
    private void HandlePlayerDeath()
    {
        // 保存记录
        CharacterDataManager.Instance.UpdateRecord(
            playerData.playerType,
            WaveCounter.Instance.CurrentWave - 1 // CurrentWave从1开始
        );

        //通知总线游戏结束
        EventBus.TriggerPlayerDeath();
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
    }

    /// <summary>
    /// 重新全部按SO初始化
    /// </summary>
    public void ReInitialize()
    {
        // 重新初始化
        InitializeComponents();
    }

    public void SetPlayerData(PlayerSO data)
    {
        playerData = data;
        ReInitialize();  // 这会调用InitializeComponents()重新初始化所有组件
    }

    public PlayerType GetPlayerType() => playerData.playerType;

    #region 提供给外部访问的接口
    public PlayerHealth Health => health;
    public PlayerShooting Shooting => shooting;
    public PlayerMovement Movement => movement;
    public PlayerAbilities Abilities => abilities;
    public PlayerBuff Buff => buff;

    #endregion
}