using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XNode;
using XNodeEditor; // Node�� Custom �ϱ� ���ؼ��� XNodeEditor namespace�� ����ؾ� �� 

//  Node�� Custom �ϱ� ���ؼ��� CustomNodeEditor Attribute�� �޾���� ��
[CustomNodeEditor(typeof(SkillCombinationSlotNode))]
public class SkillCombinationSlotNodeEditor : NodeEditor //  Node�� Custom �ϱ� ���ؼ��� NodeEditor�� ��ӹ޾ƾ� ��
{
    // Foldout Title�� �׸������� Dictionary
    private Dictionary<string, bool> isFoldoutExpandedesByName = new Dictionary<string, bool>();

    // Node�� Title�� ��� �׸��� �����ϴ� �Լ�
    // �� Header : ���ʿ� �ִ� ���� �ڽ� 
    public override void OnHeaderGUI()
    {
        var targetAsSlotNode = target as SkillCombinationSlotNode;

        // ������ �� ���� ������ �� �� �ֵ��� Header�� Node�� Tier�� Index, Node�� ���� Skill�� CodeName, ���ٸ�
        // Node�� �̸��� ������ (�츮�� ���⿡ ���� ��ų���� ���� ��ų���� ������ �߰�)
        string concept = targetAsSlotNode.IsInherent ? "<color=\"purple\">����</color>" : "����";

        string header = $"Tier {targetAsSlotNode.Tier} - {targetAsSlotNode.Index} - {concept}/ " + (targetAsSlotNode.Skill?.CodeName ?? target.name);
        GUILayout.Label(header, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
    }

    // Node�� ���θ� ��� �׸��� �����ϴ� �Լ�
    public override void OnBodyGUI()
    {
        serializedObject.Update();

        // ������ Label���� ����ϰ� �׷������� Node�� ���̸� �����ؼ� Label�� ���̸� �����Ѵ�. 
        float originLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 120f;

        // target.GetPort�� thisNode�� �޾Ƶ� Ouput Port�� ã�ƿ´�. 
        NodePort output = target.GetPort("thisNode");
        // Port �׷��ֱ� 
        // �� ù ��° ���ڴ� Label�̰�, �� ��° ���ڴ� �׷��� Port�̴�. 
        // �� ���� ��ܿ� Output Port�� ���� 
        NodeEditorGUILayout.PortField(GUIContent.none, output);

        DrawDefault();
        DrawSkill();
        DrawPrecedingCondition();

        EditorGUIUtility.labelWidth = originLabelWidth;

        serializedObject.ApplyModifiedProperties();
    }

    // tier�� index�� SkillTreeGraphEditor���� ���� �������� ���̱� ������ enabled�� ���� �������� ���ϰ� �Ѵ�. 
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

        // �� SerializedProperty : ��ũ��Ʈ�� ������ ��ü�� ����ȭ�Ͽ� Unity ������ â���� ���� �ٷ� �� �ֵ��� ������� Ŭ����
        //                       : ����ȭ�� �����͸� �ٷ�� ���� SerializedObject�� �� ���� ����� SerializedProperty
        // �� objectReferenceValue : SerializedProperty�� �پ��� ������ Ÿ���� �����ϴµ� objectReferenceValue�� Unity�� Object�� �����ϴ�
        //                           ������Ƽ�� ���
        // �� objectReferenceValue ����
        // 1) ������ ��ü�� ��ȯ : ����ȭ�� ������ �߿��� ������ Unity ��ü(UnityEngine.Object Ÿ��)�� ��ȯ�ϰų� ����
        // 2) ���� ���� �� ���� ���� : �� �Ӽ��� ���� �����Ϳ��� �ٸ� ��ũ��Ʈ�� ��ü�� �����ϰų�, �ش� ���� ���� ����
        var skillProperty = serializedObject.FindProperty("skill");
        var skill = skillProperty.objectReferenceValue as Skill;
        // skill�� null�� �ƴϰ� Icon�� ������ �ִٸ� 
        if (skill?.Icon)
        {
            // Icon�� Preview ���·� �׷��ִ� �۾��� ���� 
            EditorGUILayout.BeginHorizontal();
            {
                // Type.GetCustomAttribute�� ���ؼ� SkillCombinationSlotNode�� �޾Ƴ��� NodeWidthAttribute(NodeWidth(300))�� �����´�. 
                // �� Node�� ����(NodeWidth Attribute)�� ã�ƿ�
                var widthAttribute = typeof(SkillCombinationSlotNode).GetCustomAttribute<XNode.Node.NodeWidthAttribute>();
                // �Ʒ� Icon Texture�� ����� �׷��� �� �ֵ��� Space�� ���� GUI�� �׷����� ��ġ�� ����� �̵� 
                GUILayout.Space((widthAttribute.width * 0.5f) - 50f);

                // AssetPreview.GetAssetPreview �Լ��� Icon�� Preview Texture �������� 
                // �� AssetPreview.GetAssetPreview : Unity �����Ϳ��� ������Ʈ �信 �ִ� ������ �̸����� �̹���(�����)�� �������� �� ���
                // 1) �Ķ���� : Object asset
                // 2) ��ȯ�� : Texture2D
                var preview = AssetPreview.GetAssetPreview(skill.Icon);
                // Preview Texture �׷��ֱ� 
                GUILayout.Label(preview, GUILayout.Width(80), GUILayout.Height(80));
            }
            EditorGUILayout.EndHorizontal();
        }

        // skillProperty �׸��� 
        EditorGUILayout.PropertyField(skillProperty);
    }

