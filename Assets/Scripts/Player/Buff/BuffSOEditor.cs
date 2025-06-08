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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Description"));

        // 2. 根据buffID动态显示数值字段
        var buffID = (BuffID)serializedObject.FindProperty("buffID").enumValueIndex;
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("数值配置", EditorStyles.boldLabel);

        switch (buffID)
        {
            case BuffID.MaxHealthUp:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("healthModifier"),
                    new GUIContent("生命值增加量"));
                break;

            case BuffID.ArmorUp:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("armorModifier"),
                    new GUIContent("护甲增加量"));
                break;

            case BuffID.FireRateUp:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("fireRateModifier"),
                    new GUIContent("射速提升"));
                break;

            case BuffID.CheatDeath:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("grantCheatDeath"),
                    new GUIContent("启用名刀效果"));
                break;

                // 添加其他类型...
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif