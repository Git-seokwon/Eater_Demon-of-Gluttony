using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Stage))]
public class StageEditor : IdentifiedObjectEditor
{
    private SerializedProperty stageRoomProperty;
    private SerializedProperty enemiesByWaveListProperty;
    private SerializedProperty waveEnemySpawnParametersProperty;
    private SerializedProperty stageBossProperty;

    protected override void OnEnable()
    {
        base.OnEnable();

        stageRoomProperty = serializedObject.FindProperty("stageRoom");
        enemiesByWaveListProperty = serializedObject.FindProperty("enemiesByWaveList");
        waveEnemySpawnParametersProperty = serializedObject.FindProperty("waveEnemySpawnParametersList");
        stageBossProperty = serializedObject.FindProperty("stageBoss");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        if (DrawFoldoutTitle("Setting"))
        {
            EditorGUILayout.PropertyField(stageRoomProperty);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(enemiesByWaveListProperty);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(waveEnemySpawnParametersProperty);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(stageBossProperty);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
