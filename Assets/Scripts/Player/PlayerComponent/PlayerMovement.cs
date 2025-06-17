using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private float baserunSpeed;

    [SerializeField][Header("当前移速")]private float currentRunSpeed;

    private Vector2 inputDirection;
    private Rigidbody2D rb;
    private PlayerInput inputControl;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputControl = new PlayerInput(); // 自动生成的输入类
    }

    private void Update()
    {
        inputDirection = inputControl.Player.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// 玩家移动系统初始化
    /// </summary>
    /// <param name="playerData"></param>
    public void Initialize(PlayerSO playerData)
    {
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

    public void DisableMovement()
    {
        rb.velocity = Vector2.zero; // 立即停止移动
    }

    public void EnableMovement()
    {

    }

    /// <summary>
    /// 人物移动
    /// </summary>
    public void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * currentRunSpeed, inputDirection.y * currentRunSpeed); // 四向
    }


    #region 公共属性
    public float RunSpeed => baserunSpeed;

    #endregion

    #region 增益接口
    public void AddSpeed(float amount) => baserunSpeed += amount;

    #endregion

}
