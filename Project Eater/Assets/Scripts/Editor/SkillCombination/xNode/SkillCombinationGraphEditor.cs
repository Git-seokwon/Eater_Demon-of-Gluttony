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

    // ������ ���� �󸶳� ������ �־�� tier�� index�� ������ų��
    private readonly int spacingBetweenTier = 320;

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

    // Graph Editor�� GUI�� �׷��ִ� �Լ�
    // �����ؾ��ϴ� ������ �ְų� �׷��ְ� ���� GUI�� �ִٸ� Custom Editor�� �ۼ��ϴ� �Ͱ� �Ȱ��� Code�� �߰����ָ� ��
    public override void OnGUI()
    {
        if (CheckNodePositionUpdate())
            UpdateNodePositionsAndTiers();
    }

    // Graph�� Node�� ���� �����ϴ� �Լ� 
    public override XNode.Node CopyNode(XNode.Node original)
    {
        // base.CopyNode �Լ��� �����ؼ� ���ڷ� �Ѿ�� Copy ��� Node�� ���� ������
        var newNode = base.CopyNode(original);
        // ���ο� Node�� �߰� �Ǿ����� Node���� Position�� Tier�� Update ��
        UpdateNodePositionsAndTiers();

        return newNode;
    }

    // Graph�� Node�� �����ϴ� �Լ�
    public override XNode.Node CreateNode(Type type, Vector2 position)
    {
        var node = base.CreateNode(type, position);
        UpdateNodePositionsAndTiers();
        return node;
    }

    // Graph���� Node�� �����ϴ� �Լ�
    public override void RemoveNode(XNode.Node node)
    {
        base.RemoveNode(node);

        if (target.nodes.Count == 0)
            nodePositions = Array.Empty<Vector2>();
        else
            UpdateNodePositionsAndTiers();
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

    // ������ ���� ��ġ�� ��ġ���� �ʴ� ��ġ�� �ִٸ� true�� return �Ѵ�. 
    private bool CheckNodePositionUpdate()
    {
        for (int i = 0; i < nodePositions.Length; i++)
        {
            if (nodePositions[i] != target.nodes[i].position)
                return true;
        }
        return false;
    }

    // ������ ��ġ�� nodePositions�� �����ϰ� ������ ��ġ�� ���� �� ����� Ƽ��� Index ���� �������ִ� �Լ� 
    private void UpdateNodePositionsAndTiers()
    {
        if (target.nodes.Count == 0)
            return;

        // y��ǥ�� �������� node���� �������� ����
        // => Graph���� ���� ������ y��ǥ�� �۾����Ƿ� �Ʒ��� �ִ� Node���� ���� �ִ� Node������ ���ĵ�
        target.nodes = target.nodes.OrderByDescending(x => x.position.y).ToList();
        // ���ĵ� Node���� ��ǥ�� ������
        nodePositions = target.nodes.Select(x => x.position).ToArray();

        int tier = 0;
        // ������ �������� 
        var nodes = target.nodes;

        // SkillCombinationSlotNode�� �ִ� tier ������ ������ �������� 
        var tierField = typeof(SkillCombinationSlotNode).GetField("tier", BindingFlags.Instance | BindingFlags.NonPublic);
        // �ش� ������ ù ��° ����� Ƽ� 0���� setting 
        // �� �� ���� �ִ� SkillCombinationSlotNode�� tier�� 0�� �ȴ�. 
        tierField.SetValue(nodes[0], tier);

        // ù ��° ����� ��ǥ�� �����´�. 
        // �� �ش� ��ǥ�� Ƽ� ���ϴ� �������� �ȴ�. 
        // �� ���� ���ʿ� �ִ� ù ��° ����� ��ġ�� ���� ���Ŀ� ��ġ�� ������ Ƽ� ��������. 
        var firstNodePosition = nodes[0].position;

        // 0�� ���� �̹� Ƽ� ���������� index�� 1������ �����Ѵ�. 
        // �� tierField.SetValue(nodes[0], tier);
        for (int i = 1; i < nodes.Count; i++)
        {
            // �� Ƽ�� ���� : (���� ����� y ��ǥ - ù ��° ����� y ��ǥ) / spacingBetweenTier 
            // index�� �ش��ϴ� Node�� ù��° Node���� �Ÿ��� spacingBetweenTier�� ���� ���� �ش� Node�� Tier�� �� 
            // Ex) ù ��° ��ǥ�� 0�̰� �� ��° ��ǥ�� 300�̸� (300 - 0) / 320 �̴� ���� �Ҽ������� ���´�.
            //     �� ���� int ������ ĳ�����Ͽ� �Ҽ����� �߸��� Ƽ��� 0�� �ȴ�. 
            tier = (int)(Mathf.Abs(nodes[i].position.y - firstNodePosition.y) / spacingBetweenTier);
            tierField.SetValue(nodes[i], tier);
        }

        var index = 0;
        // x��ǥ�� �������� node���� �������� ����
        // => Graph���� �������� ������ x��ǥ�� �۾����Ƿ� ���ʿ� �ִ� Node���� �����ʿ� �ִ� Node������ ���ĵ�
        var nodesByX = nodes.OrderBy(x => x.position.x).ToArray();

        // Ƽ��ó�� SkillTreeSlotNode�� �ִ� index ������ �������� �ش� ������ ù ��° ����� index�� 0���� �����Ѵ�. 
        var indexField = typeof(SkillCombinationSlotNode).GetField("index", BindingFlags.Instance | BindingFlags.NonPublic);
        indexField.SetValue(nodesByX[0], index);

        for (int i = 1; i < nodes.Count; i++)
        {
            // ���� ������ �ٸ� ������ ��ų Ʈ������ Tier�� �߰��� ����־ �̻����� ������
            // index�� �߰��� ��������� �̻��غ��̱� ������ index�� ������ ������� �������ϱ� ����
            // ���� Node�� ���� Node�� �Ÿ� ���̰� spacingBetweenTier��ŭ ���ٸ� index ���� ������Ŵ
            if (nodesByX[i].position.x - nodesByX[i - 1].position.x >= spacingBetweenTier)
                index++;

            indexField.SetValue(nodesByX[i], index);
        }
    }
}
