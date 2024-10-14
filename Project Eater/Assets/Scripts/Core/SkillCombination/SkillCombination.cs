using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCombination : IdentifiedObject
{
    [SerializeField, HideInInspector]
    private SkillCombinationGraph graph;

    // graph가 가진 SkillCombinationSlotNode들을 반환하는 함수 
    // → UI 쪽 Code에서 해당 함수를 이용해서 SkillCombination을 UI로 구성
    public SkillCombinationSlotNode[] GetSlotNodes()
        => graph.GetSlotNodes();
}
