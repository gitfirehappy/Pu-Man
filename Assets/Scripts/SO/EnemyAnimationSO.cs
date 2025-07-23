using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAnimationSO",menuName = "Animation/EnemyAnimationSet")]
public class EnemyAnimationSO : ScriptableObject
{
    public EnemyType enemyType;
    public AnimationClip idleAnimation;
}
