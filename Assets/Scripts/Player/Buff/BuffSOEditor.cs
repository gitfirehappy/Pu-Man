#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuffSO))]
public class BuffSOEditor : Editor
{
    private SerializedProperty _buffIDProp;
    private SerializedProperty _effectDataProp;

    private void OnEnable()
    {
        _buffIDProp = serializedObject.FindProperty("buffID");
        _effectDataProp = serializedObject.FindProperty("_effectData");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 绘制基础字段
        EditorGUILayout.PropertyField(_buffIDProp);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rarity"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Description"));

        // 处理EffectData
        if (_effectDataProp.objectReferenceValue == null)
        {
            if (GUILayout.Button("创建效果数据"))
            {
                CreateEffectData();
            }
        }
        else
        {
            EditorGUILayout.PropertyField(_effectDataProp);
            DrawDynamicFields();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void CreateEffectData()
    {
        // 创建新的PlayerBuff实例
        var newEffect = ScriptableObject.CreateInstance<PlayerBuff>();
        newEffect.name = "EffectData_" + _buffIDProp.enumNames[_buffIDProp.enumValueIndex];

        // 设置为子资产
        AssetDatabase.AddObjectToAsset(newEffect, target);
        _effectDataProp.objectReferenceValue = newEffect;
        AssetDatabase.SaveAssets();
    }

    private void DrawDynamicFields()
    {
        var effectData = (PlayerBuff)_effectDataProp.objectReferenceValue;
        if (effectData == null) return;

        var effectSO = new SerializedObject(effectData);
        effectSO.Update();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("效果配置", EditorStyles.boldLabel);

        switch ((BuffID)_buffIDProp.enumValueIndex)
        {
            case BuffID.MaxHealthUp:
                EditorGUILayout.PropertyField(effectSO.FindProperty("healthModifier"),
                    new GUIContent("生命值增加"));
                break;

            case BuffID.ArmorUp:
                EditorGUILayout.PropertyField(effectSO.FindProperty("armorModifier"),
                    new GUIContent("护甲增加"));
                break;

                // 添加其他Buff类型的配置...
        }

        effectSO.ApplyModifiedProperties();
    }
}
#endif