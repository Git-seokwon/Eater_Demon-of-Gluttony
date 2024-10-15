using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using XNode;
using static XNode.Node;

// NodeWidth: Graph���� ���������� Node�� ����
// NodeTint: Node�� RGB(255, 255, 255) Color
[NodeWidth(300), NodeTint(60, 60, 60)]
public class SkillCombinationSlotNode : XNode.Node
{
    // graph���� �� ��° �� or ������ ����
    [SerializeField]
    private int tier;

    // ���� tier���� ���° Slot���� ����
    // ex. (tier=1, index=0), (tier=1, index=1), (tier=1, index=2) ...
    // tier�� index�� ���ļ� 2���� �迭 ������
    [SerializeField]
    private int index;

    // ���� ��ų ���� ����, false��� ���� ��ų�̴�. 
    [SerializeField]
    private bool isInherent;

    // �� Node�� ������ �ִ� Skill
    [SerializeField]
    private Skill skill;
    // ���� ��ų : '����� ��' ��ų�� ���� ���� ��ų�� �����ϴ� ��쿡 �Ҵ�, ������ null�� ����α� 
    [SerializeField]
    private Skill awakeningSkill;

    // Skill ������ ���� ���� Skill��� Skill���� Level�� �޴� ����.
    // precedingLevels ��ü�� int�� �迭�̶� �ʿ��� Level ���� ���� �� ������,
    // CustomEditor�� ���ؼ� �߰��Ǵ� Element���� Input Port�� �Ҵ��ؼ� �ٸ� ���� Skill Node�� ����� �� �ֵ��� �� ����.
    // ��, Element�� �߰��ϸ� �ʿ��� Level�� �Է��ϰ� Element�� �Ҵ�� Port�� ���� Skill Node�� �����ؾ� ������ ���� �Է��� �Ϸ�Ǵ� ��
    [Input]
    [SerializeField]
    private int[] precedingLevels;

    // �� Connection Type
    // �� Override : Output Port�� Input Port�� ������ ��, �̹� Input Port�� ����� Output Port�� �ִٸ� �� Output Port�� ������ ����,
    //               �� Output Port�� �����ϴ� Option
    //             : 1��1 Matching, ���� ���� ���ο� ������� �� ���� �߰������� �����ų �� �ִ� Option�̰� Default ���̱� ������
    //               ���ڷ� �ƹ��͵� ���� ������ �� Option�� ���
    // �ٸ� Node�� precdingLevels ������ ������ ���� Node(this)
    // �� Port�� ����� ���̱� ������ HideInInspector�� ���� ��ü�� �����ش�. 
    // �� GetValue �Լ��� thisNode��� �̸��� �Ѿ���� �� Node ��ü�� Return �Ѵ�. 
    [Output(connectionType = ConnectionType.Override), HideInInspector]
    [SerializeField]
    private SkillCombinationSlotNode thisNode;

    public int Tier => tier;
    public int Index => index;
    public bool IsInherent => isInherent;
    public Skill Skill => skill;

    // Node�� ������� �� ���� 
    protected override void Init()
    {
        thisNode = this;
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName != "thisNode")
            return null;
        return thisNode;
    }

    // precedingLevels ������ Port��� ����� Slot���� �������� �Լ� 
    // �� precedingLevels�� Select�� ���鼭 �� Element�� Index ���� GetPrecedingSlotNode �Լ��� �ֱ� 
    //    ���� ����� Node�� ������ ���� ToArray�� ��ȯ�ϱ� 
    // �� Select : precedingLevels �迭�� ��ȸ�ϸ鼭 �� ���(value)�� �ε���(index)�� ����� �ش� �ε����� ��Ʈ��
    //             ������� ���� ��带 ã�� ����. 
    public SkillCombinationSlotNode[] GetPrecedingSlotNodes()
        => precedingLevels.Select((value, index) => GetPrecedingSlotNode(index)).ToArray();

    // precedingLevels�� Element�� �Ҵ�� Port���� Port�� ����� �ٸ� Node�� ã�ƿ�
    // Element�� �Ҵ�� Port�� ã�ƿ��� Naming ��Ģ�� (���� �̸� + Element�� index)
    // ex. precedingLevels�� ù��° Element�� �Ҵ�� Port�� ã�ƿ��� �ʹٸ� precedingLevels 0
    // Port�� ����� Node�� ���ٸ� null�� ��ȯ��
    private SkillCombinationSlotNode GetPrecedingSlotNode(int index)
        => GetInputValue<SkillCombinationSlotNode>("precedingLevels " + index);

    // Skill�� ���� ������ �����ߴ��� Ȯ���ϴ� �Լ� 
    // �� entity : Skill�� �����Ϸ��� �ϴ� Entity
    public bool IsSkillAcquirable(Entity entity)
    {
        // Entity�� ���� SKill���� ������ �ְ�, ���� Skill���� Level�� �����ߴ��� Ȯ��
        for (int i = 0; i < precedingLevels.Length; i++)
        {
            var inputNode = GetPrecedingSlotNode(i);
            var entitySkill = entity.SkillSystem.FindOwnSkill(inputNode.Skill);

            if (entitySkill == null || entitySkill.Level < precedingLevels[i])
                return false;
        }

        return true;
    }

    // Slot�� ������ �ִ� Skill�� ���ڷ� ���� entity���� ������ִ� �Լ� 
    public Skill AcquireSkill(Entity entity)
    {
        Debug.Assert(IsSkillAcquirable(entity), "SkillTreeNode::AcquireSkill - Skill ���� ������ �������� ���߽��ϴ�.");

        if (awakeningSkill != null)
            entity.SkillSystem.Register(awakeningSkill);

        return entity.SkillSystem.Register(skill);
    }
}
