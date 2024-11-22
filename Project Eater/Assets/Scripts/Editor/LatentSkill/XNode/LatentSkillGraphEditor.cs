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
        // 그래프에서 버그로 생긴 빈 노드가 있을 경우, Remove 함수로 노드 목록에서 null인 요소를 제거한다. 
        // ※ target : Graph
        // ※ nodes  : Graph의 노드들 
        target.nodes.Remove(null);
        // 현재 노드들의 위치를 저장해준다. 
        nodePositions = target.nodes.Select(x => x.position).ToArray();
    }

    public override XNode.Node CopyNode(XNode.Node original)
    {
        // base.CopyNode 함수를 실행해서 인자로 넘어온 Copy 대상 Node를 복사 생성함
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
