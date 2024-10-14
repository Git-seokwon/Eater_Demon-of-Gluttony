using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SkillApplyAction
{
    // 필요한 TargetSearcher 검색 결과 : Target or Position
    public NeedSelectionResultType needSelectionResultType;

    // 즉시 적용 or Animatoin 적용
    public SkillApplyType applyType; 

    [Min(1)]
    public int currentApplyCount;

    // ※ PrecedingAction : Skill이 실제 사용되기 전 먼저 실행할 Action (사전 동작)
    // → 아무 효과 없이 어떤 동작을 수행하기 위해 존재 (사전 동작이 없는 Skill은 해당 값이 null)
    // Ex) 상대방에게 달려감, 구르기를 함, Jump를 함 등
    [UnderlineTitle("Preceding Action")]
    [SerializeReference, SubclassSelector]
    public SkillPrecedingAction precedingAction;

    [UnderlineTitle("Action")]
    [SerializeReference, SubclassSelector]
    public SkillAction action;

    // Skill의 적용 대상을 찾기 위한 Module
    [UnderlineTitle("Target Searcher")]
    public TargetSearcher targetSearcher;

    // 스킬의 효과들
    [UnderlineTitle("Effect")]
    public EffectSelector[] effectSelectors;

    // Entity의 InSkillActionState를 언제 끝낼 지를 나타내는 Option
    [UnderlineTitle("Animation")]
    public InSkillActionFinishOption inSkillActionFinishOption;

    // AnimatorPrameter들
    public AnimatorParameter precedingActionAnimatorParameter;
    public AnimatorParameter actionAnimatorParameter;

    // CustomAction들 
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnPrecedingAction;
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnAction;
}
