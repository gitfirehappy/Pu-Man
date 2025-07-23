using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class EnemyAnimatorController : MonoBehaviour
{
    public Animator animator;

    private AnimatorOverrideController overrideController;
    private EnemyAnimationSO animationSO;

    private EnemyCore enemyCore;

    private void Awake()
    {
        enemyCore = GetComponent<EnemyCore>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("未找到敌人的Animator组件!");
            }
        }
    }

    public void Initialize()
    {
        // 直接从DataManager获取SO
        var enemyType = enemyCore.EnemyType;
        animationSO = DataManager.Instance.GetEnemyAnimationSO(enemyType);

        if (animationSO != null)
        {
            ApplyAnimationSet();
        }
        else
        {
            Debug.LogWarning($"未找到敌方类型的动画数据: {enemyType}");
        }
    }

    private void ApplyAnimationSet()
    {
        if (animationSO == null) return;

        if (overrideController == null)
        {
            overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = overrideController;
        }

        // 只需要设置Idle动画
        if (animationSO.idleAnimation != null)
        {
            overrideController["Idle"] = animationSO.idleAnimation;
        }
    }

    public void ResetToBaseStats()
    {
        // 重置动画参数
        if (animator != null)
        {
            animator.Play("Idle", 0, 0f);
        }
    }
}
