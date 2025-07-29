using UnityEngine;

public class PlayerManager : SingletonMono<PlayerManager>
{
    public PlayerCore Player { get; private set; }
    private System.Action<PlayerCore> onPlayerRegistered;

    protected override void Init()
    {
        // 初始时尝试查找Player（可选）
        //Player = FindObjectOfType<PlayerCore>();
    }

    public void RegisterPlayer(PlayerCore player)
    {
        if (Player != null && Player != player)
        {
            Debug.LogWarning("Multiple players detected! Overwriting with new player.");
            Destroy(Player.gameObject);
        }
        Player = player;
        onPlayerRegistered?.Invoke(player);
        onPlayerRegistered = null; // 调用后清空
    }

    /// <summary>
    /// 获取Player或等待注册
    /// </summary>
    /// <param name="callback"></param>
    public void GetPlayerAsync(System.Action<PlayerCore> callback)
    {
        if (Player != null)
        {
            callback(Player);
        }
        else
        {
            onPlayerRegistered += callback;
        }
    }

    public void ClearPlayer(PlayerCore target)
    {
        if (Player == target)
        {
            Player = null;
        }
    }

}