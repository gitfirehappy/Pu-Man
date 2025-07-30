using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("相机设置")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        // 确保有虚拟相机组件
        if (virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            if (virtualCamera == null)
            {
                Debug.LogError("未找到CinemachineVirtualCamera组件！");
                return;
            }
        }

        // 注册到Battle状态事件队列
        EventQueueManager.AddStateEvent(GameState.Battle, SetupCameraFollow, 1);
    }

    private void SetupCameraFollow()
    {
        if (virtualCamera.Follow != null) return;

        // 异步获取玩家实例
        PlayerManager.Instance.GetPlayerAsync(player =>
        {
            if (player != null)
            {
                // 设置相机跟随玩家
                virtualCamera.Follow = player.transform;
                Debug.Log($"摄像机已跟随玩家: {player.name}");
            }
            else
            {
                Debug.LogWarning("找不到有效的玩家实例！");
            }
        });
    }
}