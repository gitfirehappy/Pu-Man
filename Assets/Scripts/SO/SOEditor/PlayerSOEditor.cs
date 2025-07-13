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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerSprite"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerPrefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));

        // 显示各个配置部分
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("基础配置", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("healthConfig"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("shootingConfig"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("movementConfig"));

        // 技能配置部分
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("技能配置", EditorStyles.boldLabel);
        var abilitiesConfig = serializedObject.FindProperty("abilitiesConfig");

        // 显示初始技能类型
        EditorGUILayout.PropertyField(abilitiesConfig.FindPropertyRelative("startingAbility"));

        // 显示AbilityData结构体
        EditorGUILayout.PropertyField(abilitiesConfig.FindPropertyRelative("startingAbilityData"), true);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif