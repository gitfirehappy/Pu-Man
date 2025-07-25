using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAnimationSO",menuName = "AnimationSet/PlayerAnimationSet")]
public class PlayerAnimationSO : ScriptableObject
{
    public PlayerType playerType;
    public List<AnimationSetEntry> animationSets;

    [System.Serializable]
    public class AnimationSetEntry
    {
        public AnimationSetType setType;
        public AnimationClip idle;
        public AnimationClip shoot;
    }

    private Dictionary<AnimationSetType, AnimationSetEntry> _cache;

    public AnimationSetEntry GetSet(AnimationSetType type)
    {
        if (_cache == null)
        {
            _cache = new Dictionary<AnimationSetType, AnimationSetEntry>();
            foreach (var set in animationSets)
            {
                _cache[set.setType] = set;
            }
        }

        return _cache.TryGetValue(type, out var entry) ? entry : null;
    }
}

public enum AnimationSetType
{
    Normal,
    SkillMode,
    // Future: BuffMode, BerserkMode...
}
