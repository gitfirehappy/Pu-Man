using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float collisionDamage = 10f;
    public float collisionImmunityDuration = 1.5f;

    [SerializeField] private float currentHealth; // 显示在 Inspector 调试
    private float lastCollisionDamageTime;
    private bool isCollisionImmune => Time.time - lastCollisionDamageTime < collisionImmunityDuration;

    private void Start()
    {
        currentHealth = maxHealth; // 确保初始化
        lastCollisionDamageTime = -collisionImmunityDuration;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage! Current HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool TryTakeCollisionDamage(float damage)
    {
        if (isCollisionImmune) return false;

        TakeDamage(damage);
        lastCollisionDamageTime = Time.time;
        return true;
    }

    public float GetCollisionDamage()
    {
        return collisionDamage;
    }

    private void Die()
    {
        Debug.Log("Player died!");
        // 这里可以触发游戏结束逻辑
    }
}