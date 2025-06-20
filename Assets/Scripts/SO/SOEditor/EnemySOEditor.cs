#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemySO))]
public class EnemySOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 1. 始终显示的基础字段
        EditorGUILayout.PropertyField(serializedObject.FindProperty("enemyType"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("enemyPrefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("基础属性", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxHealth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("moveSpeed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("collisionDamage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("collisionImmunityDuration"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackRadius"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("detectionInterval"));

        // 2. 根据enemyType动态显示配置块
        var enemyType = (EnemyType)serializedObject.FindProperty("enemyType").enumValueIndex;
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("特殊配置", EditorStyles.boldLabel);

        switch (enemyType)
        {
            case EnemyType.Remote:
            case EnemyType.BigRemote:
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("shootingConfig"),
                    new GUIContent("射击配置"),
                    true);
                break;

            case EnemyType.Clash:
            case EnemyType.BigClash:
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("clashConfig"),
                    new GUIContent("冲撞配置"),
                    true);
                break;

            case EnemyType.Reward:
            case EnemyType.BigReward:
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("rewardConfig"),
                    new GUIContent("奖励配置"),
                    true);
                break;

            case EnemyType.Boss:
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("shootingConfig"),
                    new GUIContent("射击配置"),
                    true);
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("clashConfig"),
                    new GUIContent("冲撞配置"),
                    true);
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("rewardConfig"),
                    new GUIContent("奖励配置"),
                    true);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif