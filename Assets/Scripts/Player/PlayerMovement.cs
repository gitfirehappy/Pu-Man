using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float runSpeed = 5f;

    private Vector2 inputDirection;
    private Rigidbody2D rb;
    private PlayerInput inputControl;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputControl = new PlayerInput(); // 自动生成的输入类
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
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
    /// 人物移动
    /// </summary>
    public void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * runSpeed, inputDirection.y * runSpeed); // 四向
    }

    public void AddSpeed(float amount) => runSpeed += amount;
}
