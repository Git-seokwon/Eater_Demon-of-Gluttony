using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// SlotNode���� ������ SkillCombinationGraph
[CreateAssetMenu(fileName = "Skill Combination", menuName = "Skill/Skill Combination")]
public class SkillCombinationGraph : XNode.NodeGraph
{
    // Graph�� ���� SlotNode���� ��ȯ�ϴ� GetSlotNode �Լ�
    // �� Graph�� ���� Node�� �߿��� null�� �ƴ� Node���� ������ SkillTreeSlotNode Type���� Casting�Ͽ� ��ȯ
    // �� XNode�� Graph�� �ٸ� Asset�� ���� Asset���� �߰��ϸ� null�� Element �ϳ��� �ڵ����� ���ܳ��� Bug�� �߻��ϱ� ������
    //    where�� null�� �ƴ� Node���� �������� �ϴ� ���̴�. 
    public SkillCombinationSlotNode[] GetSlotNodes()
        => nodes.Where(x => x != null).Cast<SkillCombinationSlotNode>().ToArray();
}
