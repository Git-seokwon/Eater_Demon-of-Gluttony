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

        // ������ �� ���� ������ �� �� �ֵ��� Header�� Node�� Index, Node�� ���� Skill�� CodeName, ���ٸ�
        // Node�� �̸��� ������ (�츮�� ���⿡ ���� ��ų���� ���� ��ų���� ������ �߰�)
        string concept = "�ع� ��ų";

        string header = $"{concept} - {targetAsSlotNode.Index}";
        GUILayout.Label(header, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
    }

    public override void OnBodyGUI()
    {
        serializedObject.Update();

        // ������ Label���� ����ϰ� �׷������� Node�� ���̸� �����ؼ� Label�� ���̸� �����Ѵ�. 
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

        // �� SerializedProperty : ��ũ��Ʈ�� ������ ��ü�� ����ȭ�Ͽ� Unity ������ â���� ���� �ٷ� �� �ֵ��� ������� Ŭ����
        //                       : ����ȭ�� �����͸� �ٷ�� ���� SerializedObject�� �� ���� ����� SerializedProperty
        // �� objectReferenceValue : SerializedProperty�� �پ��� ������ Ÿ���� �����ϴµ� objectReferenceValue�� Unity�� Object�� �����ϴ�
        //                           ������Ƽ�� ���
        // �� objectReferenceValue ����
        // 1) ������ ��ü�� ��ȯ : ����ȭ�� ������ �߿��� ������ Unity ��ü(UnityEngine.Object Ÿ��)�� ��ȯ�ϰų� ����
        // 2) ���� ���� �� ���� ���� : �� �Ӽ��� ���� �����Ϳ��� �ٸ� ��ũ��Ʈ�� ��ü�� �����ϰų�, �ش� ���� ���� ����
        var skillListProperty = serializedObject.FindProperty("skill");

        if (skillListProperty.isArray)
        {
            for (int i = 0; i < skillListProperty.arraySize; i++)
            {
                var skillElement = skillListProperty.GetArrayElementAtIndex(i).objectReferenceValue as Skill;
                // skill�� null�� �ƴϰ� Icon�� ������ �ִٸ� 
                if (skillElement?.Icon)
                {
                    // Icon�� Preview ���·� �׷��ִ� �۾��� ���� 
                    EditorGUILayout.BeginHorizontal();
                    {
                        // Type.GetCustomAttribute�� ���ؼ� SkillCombinationSlotNode�� �޾Ƴ��� NodeWidthAttribute(NodeWidth(300))�� �����´�. 
                        // �� Node�� ����(NodeWidth Attribute)�� ã�ƿ�
                        var widthAttribute = typeof(LatentSkillSlotNode).GetCustomAttribute<XNode.Node.NodeWidthAttribute>();
                        // �Ʒ� Icon Texture�� ����� �׷��� �� �ֵ��� Space�� ���� GUI�� �׷����� ��ġ�� ����� �̵� 
                        GUILayout.Space((widthAttribute.width * 0.5f) - 50f);

                        // AssetPreview.GetAssetPreview �Լ��� Icon�� Preview Texture �������� 
                        // �� AssetPreview.GetAssetPreview : Unity �����Ϳ��� ������Ʈ �信 �ִ� ������ �̸����� �̹���(�����)�� �������� �� ���
                        // 1) �Ķ���� : Object asset
                        // 2) ��ȯ�� : Texture2D
                        var preview = AssetPreview.GetAssetPreview(skillElement.Icon);
                        // Preview Texture �׷��ֱ� 
                        GUILayout.Label(preview, GUILayout.Width(80), GUILayout.Height(80));
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        // skillProperty �׸��� 
        EditorGUILayout.PropertyField(skillListProperty);
    }

    private bool DrawFoldoutTitle(string title) => CustomEditorUtility.DrawFoldoutTitle(isFoldoutExpandedesByName, title);
}
