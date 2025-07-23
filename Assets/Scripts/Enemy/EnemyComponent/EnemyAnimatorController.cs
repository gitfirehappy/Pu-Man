using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class EnemyAnimatorController : MonoBehaviour
{
    public Animator animator;
    public string animationLabel = "EnemyAnimationSO"; // 资源标签

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
                Debug.LogError("Animator component not found on enemy!");
            }
        }
    }

    public async void Initialize()
    {
        // 使用统一标签加载所有EnemyAnimationSO
        var loadHandle = Addressables.LoadAssetsAsync<EnemyAnimationSO>(animationLabel, null);
        await loadHandle.Task;

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            // 查找匹配当前敌人类型的SO
            var enemyType = enemyCore.EnemyType;
            animationSO = loadHandle.Result.FirstOrDefault(so => so.enemyType == enemyType);

            if (animationSO != null)
            {
                ApplyAnimationSet();
            }
            else
            {
                Debug.LogWarning($"No animation data found for enemy type: {enemyType}");
            }
        }
        else
        {
            Debug.LogError("Failed to load enemy animation data!");
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
