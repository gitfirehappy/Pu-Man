// AnimationConstants.cs
using UnityEngine;

public static class AnimationConstants
{
    // 基础动画剪辑名称（原始Animator Controller中使用）
    public static class Base
    {
        public const string EnemyIdle = "BaseEnemyIdle";
        public const string PlayerIdle = "BasePlayerIdle";
        public const string PlayerShoot = "BasePlayerShoot";
    }

    // 玩家专用参数
    public static class Player
    {
        public const string Shooting = "Shoot"; // Bool参数名
    }
}