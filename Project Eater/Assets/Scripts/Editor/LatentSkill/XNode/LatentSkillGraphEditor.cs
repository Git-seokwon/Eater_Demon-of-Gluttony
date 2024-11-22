using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNodeEditor;
using static XNodeEditor.NodeGraphEditor;

[CustomNodeGraphEditor(typeof(LatentSkillGraph))]
public class LatentSkillGraphEditor : NodeGraphEditor
{
    private Vector2[] nodePositions;

    public override void OnOpen()
    {
        // �׷������� ���׷� ���� �� ��尡 ���� ���, Remove �Լ��� ��� ��Ͽ��� null�� ��Ҹ� �����Ѵ�. 
        // �� target : Graph
        // �� nodes  : Graph�� ���� 
        target.nodes.Remove(null);
        // ���� ������ ��ġ�� �������ش�. 
        nodePositions = target.nodes.Select(x => x.position).ToArray();
    }

    public override XNode.Node CopyNode(XNode.Node original)
    {
        // base.CopyNode �Լ��� �����ؼ� ���ڷ� �Ѿ�� Copy ��� Node�� ���� ������
        var newNode = base.CopyNode(original);

        return newNode;
    }

    public override XNode.Node CreateNode(Type type, Vector2 position)
    {
        var node = base.CreateNode(type, position);
        return node;
    }

    public override void RemoveNode(XNode.Node node)
    {
        base.RemoveNode(node);

        if (target.nodes.Count == 0)
            nodePositions = Array.Empty<Vector2>();
    }

    public override string GetNodeMenuName(Type type)
    {
        if (type.Name == "LatentSkillSlotNode")
            return base.GetNodeMenuName(type);
        else
            return null;
    }
}
