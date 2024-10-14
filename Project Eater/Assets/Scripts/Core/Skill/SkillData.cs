using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SkillData
{
    // ��ų level
    public int level;

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

    // Apply ���� �Ҵ� �� Skill Action
    public SkillApplyAction[] applyActions;

    // ��Ÿ�� 
    // �� '��ų ����' Stat�� ������ �ޱ� ������ StatScaleFloat �ڷ������� ������ 
    [UnderlineTitle("Cooldown")]
    public StatScaleFloat coolDown;

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
    // Charge Power�� ���� Scale�� ������ �� ���� 
    public bool isApplyRangeScale;
    // Charge Power�� Effect Scale�� ������ �� ���� 
    public bool isApplyEffectScale;

    [UnderlineTitle("Animation")]
    // AnimatorPrameter��
    public AnimatorParameter castAnimatorParameter;
    public AnimatorParameter chargeAnimatorParameter;

    // CustomAction�� 
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnCast;
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnCharge;
}
