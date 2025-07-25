using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerMovement : MonoBehaviour
{
    [Header("当前移速")] public float currentRunSpeed;

    private Vector2 inputDirection;
    private Rigidbody2D rb;
    private PlayerInput inputControl;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputControl = new PlayerInput(); // 自动生成的输入类
    }
    private void OnEnable() => inputControl?.Enable();
    private void OnDisable() => inputControl?.Disable();

    private void Update()
    {
        inputDirection = inputControl.Player.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * currentRunSpeed, inputDirection.y * currentRunSpeed); // 四向
    }

}