    private void DrawPrecedingCondition()
    {
        if (!DrawFoldoutTitle("Preceding Condition"))
            return;

        // �� NodeEditorGUILayout.DynamicPortList : List�� �� Element�� Port�� �޸� ���·� List�� �׷���
        // 1) precedingLevels : �׸� ���� �̸� 
        // 2) typeof(int) : Port�� Ÿ�� 
        // �� ��� Port�� ����Ǵ� Output Port�� Type�� SkillCombinationSlotNode Type�̱� ������ �Ȱ��� SkillTreeSlotNode Type���� �����ؾ� 
        //    ������, int�� ������ 
        // �� Port�� Ÿ���� Int ������ �� ���� 
        // �� DynamicPortList�� ���ڷ� ����Ǵ� Port Type�� �����ϴ� TypeConstraint ���ڵ� �����Ѵ�. �׷��� �� �� ���� ��?
        //    ���⼭ ��������� Port�� Int���̰� Output Port�� SkillTreeNode Type�̱� ������ ���� ������ �ٸ� Type �̹Ƿ� ������
        //    �� ���� ����. ��� OnCreatePrecedingLevels���� List�� �׸� ��, ���� TypeConstraint�� ������ �� ���̴�. �̰��� �����ֱ� 
        //    ���ؼ� ���� Port�� int������ �������. 
        // 3) serializedObject : precedingLevels ������ ������ �ִ� serializedObject
        // 4) NodePort.IO : Port�� Input Ÿ������ Output Ÿ������ ���� �� Input 
        // 5) Node.ConnectionType : Port�� ConnectionType
        // 6) onCreation : Action ���� 
        // �� �ش� Action�� CallBack �Լ��� �Ѱ��ִ� ������ List�� ��� �׸��� ���� �� �� �ִ�. 
        // �� onCreation�� �� �Ѱ��ָ� XNode ���ο� ���ǵǾ� �ִ� �⺻ ���·� List�� �׷����� �ȴ�. 
        NodeEditorGUILayout.DynamicPortList("precedingLevels", typeof(int), serializedObject,
            NodePort.IO.Input, XNode.Node.ConnectionType.Override, onCreation: OnCreatePrecedingLevels);
    }

    // precedingLevels ������ ReorderableList ���·� �׷��ִ� �Լ�
    // �� ����Ʈ�� �׸� �� ��Ʈ�� �Բ� �ð������� ǥ���ϰ�, �� ����� ���� ���¸� �����ϴ� ����
    private void OnCreatePrecedingLevels(ReorderableList list)
    {
        // �� drawHeaderCallback : ����Ʈ�� ��ܿ� ǥ�õǴ� ��� �ؽ�Ʈ�� ����
        // �� list�� Header�� Preceding Skills��� Text�� ��� 
        list.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Preceding Skills");
        };

