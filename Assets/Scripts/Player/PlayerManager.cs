using UnityEngine;

public class PlayerManager : SingletonMono<PlayerManager>
{
    public PlayerCore Player { get; private set; }

    public PlayerType LastPlayerType { get; private set; }

    private System.Action<PlayerCore> onPlayerRegistered;

    /// <summary>
    /// 将Player注册到PlayerManager
    /// </summary>
    /// <param name="player"></param>
    public void RegisterPlayer(PlayerCore player)
    {
        if (Player != null && Player != player)
        {
            Debug.LogWarning("Multiple players detected! Overwriting with new player.");
            Destroy(Player.gameObject);
        }
        Player = player;
        LastPlayerType = player.GetPlayerType();
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

    /// <summary>
    /// 清空PlayerManager对Player注册
    /// </summary>
    /// <param name="target"></param>
    public void ClearPlayer(PlayerCore target)
    {
        if (Player == target)
        {
            Player = null;
        }
    }
}