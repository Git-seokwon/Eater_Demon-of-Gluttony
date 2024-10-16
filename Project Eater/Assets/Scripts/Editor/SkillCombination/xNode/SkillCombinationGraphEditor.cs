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
    // Graph에 존재하는 Node들의 위치를 저장하는 배열.
    // Node들의 저장된 위치와 현재 위치가 다르다면 Node를 Update 해줄 것임.
    private Vector2[] nodePositions;

    // Graph Editor를 열 때 실행되는 함수
    public override void OnOpen()
    {
        // 그래프에서 버그로 생긴 빈 노드가 있을 경우, Remove 함수로 노드 목록에서 null인 요소를 제거한다. 
        // ※ target : Graph
        // ※ nodes  : Graph의 노드들 
        target.nodes.Remove(null);
        // 현재 노드들의 위치를 저장해준다. 
        nodePositions = target.nodes.Select(x => x.position).ToArray();
    }

    // Graph에 Node를 복사 생성하는 함수 
    public override XNode.Node CopyNode(XNode.Node original)
    {
        // base.CopyNode 함수를 실행해서 인자로 넘어온 Copy 대상 Node를 복사 생성함
        var newNode = base.CopyNode(original);

        return newNode;
    }

    // Graph에 Node를 생성하는 함수
    public override XNode.Node CreateNode(Type type, Vector2 position)
    {
        var node = base.CreateNode(type, position);
        return node;
    }

    // Graph에서 Node를 제거하는 함수
    public override void RemoveNode(XNode.Node node)
    {
        base.RemoveNode(node);

        if (target.nodes.Count == 0)
            nodePositions = Array.Empty<Vector2>();
    }

    // SkillTreeGraph에서 사용할 Node를 반환하는 함수
    // 인자로는 Project에 정의되어있는 모든 Node Type(SkillTreeSlotNode)이 넘어옴
    public override string GetNodeMenuName(Type type)
    {
        if (type.Name == "SkillCombinationSlotNode")
            return base.GetNodeMenuName(type);
        else
            return null;
    }
}
