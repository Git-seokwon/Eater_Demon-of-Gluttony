using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XNode;
using XNodeEditor; // Node를 Custom 하기 위해서는 XNodeEditor namespace를 사용해야 함 

//  Node를 Custom 하기 위해서는 CustomNodeEditor Attribute를 달아줘야 함
[CustomNodeEditor(typeof(SkillCombinationSlotNode))]
public class SkillCombinationSlotNodeEditor : NodeEditor //  Node를 Custom 하기 위해서는 NodeEditor를 상속받아야 함
{
    // Foldout Title을 그리기위한 Dictionary
    private Dictionary<string, bool> isFoldoutExpandedesByName = new Dictionary<string, bool>();

    // Node의 Title을 어떻게 그릴지 정의하는 함수
    // ※ Header : 위쪽에 있는 검은 박스 
    public override void OnHeaderGUI()
    {
        var targetAsSlotNode = target as SkillCombinationSlotNode;

        // 개발자 한 눈에 정보를 알 수 있도록 Header에 Node의 Tier와 Index, Node가 가진 Skill의 CodeName, 없다면
        // Node의 이름을 적어줌 (우리는 여기에 포식 스킬인지 고유 스킬인지 정보도 추가)
        string concept = targetAsSlotNode.IsInherent ? "<color=\"purple\">고유</color>" : "포식";

        string header = $"Tier {targetAsSlotNode.Tier} - {targetAsSlotNode.Index} - {concept}/ " + (targetAsSlotNode.Skill?.CodeName ?? target.name);
        GUILayout.Label(header, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
    }

    // Node의 내부를 어떻게 그릴지 정의하는 함수
    public override void OnBodyGUI()
    {
        serializedObject.Update();

        // 변수의 Label들이 깔끔하게 그려지도록 Node의 넓이를 생각해서 Label의 넓이를 수정한다. 
        float originLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 120f;

        // target.GetPort로 thisNode에 달아둔 Ouput Port를 찾아온다. 
        NodePort output = target.GetPort("thisNode");
        // Port 그려주기 
        // → 첫 번째 인자는 Label이고, 두 번째 인자는 그려줄 Port이다. 
        // → 우측 상단에 Output Port가 생김 
        NodeEditorGUILayout.PortField(GUIContent.none, output);

        DrawDefault();
        DrawSkill();
        DrawPrecedingCondition();

        EditorGUIUtility.labelWidth = originLabelWidth;

        serializedObject.ApplyModifiedProperties();
    }

    // tier와 index는 SkillTreeGraphEditor에서 값을 조작해줄 것이기 때문에 enabled를 꺼서 조작하지 못하게 한다. 
    private void DrawDefault()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tier"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("index"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isInherent"));
    }

    private void DrawSkill()
    {
        if (!DrawFoldoutTitle("Skill"))
            return;

        // ※ SerializedProperty : 스크립트의 변수나 객체를 직렬화하여 Unity 에디터 창에서 쉽게 다룰 수 있도록 만들어진 클래스
        //                       : 직렬화된 데이터를 다루는 것은 SerializedObject와 그 하위 요소인 SerializedProperty
        // ※ objectReferenceValue : SerializedProperty는 다양한 데이터 타입을 지원하는데 objectReferenceValue는 Unity의 Object를 참조하는
        //                           프로퍼티에 사용
        // ※ objectReferenceValue 역할
        // 1) 참조된 객체를 반환 : 직렬화된 데이터 중에서 참조된 Unity 객체(UnityEngine.Object 타입)를 반환하거나 설정
        // 2) 참조 설정 및 변경 가능 : 이 속성을 통해 에디터에서 다른 스크립트나 객체를 참조하거나, 해당 참조 값을 변경
        var skillProperty = serializedObject.FindProperty("skill");
        var skill = skillProperty.objectReferenceValue as Skill;
        // skill이 null이 아니고 Icon을 가지고 있다면 
        if (skill?.Icon)
        {
            // Icon을 Preview 형태로 그려주는 작업을 진행 
            EditorGUILayout.BeginHorizontal();
            {
                // Type.GetCustomAttribute를 통해서 SkillCombinationSlotNode에 달아놨던 NodeWidthAttribute(NodeWidth(300))를 가져온다. 
                // → Node의 넓이(NodeWidth Attribute)를 찾아옴
                var widthAttribute = typeof(SkillCombinationSlotNode).GetCustomAttribute<XNode.Node.NodeWidthAttribute>();
                // 아래 Icon Texture가 가운데에 그려질 수 있도록 Space를 통해 GUI가 그려지는 위치를 가운데로 이동 
                GUILayout.Space((widthAttribute.width * 0.5f) - 50f);

                // AssetPreview.GetAssetPreview 함수로 Icon의 Preview Texture 가져오기 
                // ※ AssetPreview.GetAssetPreview : Unity 에디터에서 프로젝트 뷰에 있는 에셋의 미리보기 이미지(썸네일)를 가져오는 데 사용
                // 1) 파라미터 : Object asset
                // 2) 반환값 : Texture2D
                var preview = AssetPreview.GetAssetPreview(skill.Icon);
                // Preview Texture 그려주기 
                GUILayout.Label(preview, GUILayout.Width(80), GUILayout.Height(80));
            }
            EditorGUILayout.EndHorizontal();
        }

        // skillProperty 그리기 
        EditorGUILayout.PropertyField(skillProperty);
    }

    private void DrawPrecedingCondition()
    {
        if (!DrawFoldoutTitle("Preceding Condition"))
            return;

        // ※ NodeEditorGUILayout.DynamicPortList : List의 각 Element에 Port가 달린 형태로 List를 그려줌
        // 1) precedingLevels : 그릴 변수 이름 
        // 2) typeof(int) : Port의 타입 
        // → 사실 Port와 연결되는 Output Port의 Type이 SkillCombinationSlotNode Type이기 때문에 똑같이 SkillTreeSlotNode Type으로 설정해야 
        //    하지만, int로 설정함 
        // ※ Port의 타입을 Int 형으로 한 이유 
        // → DynamicPortList의 인자로 연결되는 Port Type을 제한하는 TypeConstraint 인자도 존재한다. 그러면 왜 안 쓰는 가?
        //    여기서 만들어지는 Port는 Int형이고 Output Port는 SkillTreeNode Type이기 때문에 서로 완전히 다른 Type 이므로 제한을
        //    걸 수가 없다. 대신 OnCreatePrecedingLevels에서 List를 그릴 때, 직접 TypeConstraint를 구현해 줄 것이다. 이것을 보여주기 
        //    위해서 굳이 Port를 int형으로 만들었다. 
        // 3) serializedObject : precedingLevels 변수를 가지고 있는 serializedObject
        // 4) NodePort.IO : Port가 Input 타입인지 Output 타입인지 여부 → Input 
        // 5) Node.ConnectionType : Port의 ConnectionType
        // 6) onCreation : Action 인자 
        // → 해당 Action에 CallBack 함수를 넘겨주는 것으로 List를 어떻게 그릴지 정해 줄 수 있다. 
        // → onCreation을 안 넘겨주면 XNode 내부에 정의되어 있는 기본 형태로 List가 그려지게 된다. 
        NodeEditorGUILayout.DynamicPortList("precedingLevels", typeof(int), serializedObject,
            NodePort.IO.Input, XNode.Node.ConnectionType.Override, onCreation: OnCreatePrecedingLevels);
    }

    // precedingLevels 변수를 ReorderableList 형태로 그려주는 함수
    // → 리스트를 그릴 때 포트와 함께 시각적으로 표시하고, 각 요소의 연결 상태를 관리하는 역할
    private void OnCreatePrecedingLevels(ReorderableList list)
    {
        // ※ drawHeaderCallback : 리스트의 상단에 표시되는 헤더 텍스트를 정의
        // → list의 Header에 Preceding Skills라는 Text를 띄움 
        list.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Preceding Skills");
        };

        // ※ 리스트의 각 요소를 그릴 때 호출
        // → list의 각 Element를 그려줌 
        // 1) rect  : 현재 요소가 그려질 위치와 크기를 정의하는 사각형
        // 2) index : 리스트에서 현재 요소의 인덱스
        // 3) isActive : 현재 리스트에서 활성화된 요소인지 여부
        //             : 사용자가 리스트의 특정 항목을 클릭해서 선택 상태가 되었을 때 isActive가 true가 된다. 
        // Ex) 리스트 항목 중 하나가 선택된 상태면, 선택된 항목의 배경색을 다르게 그리거나, 선택된 항목만 편집할 수 있는 인터페이스를 제공
        // 4) isFocused : 현재 포커스를 받은 요소인지 여부
        //              : 사용자가 리스트 항목을 클릭하거나 키보드로 탐색할 때 포커스가 해당 항목에 있는지를 나타낸다. 
        //              : 보통 이 상태는 사용자가 키보드로 리스트를 조작할 때 중요
        // Ex) 포커스가 있는 항목에 특별한 테두리를 그리거나, 다른 색상으로 강조 표시
        list.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            // 현재 index에 해당하는 Element를 가져옴
            var element = serializedObject.FindProperty("precedingLevels").GetArrayElementAtIndex(index);
            // 가져온 Element를 그려줌, Need Level이라는 Label을 가진 int Field가 그려짐
            EditorGUI.PropertyField(rect, element, new GUIContent("Need Level"));

            // 노드에서 precedingLevels 배열의 각 요소와 연결된 Input 포트를 가져옴, 이 Port에 다른 Node들이 연결됨
            // GetPort 규칙은 (배열 변수 이름 + 찾아올 Port의 index)
            // ex. precedingLevels의 첫번째 Element에 할당된 Port를 찾아오려면 target.GetPort("precedingLevels 0")
            var port = target.GetPort("precedingLevels " + index);

            // Port와 연결된 Output Port가 있을 때,
            // Output Port의 반환 값이 SkillTreeSlotNode Type이 아니라면 연결을 끊음
            // → 같은 Type만 연결되게 하는 Node.TypeConstraint.Strict를 직접 구현해준 것임
            // → 원래는 위에서 Port의 타입을 지정해주는 것이 훨씬 더 깔끔하다. 
            if (port.Connection != null && port.Connection.GetOutputValue() is not SkillCombinationSlotNode)
                port.Disconnect(port.Connection);

            // port.GetInputValue 함수로 이 Port(Input)와 연결된 Output Port의 값을 찾아온다. 
            // =) port.Connection.GetOutputValue()
            // → 지금은 precedinglevel Port와 연결될 수 있는 Output Port가 SkillTreeSlotNode를 반환하는 Port 밖에 없어서 
            //    value를 SkillTreeSlotNode로 바로 Casting 하지만, 다양한 반환 값을 가진 Port들이 연결할 수 있는 경우, Generic이 아닌
            //    object Type으로 Value를 반환하는 GetInputValue 함수를 써준 뒤 반환 값을 때에 따라 적절히 Casting 하면 된다. 
            // → Node의 ConnectionType이 Multiple일 경우,
            //    GetInputValues 함수로 연결된 모든 Port의 Value를 가져올 수 있음
            var inputSlot = port.GetInputValue<SkillCombinationSlotNode>();
            // 연결된 Port가 있고, 해당 Port에 Skill 할당되어 있다면, 값을 Node가 가진 Skill의 최대 Level로 제한
            if (inputSlot && inputSlot.Skill)
                element.intValue = Mathf.Clamp(element.intValue, 1, inputSlot.Skill.MaxLevel);

            // Input Port를 Element의 왼쪽 끝에 그림 
            var position = rect.position;
            position.x -= 37f;
            // Port를 그려줌
            NodeEditorGUILayout.PortField(position, port);
        };
    }

    private bool DrawFoldoutTitle(string title)
    => CustomEditorUtility.DrawFoldoutTitle(isFoldoutExpandedesByName, title);
}
