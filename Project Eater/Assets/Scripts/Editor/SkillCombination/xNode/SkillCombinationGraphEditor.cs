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

    // 노드들이 서로 얼마나 떨어져 있어야 tier와 index를 증가시킬지
    private readonly int spacingBetweenTier = 320;

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

    // Graph Editor의 GUI를 그려주는 함수
    // 실행해야하는 로직이 있거나 그려주고 싶은 GUI가 있다면 Custom Editor를 작성하던 것과 똑같이 Code를 추가해주면 됨
    public override void OnGUI()
    {
        if (CheckNodePositionUpdate())
            UpdateNodePositionsAndTiers();
    }

    // Graph에 Node를 복사 생성하는 함수 
    public override XNode.Node CopyNode(XNode.Node original)
    {
        // base.CopyNode 함수를 실행해서 인자로 넘어온 Copy 대상 Node를 복사 생성함
        var newNode = base.CopyNode(original);
        // 새로운 Node가 추가 되었으니 Node들의 Position과 Tier를 Update 함
        UpdateNodePositionsAndTiers();

        return newNode;
    }

    // Graph에 Node를 생성하는 함수
    public override XNode.Node CreateNode(Type type, Vector2 position)
    {
        var node = base.CreateNode(type, position);
        UpdateNodePositionsAndTiers();
        return node;
    }

    // Graph에서 Node를 제거하는 함수
    public override void RemoveNode(XNode.Node node)
    {
        base.RemoveNode(node);

        if (target.nodes.Count == 0)
            nodePositions = Array.Empty<Vector2>();
        else
            UpdateNodePositionsAndTiers();
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

    // 노드들의 현재 위치와 일치하지 않는 위치가 있다면 true를 return 한다. 
    private bool CheckNodePositionUpdate()
    {
        for (int i = 0; i < nodePositions.Length; i++)
        {
            if (nodePositions[i] != target.nodes[i].position)
                return true;
        }
        return false;
    }

    // 노드들의 위치를 nodePositions에 저장하고 노드들의 위치에 따라 각 노드의 티어와 Index 값을 설정해주는 함수 
    private void UpdateNodePositionsAndTiers()
    {
        if (target.nodes.Count == 0)
            return;

        // y좌표를 기준으로 node들을 내림차순 정렬
        // => Graph에서 위로 갈수록 y좌표가 작아지므로 아래에 있는 Node에서 위에 있는 Node순으로 정렬됨
        target.nodes = target.nodes.OrderByDescending(x => x.position.y).ToList();
        // 정렬된 Node들의 좌표를 저장함
        nodePositions = target.nodes.Select(x => x.position).ToArray();

        int tier = 0;
        // 노드들을 가져오기 
        var nodes = target.nodes;

        // SkillCombinationSlotNode에 있는 tier 변수의 정보를 가져오기 
        var tierField = typeof(SkillCombinationSlotNode).GetField("tier", BindingFlags.Instance | BindingFlags.NonPublic);
        // 해당 정보로 첫 번째 노드의 티어를 0으로 setting 
        // → 맨 위에 있는 SkillCombinationSlotNode의 tier가 0이 된다. 
        tierField.SetValue(nodes[0], tier);

        // 첫 번째 노드의 좌표를 가져온다. 
        // → 해당 좌표가 티어를 정하는 기준점이 된다. 
        // → 가장 위쪽에 있는 첫 번째 노드의 위치에 따라 이후에 위치한 노드들의 티어가 정해진다. 
        var firstNodePosition = nodes[0].position;

        // 0번 노드는 이미 티어를 설정했으니 index를 1번부터 시작한다. 
        // → tierField.SetValue(nodes[0], tier);
        for (int i = 1; i < nodes.Count; i++)
        {
            // ※ 티어 공식 : (현재 노드의 y 좌표 - 첫 번째 노드의 y 좌표) / spacingBetweenTier 
            // index에 해당하는 Node와 첫번째 Node와의 거리를 spacingBetweenTier로 나눈 값이 해당 Node의 Tier가 됨 
            // Ex) 첫 번째 좌표가 0이고 두 번째 좌표가 300이면 (300 - 0) / 320 이니 값이 소숫점으로 나온다.
            //     그 값을 int 형으로 캐스팅하여 소숫점이 잘리고 티어는 0이 된다. 
            tier = (int)(Mathf.Abs(nodes[i].position.y - firstNodePosition.y) / spacingBetweenTier);
            tierField.SetValue(nodes[i], tier);
        }

        var index = 0;
        // x좌표를 기준으로 node들을 오름차순 정렬
        // => Graph에서 왼쪽으로 갈수록 x좌표가 작아지므로 왼쪽에 있는 Node에서 오른쪽에 있는 Node순으로 정렬됨
        var nodesByX = nodes.OrderBy(x => x.position.x).ToArray();

        // 티어처럼 SkillTreeSlotNode에 있는 index 정보를 가져오고 해당 정보로 첫 번째 노드의 index를 0으로 설정한다. 
        var indexField = typeof(SkillCombinationSlotNode).GetField("index", BindingFlags.Instance | BindingFlags.NonPublic);
        indexField.SetValue(nodesByX[0], index);

        for (int i = 1; i < nodes.Count; i++)
        {
            // 위와 수식이 다른 이유는 스킬 트리에서 Tier는 중간이 비어있어도 이상하지 않지만
            // index는 중간이 비어있으면 이상해보이기 때문에 index가 무조건 순서대로 나오게하기 위함
            // 이전 Node와 현재 Node의 거리 차이가 spacingBetweenTier만큼 난다면 index 값을 증가시킴
            if (nodesByX[i].position.x - nodesByX[i - 1].position.x >= spacingBetweenTier)
                index++;

            indexField.SetValue(nodesByX[i], index);
        }
    }
}
