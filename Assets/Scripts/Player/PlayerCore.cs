using UnityEngine;

[RequireComponent(typeof(PlayerHealth), typeof(PlayerShooting), typeof(PlayerMovement))]
public class PlayerCore : MonoBehaviour
{
    private PlayerHealth healthSystem;
    private PlayerShooting shootingSystem;
    private PlayerMovement movementSystem;
    private PlayerAbilities abilitySystem;


    private void Awake()
    {
        healthSystem = GetComponent<PlayerHealth>();
        shootingSystem = GetComponent<PlayerShooting>();
        movementSystem = GetComponent<PlayerMovement>();
        abilitySystem = GetComponent<PlayerAbilities>();

    }


    // 提供给外部访问的接口
    public PlayerHealth Health => healthSystem;
    public PlayerShooting Shooting => shootingSystem;
    public PlayerMovement Movement => movementSystem;
    public PlayerAbilities Abilities => abilitySystem;

    /// <summary>
    /// 应用Buff效果（新版本）
    /// </summary>
    public void ApplyBuff(BuffSO buffData)
    {
        buffData.Apply(this); // 直接调用BuffSO的Apply
    }

    /// <summary>
    /// 移除Buff效果（新版本）
    /// </summary>
    public void RemoveBuff(BuffSO buffData)
    {
        buffData.Remove(this); // 直接调用BuffSO的Remove
    }


}