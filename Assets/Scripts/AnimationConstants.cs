// AnimationConstants.cs
using UnityEngine;

public static class AnimationConstants
{
    // 基础动画剪辑名称（原始Animator Controller中使用）
    public static class Base
    {
        public const string EnemyIdle = "BaseEnemyIdle";
        public const string EnemyClash = "BaseEnemyClash";
        public const string PlayerIdle = "BasePlayerIdle";
        public const string PlayerShoot = "BasePlayerShoot";
    }

    // 玩家专用参数
    public static class Player
    {
        public const string Shooting = "Shoot"; // Bool参数名
    }

    public static class Enemy
    {
        public const string Clashing = "IsClashing"; // 新增冲撞状态参数
    }
}