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
    private SerializedProperty stageClearCountProperty;
    private SerializedProperty enemiesByWaveListProperty;
    private SerializedProperty eliteEnemiesByWaveListProperty;
    private SerializedProperty waveEnemySpawnParametersProperty;
    private SerializedProperty stageBossProperty;
    private SerializedProperty itemDropRateProperty;
    private SerializedProperty stageEnterMusicProperty;
    private SerializedProperty waveStartMusicProperty;
    private SerializedProperty berserkMusicProperty;
    private SerializedProperty clearMusicProperty;
    private SerializedProperty defeatMusicProperty;

    protected override void OnEnable()
    {
        base.OnEnable();

        stageNumberProperty = serializedObject.FindProperty("stageNumber");
        stageRoomProperty = serializedObject.FindProperty("stageRoom");
        stageRoomPostionProperty = serializedObject.FindProperty("stageRoomPostion");
        stageClearCountProperty = serializedObject.FindProperty("clearCount");
        enemiesByWaveListProperty = serializedObject.FindProperty("enemiesByWaveList");
        eliteEnemiesByWaveListProperty = serializedObject.FindProperty("eliteEnemiesByWaveList");
        waveEnemySpawnParametersProperty = serializedObject.FindProperty("waveEnemySpawnParametersList");
        stageBossProperty = serializedObject.FindProperty("stageBoss");
        itemDropRateProperty = serializedObject.FindProperty("itemDropRate");

        stageEnterMusicProperty = serializedObject.FindProperty("stageEnterMusic");
        waveStartMusicProperty = serializedObject.FindProperty("waveStartMusic");
        berserkMusicProperty = serializedObject.FindProperty("berserkMusic");
        clearMusicProperty = serializedObject.FindProperty("clearMusic");
        defeatMusicProperty = serializedObject.FindProperty("defeatMusic");
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
            EditorGUILayout.PropertyField(stageClearCountProperty);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(enemiesByWaveListProperty);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(eliteEnemiesByWaveListProperty);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(waveEnemySpawnParametersProperty);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(stageBossProperty);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(itemDropRateProperty);
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(stageEnterMusicProperty);
            EditorGUILayout.PropertyField(waveStartMusicProperty);
            EditorGUILayout.PropertyField(berserkMusicProperty);
            EditorGUILayout.PropertyField(clearMusicProperty);
            EditorGUILayout.PropertyField(defeatMusicProperty);

        }

        serializedObject.ApplyModifiedProperties();
    }
}
