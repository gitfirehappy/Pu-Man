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
    /// 应用增益效果
    /// </summary>
    /// <param name="buff"></param>
    public void ApplyBuff(PlayerBuff buff)
    {
        buff.Apply(this);
    }

    /// <summary>
    /// 移除增益效果
    /// </summary>
    /// <param name="buff"></param>
    public void RemoveBuff(PlayerBuff buff)
    {
        buff.Remove(this);
    }


}