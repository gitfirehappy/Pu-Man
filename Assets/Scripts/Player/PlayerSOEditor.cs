#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerSO))]
public class PlayerSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 基础字段
        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerType"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));

        // 显示各个配置部分
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("基础配置", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("healthConfig"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("shootingConfig"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("movementConfig"));

        // 根据技能类型显示对应配置
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("技能配置", EditorStyles.boldLabel);
        var abilitiesConfig = serializedObject.FindProperty("abilitiesConfig");

        EditorGUILayout.PropertyField(abilitiesConfig.FindPropertyRelative("startingAbility"));

        var abilityType = (AbilityType)abilitiesConfig.FindPropertyRelative("startingAbility").enumValueIndex;

        switch (abilityType)
        {
            case AbilityType.Classic:
                EditorGUILayout.PropertyField(abilitiesConfig.FindPropertyRelative("classicCooldownWaves"));
                EditorGUILayout.PropertyField(abilitiesConfig.FindPropertyRelative("classicDuration"));
                break;

            case AbilityType.Berserk:
                EditorGUILayout.PropertyField(abilitiesConfig.FindPropertyRelative("berserkCooldownWaves"));
                EditorGUILayout.PropertyField(abilitiesConfig.FindPropertyRelative("berserkDuration"));
                EditorGUILayout.PropertyField(abilitiesConfig.FindPropertyRelative("berserkFireRateMultiplier"));
                break;

            case AbilityType.Skilled:
                EditorGUILayout.PropertyField(abilitiesConfig.FindPropertyRelative("extraRefreshChancesPerWave"));
                break;

            case AbilityType.ChainKill:
                EditorGUILayout.PropertyField(abilitiesConfig.FindPropertyRelative("chainkillCooldownWaves"));
                break;

        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif