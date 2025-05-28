using UnityEngine;
using Cinemachine;


[RequireComponent(typeof(CinemachineConfiner2D))]
public class CameraConfiner2D : MonoBehaviour
{
    public PolygonCollider2D cameraBounds; // 拖入你设置的边界

    private CinemachineConfiner2D confiner;

    private void Awake()
    {
        confiner = GetComponent<CinemachineConfiner2D>();
        confiner.m_BoundingShape2D = cameraBounds;
        confiner.InvalidateCache(); // 重新计算路径
    }
}
