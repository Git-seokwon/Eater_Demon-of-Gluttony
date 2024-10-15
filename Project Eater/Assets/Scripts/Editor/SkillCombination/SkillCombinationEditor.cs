using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNodeEditor;
using XNode;

// SkillTree�� IO ��ü�̱� ������ IdentifiedObjectEditor�� ��ӹ��� 
[CustomEditor(typeof(SkillCombination))] // SkillCombination�̶�� Ŭ������ �ν����Ϳ��� �� �����Ͱ� Ȱ��ȭ
public class SkillCombinationEditor : IdentifiedObjectEditor
{
    private SerializedProperty graphProperty;

    protected override void OnEnable()
    {
        base.OnEnable();

        // SkillCombination�� graph �ʵ�
        graphProperty = serializedObject.FindProperty("graph");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // ����ȭ�� ��ü�� �ֽ� ���¸� �������� 
        serializedObject.Update();

        // graph�� ������ �ڵ����� �������
        if (graphProperty.objectReferenceValue == null)
        {
            // �� serializedObject.targetObject : ���� �ν����Ϳ� ����� SkillCombination ��ü
            var targetObject = serializedObject.targetObject; 
            // Graph�� �⺻������ Scriptable Object�̱� ������ CreateInstance�� SkillTreeGraph�� ����
            var newGraph = CreateInstance<SkillCombinationGraph>();
            newGraph.name = "Skill Tree Graph";

            // Graph�� Scriptable Object�̱� ������ Project�� Asset���� �������� ������ �׳� �����������. 
            // Asset���� ������ ��, �ƹ� ������ �����ع����� SkillCombination Asset�� SkillCombination Asset�� ����ϴ� Graph Asset
            // �̷��� �� ���� Asset�� ���� �����ؾ� �ϴ� �������� �����. 
            // �� Unity���� Asset�� �ٸ� Asset�� ���� Asset���� ���ս��� �� ���� ������ �� �ִ� �Լ��� �����Ѵ�. 
            // �� AddObjectToAsset : ù ��° ���ڷ� ���� ��ü�� �� ��° ���ڷ� ���� Asset�� ���� Asset���� �����ϰ� �ȴ�. 
            // �� Graph�� SkillTree�� ���� Asset���� ������� 
            // �� ����� �� �Լ��� �ַ� Scriptable Object���� ������ �� �ַ� ���� �ϴµ� Asset ���·� ������ �� �ִ� ��� ��ü�� ��
            //    ����� �� �ִ�. 
            AssetDatabase.AddObjectToAsset(newGraph, targetObject);
            AssetDatabase.SaveAssets();

            graphProperty.objectReferenceValue = newGraph;
        }

        EditorGUILayout.Space();

        // Graph�� �����ִ� ��ư ����� 
        if (GUILayout.Button("Open Window", GUILayout.Height(50f)))
            NodeEditorWindow.Open(graphProperty.objectReferenceValue as NodeGraph);

        serializedObject.ApplyModifiedProperties();
    }
}
