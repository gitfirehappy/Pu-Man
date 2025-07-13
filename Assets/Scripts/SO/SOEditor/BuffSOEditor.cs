#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuffSO))]
public class BuffSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 1. 始终显示的基础字段
        EditorGUILayout.PropertyField(serializedObject.FindProperty("buffID"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rarity"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("buffPicture"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Description"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isUnique"));

        // 2. 根据buffID动态显示数值字段
        var buffID = (BuffID)serializedObject.FindProperty("buffID").enumValueIndex;
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("数值配置", EditorStyles.boldLabel);

        switch (buffID)
        {
            //普通
            case BuffID.MaxHealthUp:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("healthModifier"),
                    new GUIContent("生命值增加"));
                break;

            case BuffID.ArmorUp:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("armorModifier"),
                    new GUIContent("护甲增加"));
                break;

            case BuffID.HealthRegenUp:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("healthRegenModifier"),
                    new GUIContent("生命恢复增加"));
                break;

            case BuffID.DogeChanceUp:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("dodgeChanceModifier"),
                    new GUIContent("闪避率增加"));
                break;

            case BuffID.CollitionDamageUp:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("collisionDamageModifier"),
                    new GUIContent("碰撞伤害增加"));
                break;

            case BuffID.FireDamageUp:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("damageModifier"),
                    new GUIContent("子弹伤害增加"));
                break;

            case BuffID.FireRateUp:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("fireRateModifier"),
                    new GUIContent("射速提升"));
                break;

            case BuffID.KnockbackUp:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("knockbackModifier"),
                    new GUIContent("击退幅度提升"));
                break;

            case BuffID.ProjectileCountUp:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileCountModifier"),
                    new GUIContent("增加弹道"));
                break;

            case BuffID.ProjectileSizeUp:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileSizeModifier"),
                    new GUIContent("增加子弹大小"));
                break;

            case BuffID.SpeedUp:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("speedModifier"),
                    new GUIContent("增加移速"));
                break;

            //稀有

            case BuffID.ExtraBuff:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("extraBuffChoices"),
                    new GUIContent("增加下次选择buff数量"));
                break;

            case BuffID.ExtraRefreshChance:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("extraRefreshChance"),
                    new GUIContent("增加刷新次数"));
                break;

            case BuffID.ReduceAbilityCooldown:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("reduceAbilityCooldown"),
                    new GUIContent("减少技能冷却"));
                break;


            //史诗

            case BuffID.CheatDeath:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cheatDeathInvisibleTime"));
                break;

            //case BuffID.AoeShot:
            //    EditorGUILayout.PropertyField(serializedObject.FindProperty("aoeShot"),
            //        new GUIContent("范围伤害"));
            //    break;

            case BuffID.ChainKillSkill:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("chainKillCooldown"));
                break;

            //case BuffID.ChangeSkill:
            //    EditorGUILayout.PropertyField(serializedObject.FindProperty("randomizeAbility"),
            //        new GUIContent("替换为随机技能"));
            //    break;

            case BuffID.AllNormalBuff:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("allNormalBuffModifier"));
                break;

            //传说

            case BuffID.ReduceEnemy:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("reduceEnemySpawn"));
                break;

            //case BuffID.HealthToArmor:
            //    EditorGUILayout.PropertyField(serializedObject.FindProperty("healthToArmor"),
            //        new GUIContent("血量转护甲"));
            //    break;

            case BuffID.Barserk:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("amorToDamageModifier"));
                break;

        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif