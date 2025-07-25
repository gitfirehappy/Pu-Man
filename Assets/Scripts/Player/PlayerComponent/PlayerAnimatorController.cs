using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerAnimatorController : MonoBehaviour
{
    public Animator animator;

    private AnimatorOverrideController overrideController;
    private PlayerAnimationSO animationSO;
    private AnimationSetType currentSet = AnimationSetType.Normal;

    // 状态检测变量
    private bool isShooting;
    private bool isSkilling;
    private PlayerCore playerCore;

    private void Awake()
    {
        playerCore = GetComponent<PlayerCore>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("未找到玩家的Animator组件!");
            }
        }
    }

    public void Init()
    {
        // 直接从DataManager获取SO
        var playerType = playerCore.GetPlayerType();
        animationSO = DataManager.Instance.GetPlayerAnimationSO(playerType);

        if (animationSO != null)
        {
            ApplyAnimationSet(currentSet);
        }
        else
        {
            Debug.LogError($"未找到玩家类型的动画数据: {playerType}");
        }
    }

    private void Update()
    {
        if (PauseManager.Instance.IsPaused) return;

        // 实时检测玩家状态
        CheckPlayerState();
    }

    private void CheckPlayerState()
    {
        // 检测技能状态
        bool newSkilling = playerCore.Abilities.isAbilityActive;

        // 检测射击状态
        bool newShooting = playerCore.Shooting.IsShooting;

        // 状态变化处理
        if (newSkilling != isSkilling)
        {
            isSkilling = newSkilling;
            SetSkillMode(isSkilling);
        }

        if (newShooting != isShooting)
        {
            isShooting = newShooting;
            SetShootingState(isShooting);
        }
    }

    public void SetSkillMode(bool isSkillMode)
    {
        var nextSet = isSkillMode ? AnimationSetType.SkillMode : AnimationSetType.Normal;
        if (nextSet != currentSet)
        {
            currentSet = nextSet;
            ApplyAnimationSet(currentSet);
        }
    }

    private void ApplyAnimationSet(AnimationSetType setType)
    {
        var set = animationSO.GetSet(setType);
        if (set == null)
        {
            Debug.LogWarning($"没找到 {setType} 的AnimationSet");
            return;
        }

        if (overrideController == null)
        {
            overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = overrideController;
        }

        overrideController[AnimationConstants.Base.PlayerIdle] = set.idle;
        overrideController[AnimationConstants.Base.PlayerShoot] = set.shoot;
    }

    private void SetShootingState(bool isShooting)
    {
        if (animator != null)
        {
            animator.SetBool(AnimationConstants.Player.Shooting, isShooting);
        }
    }

    public void ResetToBaseStats()
    {
        // 重置到默认动画集
        currentSet = AnimationSetType.Normal;
        ApplyAnimationSet(currentSet);

        // 重置所有动画参数
        ResetAnimatorParameters();
    }

    private void ResetAnimatorParameters()
    {
        if (animator == null) return;

        // 重置所有触发器
        foreach (var param in animator.parameters)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Trigger:
                    animator.ResetTrigger(param.name);
                    break;
                case AnimatorControllerParameterType.Bool:
                    if (param.name == AnimationConstants.Player.Shooting)
                    {
                        animator.SetBool(param.name, false);
                    }
                    break;
            }
        }

        // 重置其他参数为默认值
        animator.Play("Idle", 0, 0f);
    }
}
