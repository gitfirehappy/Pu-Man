using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouseRotation : MonoBehaviour
{
    public bool isRotating = true;

    private void Update()
    {
        if (isRotating)
        {
            RotateTowardsMouse();
        }
    }

    private void RotateTowardsMouse()
    {
        // 获取鼠标位置（屏幕坐标）
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();

        // 将鼠标位置从屏幕坐标转换为世界坐标
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;  // 确保在 2D 平面

        // 计算方向向量
        Vector3 direction = mouseWorldPos - transform.position;
        direction.z = 0f;
        direction.Normalize();

        // 计算角度并设置旋转（Z 轴朝向）
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
