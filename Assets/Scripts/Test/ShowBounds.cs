using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBounds : MonoBehaviour
{
    [Header("边界设置")]
    public float width = 10f;    // 边界宽度
    public float height = 5f;    // 边界高度
    public Color gizmoColor = Color.green; // 边界颜色

    void OnDrawGizmos()
    {
        // 设置Gizmo颜色
        Gizmos.color = gizmoColor;

        // 计算边界四个角的位置
        Vector3 topLeft = transform.position + new Vector3(-width / 2, height / 2, 0);
        Vector3 topRight = transform.position + new Vector3(width / 2, height / 2, 0);
        Vector3 bottomLeft = transform.position + new Vector3(-width / 2, -height / 2, 0);
        Vector3 bottomRight = transform.position + new Vector3(width / 2, -height / 2, 0);

        // 绘制四条边
        Gizmos.DrawLine(topLeft, topRight);    // 上边
        Gizmos.DrawLine(topRight, bottomRight);// 右边
        Gizmos.DrawLine(bottomRight, bottomLeft); // 下边
        Gizmos.DrawLine(bottomLeft, topLeft);  // 左边
    }

}
