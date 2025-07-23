using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAnimationSO",menuName = "AnimationSet/EnemyAnimationSet")]
public class EnemyAnimationSO : ScriptableObject
{
    public EnemyType enemyType;
    public AnimationClip idleAnimation;
}
