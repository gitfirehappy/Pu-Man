using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerAnimatorController : MonoBehaviour
{
    public Animator animator;
    public string animationLabel = "PlayerAnimationSO"; // 统一的资源标签

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
                Debug.LogError("Animator component not found!");
            }
        }
    }

    public async void Init()
    {
        // 使用统一标签加载所有PlayerAnimationSO
        var loadHandle = Addressables.LoadAssetsAsync<PlayerAnimationSO>(animationLabel, null);
        await loadHandle.Task;

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            // 从加载的SO中查找与当前玩家类型匹配的
            var playerType = playerCore.GetPlayerType();
            animationSO = loadHandle.Result.FirstOrDefault(so => so.playerType == playerType);

            if (animationSO != null)
            {
                ApplyAnimationSet(currentSet);
            }
            else
            {
                Debug.LogError($"No animation data found for player type: {playerType}");
            }
        }
        else
        {
            Debug.LogError("Failed to load animation data!");
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
            if (isShooting) PlayShoot();
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
            Debug.LogWarning($"No animation set found for {setType}");
            return;
        }

        if (overrideController == null)
        {
            overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = overrideController;
        }

        overrideController["Idle"] = set.idle;
        overrideController["Shoot"] = set.shoot;
    }

    public void PlayShoot() => animator.SetTrigger("Shoot");

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
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }

        // 重置其他参数为默认值
        animator.Play("Idle", 0, 0f);
    }
}
