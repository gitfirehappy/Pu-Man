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
    private EnemyClash enemyClash;
    private bool lastClashState;

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

    private void Update()
    {
        if (PauseManager.Instance.IsPaused) return;

        // 检测冲撞状态变化
        if (enemyClash != null && enemyClash.IsClashing != lastClashState)
        {
            lastClashState = enemyClash.IsClashing;
            animator.SetBool(AnimationConstants.Enemy.Clashing, lastClashState);
        }
    }

    #region EnemyCore相关
    public void Initialize()
    {
        enemyClash = GetComponent<EnemyClash>(); // 获取冲撞组件

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
        // 设置基础动画
        overrideController[AnimationConstants.Base.EnemyIdle] = animationSO.idleAnimation;

        // 设置冲撞动画（如果存在）
        if (animationSO.clashAnimation != null)
        {
            overrideController[AnimationConstants.Base.EnemyClash] = animationSO.clashAnimation;
            Debug.Log("设置了冲撞动画");
        }
    }

    public void ResetToBaseStats()
    {
        // 重置动画参数
        if (animator != null)
        {
            animator.SetBool(AnimationConstants.Enemy.Clashing, false);
            animator.Play(AnimationConstants.Base.EnemyIdle, 0, 0f);
        }
    }
    #endregion
}
