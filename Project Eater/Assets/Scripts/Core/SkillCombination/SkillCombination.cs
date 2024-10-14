using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCombination : IdentifiedObject
{
    [SerializeField, HideInInspector]
    private SkillCombinationGraph graph;

    // graph�� ���� SkillCombinationSlotNode���� ��ȯ�ϴ� �Լ� 
    // �� UI �� Code���� �ش� �Լ��� �̿��ؼ� SkillCombination�� UI�� ����
    public SkillCombinationSlotNode[] GetSlotNodes()
        => graph.GetSlotNodes();
}
