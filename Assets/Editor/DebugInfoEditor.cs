using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugInfo))]
public class DebugInfoEditor : Editor
{
    private bool showReferences = false;
    private SerializedProperty m_FPSMeterReferenceProperty;
    private SerializedProperty m_GraphicsAPIReferenceProperty;

    private bool showSettings = true;
    private SerializedProperty m_FPSMeterSettingProperty;

    private void OnEnable()
    {
        m_FPSMeterReferenceProperty = serializedObject.FindProperty("m_FPSMeterText");
        m_GraphicsAPIReferenceProperty = serializedObject.FindProperty("m_GraphicsAPIText");

        m_FPSMeterSettingProperty = serializedObject.FindProperty("m_UseFpsMeter");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        showReferences = EditorGUILayout.Foldout(showReferences, "References");
        if (showReferences)
        {
            EditorGUILayout.PropertyField(m_FPSMeterReferenceProperty, new GUIContent("FPSMeterRef"));
            EditorGUILayout.PropertyField(m_GraphicsAPIReferenceProperty, new GUIContent("GraphicsAPIRef"));
        }

        showSettings = EditorGUILayout.Foldout(showSettings, "Settings");
        if (showSettings)
        {
            EditorGUILayout.PropertyField(m_FPSMeterSettingProperty, new GUIContent("Use FPS Meter"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
