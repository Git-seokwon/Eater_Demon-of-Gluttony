using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Stage))]
public class StageEditor : IdentifiedObjectEditor
{
    private SerializedProperty stageNumberProperty;
    private SerializedProperty stageRoomProperty;
    private SerializedProperty stageRoomPostionProperty;
    private SerializedProperty enemiesByWaveListProperty;
    private SerializedProperty eliteEnemiesByWaveListProperty;
    private SerializedProperty waveEnemySpawnParametersProperty;
    private SerializedProperty stageBossProperty;

    protected override void OnEnable()
    {
        base.OnEnable();

        stageNumberProperty = serializedObject.FindProperty("stageNumber");
        stageRoomProperty = serializedObject.FindProperty("stageRoom");
        stageRoomPostionProperty = serializedObject.FindProperty("stageRoomPostion");
        enemiesByWaveListProperty = serializedObject.FindProperty("enemiesByWaveList");
        eliteEnemiesByWaveListProperty = serializedObject.FindProperty("eliteEnemiesByWaveList");
        waveEnemySpawnParametersProperty = serializedObject.FindProperty("waveEnemySpawnParametersList");
        stageBossProperty = serializedObject.FindProperty("stageBoss");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        if (DrawFoldoutTitle("Setting"))
        {
            EditorGUILayout.PropertyField(stageNumberProperty);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(stageRoomProperty);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(stageRoomPostionProperty);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(enemiesByWaveListProperty);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(eliteEnemiesByWaveListProperty);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(waveEnemySpawnParametersProperty);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(stageBossProperty);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
