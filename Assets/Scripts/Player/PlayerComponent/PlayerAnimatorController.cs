using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerAnimatorController : MonoBehaviour
{
    public Animator animator;
    public AssetReferenceT<PlayerAnimationSO> animationDataRef;

    private AnimatorOverrideController overrideController;
    private PlayerAnimationSO animationSO;
    private AnimationSetType currentSet = AnimationSetType.Normal;

    private readonly string[] clipNames = { "Idle", "Shoot", "Hurt" };

    public async void Init(PlayerType playerType)
    {
        // Addressables 异步加载动画资源
        animationSO = await animationDataRef.LoadAssetAsync().Task;
        ApplyAnimationSet(currentSet);
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
        overrideController["Hurt"] = set.hurt;
    }

    public void PlayShoot() => animator.SetTrigger("Shoot");
    public void PlayHurt() => animator.SetTrigger("Hurt");
}
