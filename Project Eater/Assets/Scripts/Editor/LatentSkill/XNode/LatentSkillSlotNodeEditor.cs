using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using XNode;
using XNodeEditor;
using static XNodeEditor.NodeEditor;

[CustomNodeEditor(typeof(LatentSkillSlotNode))]
public class LatentSkillSlotNodeEditor : NodeEditor
{
    private Dictionary<string, bool> isFoldoutExpandedesByName = new Dictionary<string, bool>();

    public override void OnHeaderGUI()
    {
        var targetAsSlotNode = target as LatentSkillSlotNode;

        // 개발자 한 눈에 정보를 알 수 있도록 Header에 Node의 Index, Node가 가진 Skill의 CodeName, 없다면
        // Node의 이름을 적어줌 (우리는 여기에 포식 스킬인지 고유 스킬인지 정보도 추가)
        string concept = "해방 스킬";

        string header = $"{concept} - {targetAsSlotNode.Index}";
        GUILayout.Label(header, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
    }

    public override void OnBodyGUI()
    {
        serializedObject.Update();

        // 변수의 Label들이 깔끔하게 그려지도록 Node의 넓이를 생각해서 Label의 넓이를 수정한다. 
        float originLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 120f;

        DrawDefault();
        DrawSkill();

        EditorGUIUtility.labelWidth = originLabelWidth;

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDefault() => EditorGUILayout.PropertyField(serializedObject.FindProperty("index"));

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
        var skillListProperty = serializedObject.FindProperty("skill");

        if (skillListProperty.isArray)
        {
            for (int i = 0; i < skillListProperty.arraySize; i++)
            {
                var skillElement = skillListProperty.GetArrayElementAtIndex(i).objectReferenceValue as Skill;
                // skill이 null이 아니고 Icon을 가지고 있다면 
                if (skillElement?.Icon)
                {
                    // Icon을 Preview 형태로 그려주는 작업을 진행 
                    EditorGUILayout.BeginHorizontal();
                    {
                        // Type.GetCustomAttribute를 통해서 SkillCombinationSlotNode에 달아놨던 NodeWidthAttribute(NodeWidth(300))를 가져온다. 
                        // → Node의 넓이(NodeWidth Attribute)를 찾아옴
                        var widthAttribute = typeof(LatentSkillSlotNode).GetCustomAttribute<XNode.Node.NodeWidthAttribute>();
                        // 아래 Icon Texture가 가운데에 그려질 수 있도록 Space를 통해 GUI가 그려지는 위치를 가운데로 이동 
                        GUILayout.Space((widthAttribute.width * 0.5f) - 50f);

                        // AssetPreview.GetAssetPreview 함수로 Icon의 Preview Texture 가져오기 
                        // ※ AssetPreview.GetAssetPreview : Unity 에디터에서 프로젝트 뷰에 있는 에셋의 미리보기 이미지(썸네일)를 가져오는 데 사용
                        // 1) 파라미터 : Object asset
                        // 2) 반환값 : Texture2D
                        var preview = AssetPreview.GetAssetPreview(skillElement.Icon);
                        // Preview Texture 그려주기 
                        GUILayout.Label(preview, GUILayout.Width(80), GUILayout.Height(80));
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        // skillProperty 그리기 
        EditorGUILayout.PropertyField(skillListProperty);
    }

    private bool DrawFoldoutTitle(string title) => CustomEditorUtility.DrawFoldoutTitle(isFoldoutExpandedesByName, title);
}