        // �� ����Ʈ�� �� ��Ҹ� �׸� �� ȣ��
        // �� list�� �� Element�� �׷��� 
        // 1) rect  : ���� ��Ұ� �׷��� ��ġ�� ũ�⸦ �����ϴ� �簢��
        // 2) index : ����Ʈ���� ���� ����� �ε���
        // 3) isActive : ���� ����Ʈ���� Ȱ��ȭ�� ������� ����
        //             : ����ڰ� ����Ʈ�� Ư�� �׸��� Ŭ���ؼ� ���� ���°� �Ǿ��� �� isActive�� true�� �ȴ�. 
        // Ex) ����Ʈ �׸� �� �ϳ��� ���õ� ���¸�, ���õ� �׸��� ������ �ٸ��� �׸��ų�, ���õ� �׸� ������ �� �ִ� �������̽��� ����
        // 4) isFocused : ���� ��Ŀ���� ���� ������� ����
        //              : ����ڰ� ����Ʈ �׸��� Ŭ���ϰų� Ű����� Ž���� �� ��Ŀ���� �ش� �׸� �ִ����� ��Ÿ����. 
        //              : ���� �� ���´� ����ڰ� Ű����� ����Ʈ�� ������ �� �߿�
        // Ex) ��Ŀ���� �ִ� �׸� Ư���� �׵θ��� �׸��ų�, �ٸ� �������� ���� ǥ��
        list.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            // ���� index�� �ش��ϴ� Element�� ������
            var element = serializedObject.FindProperty("precedingLevels").GetArrayElementAtIndex(index);
            // ������ Element�� �׷���, Need Level�̶�� Label�� ���� int Field�� �׷���
            EditorGUI.PropertyField(rect, element, new GUIContent("Need Level"));

            // ��忡�� precedingLevels �迭�� �� ��ҿ� ����� Input ��Ʈ�� ������, �� Port�� �ٸ� Node���� �����
            // GetPort ��Ģ�� (�迭 ���� �̸� + ã�ƿ� Port�� index)
            // ex. precedingLevels�� ù��° Element�� �Ҵ�� Port�� ã�ƿ����� target.GetPort("precedingLevels 0")
            var port = target.GetPort("precedingLevels " + index);

            // Port�� ����� Output Port�� ���� ��,
            // Output Port�� ��ȯ ���� SkillTreeSlotNode Type�� �ƴ϶�� ������ ����
            // �� ���� Type�� ����ǰ� �ϴ� Node.TypeConstraint.Strict�� ���� �������� ����
            // �� ������ ������ Port�� Ÿ���� �������ִ� ���� �ξ� �� ����ϴ�. 
            if (port.Connection != null && port.Connection.GetOutputValue() is not SkillCombinationSlotNode)
                port.Disconnect(port.Connection);

            // port.GetInputValue �Լ��� �� Port(Input)�� ����� Output Port�� ���� ã�ƿ´�. 
            // =) port.Connection.GetOutputValue()
            // �� ������ precedinglevel Port�� ����� �� �ִ� Output Port�� SkillTreeSlotNode�� ��ȯ�ϴ� Port �ۿ� ��� 
            //    value�� SkillTreeSlotNode�� �ٷ� Casting ������, �پ��� ��ȯ ���� ���� Port���� ������ �� �ִ� ���, Generic�� �ƴ�
            //    object Type���� Value�� ��ȯ�ϴ� GetInputValue �Լ��� ���� �� ��ȯ ���� ���� ���� ������ Casting �ϸ� �ȴ�. 
            // �� Node�� ConnectionType�� Multiple�� ���,
            //    GetInputValues �Լ��� ����� ��� Port�� Value�� ������ �� ����
            var inputSlot = port.GetInputValue<SkillCombinationSlotNode>();
            // ����� Port�� �ְ�, �ش� Port�� Skill �Ҵ�Ǿ� �ִٸ�, ���� Node�� ���� Skill�� �ִ� Level�� ����
            if (inputSlot && inputSlot.Skill)
                element.intValue = Mathf.Clamp(element.intValue, 1, inputSlot.Skill.MaxLevel);

            // Input Port�� Element�� ���� ���� �׸� 
            var position = rect.position;
            position.x -= 37f;
            // Port�� �׷���
            NodeEditorGUILayout.PortField(position, port);
        };
    }

    private bool DrawFoldoutTitle(string title)
    => CustomEditorUtility.DrawFoldoutTitle(isFoldoutExpandedesByName, title);
}
