using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeGraphEditor(typeof(SkillCombinationGraph))]
public class SkillCombinationGraphEditor : NodeGraphEditor
{
    // Graph�� �����ϴ� Node���� ��ġ�� �����ϴ� �迭.
    // Node���� ����� ��ġ�� ���� ��ġ�� �ٸ��ٸ� Node�� Update ���� ����.
    private Vector2[] nodePositions;

    // Graph Editor�� �� �� ����Ǵ� �Լ�
    public override void OnOpen()
    {
        // �׷������� ���׷� ���� �� ��尡 ���� ���, Remove �Լ��� ��� ��Ͽ��� null�� ��Ҹ� �����Ѵ�. 
        // �� target : Graph
        // �� nodes  : Graph�� ���� 
        target.nodes.Remove(null);
        // ���� ������ ��ġ�� �������ش�. 
        nodePositions = target.nodes.Select(x => x.position).ToArray();
    }

    // Graph�� Node�� ���� �����ϴ� �Լ� 
    public override XNode.Node CopyNode(XNode.Node original)
    {
        // base.CopyNode �Լ��� �����ؼ� ���ڷ� �Ѿ�� Copy ��� Node�� ���� ������
        var newNode = base.CopyNode(original);

        return newNode;
    }

    // Graph�� Node�� �����ϴ� �Լ�
    public override XNode.Node CreateNode(Type type, Vector2 position)
    {
        var node = base.CreateNode(type, position);
        return node;
    }

    // Graph���� Node�� �����ϴ� �Լ�
    public override void RemoveNode(XNode.Node node)
    {
        base.RemoveNode(node);

        if (target.nodes.Count == 0)
            nodePositions = Array.Empty<Vector2>();
    }

    // SkillTreeGraph���� ����� Node�� ��ȯ�ϴ� �Լ�
    // ���ڷδ� Project�� ���ǵǾ��ִ� ��� Node Type(SkillTreeSlotNode)�� �Ѿ��
    public override string GetNodeMenuName(Type type)
    {
        if (type.Name == "SkillCombinationSlotNode")
            return base.GetNodeMenuName(type);
        else
            return null;
    }
}
