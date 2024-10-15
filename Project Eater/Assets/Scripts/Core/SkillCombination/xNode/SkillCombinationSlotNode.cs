using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;
using XNode;
using static XNode.Node;

// NodeWidth: Graph에서 보여지지는 Node의 넓이
// NodeTint: Node의 RGB(255, 255, 255) Color
[NodeWidth(300), NodeTint(60, 60, 60)]
public class SkillCombinationSlotNode : XNode.Node
{
    // graph에서 몇 번째 열 or 행인지 여부
    [SerializeField]
    private int tier;

    // 현재 tier에서 몇번째 Slot인지 여부
    // ex. (tier=1, index=0), (tier=1, index=1), (tier=1, index=2) ...
    // tier와 index를 합쳐서 2차원 배열 형태임
    [SerializeField]
    private int index;

    // 고유 스킬 인지 여부, false라면 포식 스킬이다. 
    [SerializeField]
    private bool isInherent;

    // 이 Node가 가지고 있는 Skill
    [SerializeField]
    private Skill skill;
    // 각성 스킬 : '사신의 낫' 스킬과 같이 각성 스킬이 존재하는 경우에 할당, 없으면 null로 비워두기 
    [SerializeField]
    private Skill awakeningSkill;

    // Skill 습득을 위한 선행 Skill들과 Skill들의 Level을 받는 변수.
    // precedingLevels 자체는 int형 배열이라 필요한 Level 값만 받을 수 있지만,
    // CustomEditor를 통해서 추가되는 Element마다 Input Port를 할당해서 다른 선행 Skill Node가 연결될 수 있도록 할 것임.
    // 즉, Element를 추가하면 필요한 Level을 입력하고 Element에 할당된 Port에 선행 Skill Node를 연결해야 온전히 조건 입력이 완료되는 것
    [Input]
    [SerializeField]
    private int[] precedingLevels;

    // ※ Connection Type
    // → Override : Output Port와 Input Port를 연결할 때, 이미 Input Port와 연결된 Output Port가 있다면 그 Output Port와 연결을 끊고,
    //               새 Output Port와 연결하는 Option
    //             : 1대1 Matching, 기존 연결 여부와 상관없이 몇 개든 추가적으로 연결시킬 수 있는 Option이고 Default 값이기 때문에
    //               인자로 아무것도 넣지 않으면 이 Option이 사용
    // 다른 Node의 precdingLevels 변수에 연결할 현재 Node(this)
    // → Port만 사용할 것이기 때문에 HideInInspector로 변수 자체는 가려준다. 
    // → GetValue 함수로 thisNode라는 이름이 넘어오면 이 Node 자체를 Return 한다. 
    [Output(connectionType = ConnectionType.Override), HideInInspector]
    [SerializeField]
    private SkillCombinationSlotNode thisNode;

    public int Tier => tier;
    public int Index => index;
    public bool IsInherent => isInherent;
    public Skill Skill => skill;

    // Node가 만들어질 때 실행 
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

    // precedingLevels 변수의 Port들과 연결된 Slot들을 가져오는 함수 
    // → precedingLevels를 Select로 돌면서 각 Element의 Index 값을 GetPrecedingSlotNode 함수에 넣기 
    //    이후 연결된 Node를 가져온 다음 ToArray로 반환하기 
    // ※ Select : precedingLevels 배열을 순회하면서 각 요소(value)의 인덱스(index)를 사용해 해당 인덱스의 포트를
    //             기반으로 선행 노드를 찾아 낸다. 
    public SkillCombinationSlotNode[] GetPrecedingSlotNodes()
        => precedingLevels.Select((value, index) => GetPrecedingSlotNode(index)).ToArray();

    // precedingLevels의 Element에 할당된 Port에서 Port와 연결된 다른 Node를 찾아옴
    // Element에 할당된 Port를 찾아오는 Naming 규칙은 (변수 이름 + Element의 index)
    // ex. precedingLevels의 첫번째 Element에 할당된 Port를 찾아오고 싶다면 precedingLevels 0
    // Port에 연결된 Node가 없다면 null이 반환됨
    private SkillCombinationSlotNode GetPrecedingSlotNode(int index)
        => GetInputValue<SkillCombinationSlotNode>("precedingLevels " + index);

    // Skill의 습득 조건을 충족했는지 확인하는 함수 
    // ※ entity : Skill을 습득하려고 하는 Entity
    public bool IsSkillAcquirable(Entity entity)
    {
        // Entity가 선행 SKill들을 가지고 있고, 선행 Skill들이 Level을 충족했는지 확인
        for (int i = 0; i < precedingLevels.Length; i++)
        {
            var inputNode = GetPrecedingSlotNode(i);
            var entitySkill = entity.SkillSystem.FindOwnSkill(inputNode.Skill);

            if (entitySkill == null || entitySkill.Level < precedingLevels[i])
                return false;
        }

        return true;
    }

    // Slot이 가지고 있는 Skill을 인자로 받은 entity에게 등록해주는 함수 
    public Skill AcquireSkill(Entity entity)
    {
        Debug.Assert(IsSkillAcquirable(entity), "SkillTreeNode::AcquireSkill - Skill 습득 조건을 충족하지 못했습니다.");

        if (awakeningSkill != null)
            entity.SkillSystem.Register(awakeningSkill);

        return entity.SkillSystem.Register(skill);
    }
}
