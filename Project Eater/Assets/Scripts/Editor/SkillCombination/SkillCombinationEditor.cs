using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNodeEditor;
using XNode;

// SkillTree가 IO 객체이기 때문에 IdentifiedObjectEditor를 상속받음 
[CustomEditor(typeof(SkillCombination))] // SkillCombination이라는 클래스의 인스펙터에서 이 에디터가 활성화
public class SkillCombinationEditor : IdentifiedObjectEditor
{
    private SerializedProperty graphProperty;

    protected override void OnEnable()
    {
        base.OnEnable();

        // SkillCombination의 graph 필드
        graphProperty = serializedObject.FindProperty("graph");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // 직렬화된 객체의 최신 상태를 가져오기 
        serializedObject.Update();

        // graph가 없으면 자동으로 만들어줌
        if (graphProperty.objectReferenceValue == null)
        {
            // ※ serializedObject.targetObject : 현재 인스펙터에 연결된 SkillCombination 객체
            var targetObject = serializedObject.targetObject; 
            // Graph는 기본적으로 Scriptable Object이기 때문에 CreateInstance로 SkillTreeGraph를 생성
            var newGraph = CreateInstance<SkillCombinationGraph>();
            newGraph.name = "Skill Tree Graph";

            // Graph는 Scriptable Object이기 때문에 Project에 Asset으로 저장하지 않으면 그냥 사라져버린다. 
            // Asset으로 저장할 때, 아무 곳에나 저장해버리면 SkillCombination Asset과 SkillCombination Asset이 사용하는 Graph Asset
            // 이렇게 두 개의 Asset을 따로 관리해야 하는 귀찮음이 생긴다. 
            // → Unity에는 Asset을 다른 Asset의 하위 Asset으로 결합시켜 한 번에 관리할 수 있는 함수가 존재한다. 
            // → AddObjectToAsset : 첫 번째 인자로 넣은 객체가 두 번째 인자로 넣은 Asset의 하위 Asset으로 결합하게 된다. 
            // → Graph를 SkillTree의 하위 Asset으로 만들어줌 
            // ※ 참고로 이 함수는 주로 Scriptable Object들을 결합할 때 주로 쓰긴 하는데 Asset 형태로 저장할 수 있는 모든 객체에 다
            //    사용할 수 있다. 
            AssetDatabase.AddObjectToAsset(newGraph, targetObject);
            AssetDatabase.SaveAssets();

            graphProperty.objectReferenceValue = newGraph;
        }

        EditorGUILayout.Space();

        // Graph를 열어주는 버튼 만들기 
        if (GUILayout.Button("Open Window", GUILayout.Height(50f)))
            NodeEditorWindow.Open(graphProperty.objectReferenceValue as NodeGraph);

        serializedObject.ApplyModifiedProperties();
    }
}
