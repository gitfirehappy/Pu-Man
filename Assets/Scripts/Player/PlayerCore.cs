using UnityEngine.Pool;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerCore : MonoBehaviour
{
    [SerializeField] private PlayerSO playerData;

    private void Awake()
    {
        if (playerData == null)
        {
            Debug.LogError("PlayerSO not assigned!");
            return;
        }

        InitializeComponents();
    }

    private void InitializeComponents()
    {
        // 确保所有必须组件存在
        if (!TryGetComponent<PlayerHealth>(out var health))
            health = gameObject.AddComponent<PlayerHealth>();

        if (!TryGetComponent<PlayerShooting>(out var shooting))
            shooting = gameObject.AddComponent<PlayerShooting>();

        if (!TryGetComponent<PlayerMovement>(out var movement))
            movement = gameObject.AddComponent<PlayerMovement>();

        if (!TryGetComponent<PlayerAbilities>(out var abilities))
            abilities = gameObject.AddComponent<PlayerAbilities>();

        // 初始化所有组件
        GetComponent<PlayerHealth>().Initialize(playerData);
        GetComponent<PlayerShooting>().Initialize(playerData);
        GetComponent<PlayerMovement>().Initialize(playerData);
        GetComponent<PlayerAbilities>().Initialize(playerData);
    }

    /// <summary>
    /// 恢复状态,每波开始前调用
    /// </summary>
    public void ResetState()
    {
        // 重置状态（给每波开始前可以调用，主要是回血和重置临时增益）
        GetComponent<PlayerHealth>().ResetToBaseStats();
        GetComponent<PlayerShooting>().ResetToBaseStats();
        GetComponent<PlayerMovement>().ResetToBaseStats();
    }

    /// <summary>
    /// 重新全部按SO初始化
    /// </summary>
    public void ReInitialize()
    {
        // 重新初始化
        InitializeComponents();
    }

    public PlayerType GetPlayerType() => playerData.playerType;

    // 提供给外部访问的接口
    public PlayerHealth Health => GetComponent<PlayerHealth>();
    public PlayerShooting Shooting => GetComponent<PlayerShooting>();
    public PlayerMovement Movement => GetComponent<PlayerMovement>();
    public PlayerAbilities Abilities => GetComponent<PlayerAbilities>();
}