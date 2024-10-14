using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SkillApplyAction
{
    // �ʿ��� TargetSearcher �˻� ��� : Target or Position
    public NeedSelectionResultType needSelectionResultType;

    // ��� ���� or Animatoin ����
    public SkillApplyType applyType; 

    [Min(1)]
    public int currentApplyCount;

    // �� PrecedingAction : Skill�� ���� ���Ǳ� �� ���� ������ Action (���� ����)
    // �� �ƹ� ȿ�� ���� � ������ �����ϱ� ���� ���� (���� ������ ���� Skill�� �ش� ���� null)
    // Ex) ���濡�� �޷���, �����⸦ ��, Jump�� �� ��
    [UnderlineTitle("Preceding Action")]
    [SerializeReference, SubclassSelector]
    public SkillPrecedingAction precedingAction;

    [UnderlineTitle("Action")]
    [SerializeReference, SubclassSelector]
    public SkillAction action;

    // Skill�� ���� ����� ã�� ���� Module
    [UnderlineTitle("Target Searcher")]
    public TargetSearcher targetSearcher;

    // ��ų�� ȿ����
    [UnderlineTitle("Effect")]
    public EffectSelector[] effectSelectors;

    // Entity�� InSkillActionState�� ���� ���� ���� ��Ÿ���� Option
    [UnderlineTitle("Animation")]
    public InSkillActionFinishOption inSkillActionFinishOption;

    // AnimatorPrameter��
    public AnimatorParameter precedingActionAnimatorParameter;
    public AnimatorParameter actionAnimatorParameter;

    // CustomAction�� 
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnPrecedingAction;
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnAction;
}
