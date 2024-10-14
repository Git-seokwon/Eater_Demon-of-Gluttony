using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// SlotNode들을 관리할 SkillCombinationGraph
[CreateAssetMenu(fileName = "Skill Combination", menuName = "Skill/Skill Combination")]
public class SkillCombinationGraph : XNode.NodeGraph
{
    // Graph가 가진 SlotNode들을 반환하는 GetSlotNode 함수
    // → Graph가 가진 Node들 중에서 null이 아닌 Node들을 가져와 SkillTreeSlotNode Type으로 Casting하여 반환
    // → XNode의 Graph를 다른 Asset에 하위 Asset으로 추가하면 null인 Element 하나가 자동으로 생겨나는 Bug가 발생하기 때문에
    //    where로 null이 아닌 Node들을 가져오게 하는 것이다. 
    public SkillCombinationSlotNode[] GetSlotNodes()
        => nodes.Where(x => x != null).Cast<SkillCombinationSlotNode>().ToArray();
}
