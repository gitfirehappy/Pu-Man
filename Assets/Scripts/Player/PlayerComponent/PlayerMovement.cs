using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("当前属性")]
    [Tooltip("当前移速")][SerializeField]private float currentRunSpeed;

    [Header("基础属性")]
    [Tooltip("基础移速")][SerializeField] private float baserunSpeed;

    private Vector2 inputDirection;
    private Rigidbody2D rb;
    private PlayerCore playerCore;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerCore = GetComponent<PlayerCore>();
    }

    private void Update()
    {
        if (PauseManager.Instance.IsPaused)
        {
            inputDirection = Vector2.zero;
            return;
        }

        inputDirection = playerInput.Player.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    #region PlayerCore相关
    public void EnableMovement()
    {

    }

    public void DisableMovement()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero; // 立即停止移动
    }

    /// <summary>
    /// 玩家移动系统初始化
    /// </summary>
    /// <param name="playerData"></param>
    public void Initialize(PlayerSO playerData)
    {
        if (playerCore != null && playerCore.playerInput != null)
        {
            playerInput = playerCore.playerInput;
        }
        else
        {
            Debug.LogError("PlayerCore 或 PlayerInput 未正确初始化！");
        }

        rb = GetComponent<Rigidbody2D>();

        baserunSpeed = playerData.movementConfig.runSpeed;

        ResetToBaseStats();
    }

    /// <summary>
    /// 恢复移速状态
    /// </summary>
    public void ResetToBaseStats()
    {
        currentRunSpeed = baserunSpeed;
    }
    #endregion

    /// <summary>
    /// 人物移动
    /// </summary>
    public void Move()
    {
        rb.linearVelocity = new Vector2(inputDirection.x * currentRunSpeed, inputDirection.y * currentRunSpeed); // 四向
    }


    #region 公共属性
    public float RunSpeed => baserunSpeed;

    #endregion

    #region 增益接口
    public void AddSpeed(float amount) => baserunSpeed += amount;

    #endregion

}
