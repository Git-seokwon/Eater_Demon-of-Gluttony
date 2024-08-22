using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SkillData
{
    // ��ų level
    public int level;

    // �� PrecedingAction : Skill�� ���� ���Ǳ� �� ���� ������ Action (���� ����)
    // �� �ƹ� ȿ�� ���� � ������ �����ϱ� ���� ���� (���� ������ ���� Skill�� �ش� ���� null)
    // Ex) ���濡�� �޷���, �����⸦ ��, Jump�� �� ��
    [UnderlineTitle("Preceding Action")]
    [SerializeReference, SubclassSelector]
    public SkillPrecedingAction precedingAction;

    [UnderlineTitle("Action")]
    [SerializeReference, SubclassSelector]
    public SkillAction action;

    // ��ų�� ���� ���� ���� ��Ÿ���� Option
    [UnderlineTitle("Setting")]
    public SkillRunningFinishOption runningFinishOption;

    // �� Skill ���� �ð�
    // �� runningFinishOption�� FinishWhenDurationEnded�̰�, duration�� 0�̸� ���� ���� 
    [Min(0f)]
    public float duration;

    // �� Skill�� ����� Ƚ�� 
    // �� applyCount�� 0�̸� ���� ���� 
    [Min(0)]
    public int applyCount;

    // �� ApplyCount�� 1���� Ŭ ��, Apply�� ������ �ֱ�
    // �� ù �ѹ��� ȿ���� �ٷ� ����� ���̱� ������, �ѹ� ����� �ĺ��� ApplyCycle �ð��� ������ �����
    //    ���� ��, ApplyCycle�� 1�ʶ��, �ٷ� �ѹ� ����� �� 1�ʸ��� ����ǰ� ��.
    [Min(0f)]
    public float applyCycle;

    // ��Ÿ�� 
    // �� '��ų ����' Stat�� ������ �ޱ� ������ StatScaleFloat �ڷ������� ������ 
    public StatScaleFloat coolDown;

    // Skill�� ���� ����� ã�� ���� Module
    [UnderlineTitle("Target Searcher")]
    public TargetSearcher targetSearcher;

    // �� Casting�� Charging �� �� �ϳ��� ������ �����ϵ��� ���� 
    // �� Casting�� �ϸ鼭 Charge ���� �ϴ� ��ų�� ���� 

    // ��ų Casting
    [UnderlineTitle("Cast")]
    public bool isUseCast;
    // Casting �ð�
    public StatScaleFloat castTime;

    // ��ų Charget
    [UnderlineTitle("Charge")]
    public bool isUseCharge;
    // Charge�� ���� �ð��� ������ ��, ��� �ൿ�� ���ΰ�
    public SkillChargeFinishActionOption chargeFinishActionOption;
    // Charge�� ���� �ð�
    [Min(0f)]
    public float chargeDuration;
    // Full Charge���� �ɸ��� �ð�
    [Min(0f)]
    public float chargeTime;
    // Skill�� ����ϱ� ���� �ʿ��� �ּ� ���� �ð�
    [Min(0f)]
    public float needChargeTimeToUse;
    // Charge�� ���� Power
    [Range(0f, 1f)]
    public float startChargePower;

    // ��ų�� ȿ����
    [UnderlineTitle("Effect")]
    public EffectSelector[] effectSelectors;

    // Entity�� InSkillActionState�� ���� ���� ���� ��Ÿ���� Option
    [UnderlineTitle("Animation")]
    public InSkillActionFinishOption inSkillActionFinishOption;

    // AnimatorPrameter��
    public AnimatorParameter castAnimatorParameter;
    public AnimatorParameter chargeAnimatorParameter;
    public AnimatorParameter precedingActionAnimatorParameter;
    public AnimatorParameter actionAnimatorParameter;

    // CustomAction�� 
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnCast;
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnCharge;
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnPrecedingAction;
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnAction;
}
