using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyHealth : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public Collider2D _collider;
    public Transform playerTransform;
    public SpriteRenderer spriteRenderer;

    [Header("基础属性")]
    [Header("血量上限")] public float maxHealth;
    [Header("碰撞伤害")] public float collisionDamage;
    [Header("受击无敌")] public float collisionImmunityDuration;
    [Header("碰撞范围")] public float attackRadius;
    [Header("检测间隔")] public float detectionInterval;

    [Header("当前状态")]
    public float currentHealth;
    public bool _isDead = false;

    [Header("计时器")]
    public float lastCollisionDamageTime;
    public float lastDetectionTime;

    public bool isCollisionImmune => Time.time - lastCollisionDamageTime < collisionImmunityDuration;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _isDead = false;
        if (_collider != null) _collider.enabled = true;
        if (rb != null) rb.simulated = true;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        currentHealth = maxHealth;
        lastCollisionDamageTime = -collisionImmunityDuration;
    }

    /// <summary>
    /// 刷新血量系统状态
    /// </summary>
    public void ResetToBaseStats()
    {
        _isDead = false;
        if (_collider != null) _collider.enabled = true;
        if (rb != null) rb.simulated = true;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        currentHealth = maxHealth;
        lastCollisionDamageTime = -collisionImmunityDuration;
    }

    private void Update()
    {
        if (playerTransform == null || Time.time - lastDetectionTime < detectionInterval)
            return;

        DetectAndDamagePlayer();
        lastDetectionTime = Time.time;
    }

    private void DetectAndDamagePlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(
            transform.position,
            attackRadius,
            LayerMask.GetMask("Player"));

        if (playerCollider != null &&
            playerCollider.TryGetComponent<PlayerHealth>(out var playerHealth))
        {
            // 对玩家造成伤害
            playerHealth.TryTakeCollisionDamage(collisionDamage);

            // 敌人受到反伤（带无敌帧检查）
            TryTakeCollisionDamage(playerHealth.GetCollisionDamage());
        }
    }

    /// <summary>
    /// 敌人受伤
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage, DamageSource source)
    {
        if (_isDead) return;

        currentHealth -= damage;
        Debug.Log($"{source} caused {damage} damage to Enemy! Current HP: {currentHealth}");

        StartCoroutine(HitAnimationRoutine());

        if (currentHealth <= 0)
        {
            Die(source);
        }
    }

    private IEnumerator HitAnimationRoutine()
    {
        if (spriteRenderer == null) yield break;

        float duration = 0.3f; // 比Player更短
        float blinkSpeed = 0.07f; // 更快的闪烁频率
        float timer = 0;

        while (timer < duration)
        {
            // 使用较暗的半透明效果（alpha=0.5）
            spriteRenderer.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
            yield return new WaitForSeconds(blinkSpeed);

            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(blinkSpeed);

            timer += blinkSpeed * 2;
        }

        // 确保最终状态正常
        spriteRenderer.color = Color.white;
    }

    public bool TryTakeCollisionDamage(float damage)
    {
        if (isCollisionImmune) return false;

        TakeDamage(damage, DamageSource.Player);
        lastCollisionDamageTime = Time.time;
        return true;
    }

    /// <summary>
    /// 敌人死亡
    /// </summary>
    private void Die(DamageSource source)
    {
        if (_isDead || this == null) return;
        _isDead = true;

        // 立即禁用所有相关组件
        if (_collider != null) _collider.enabled = false;
        if (rb != null) rb.simulated = false;

        Debug.Log("敌人死亡", this);
        gameObject.SetActive(false);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

}
