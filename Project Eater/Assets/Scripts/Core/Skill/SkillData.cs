using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SkillData
{
    // 스킬 level
    public int level;

    // ※ PrecedingAction : Skill이 실제 사용되기 전 먼저 실행할 Action (사전 동작)
    // → 아무 효과 없이 어떤 동작을 수행하기 위해 존재 (사전 동작이 없는 Skill은 해당 값이 null)
    // Ex) 상대방에게 달려감, 구르기를 함, Jump를 함 등
    [UnderlineTitle("Preceding Action")]
    [SerializeReference, SubclassSelector]
    public SkillPrecedingAction precedingAction;

    [UnderlineTitle("Action")]
    [SerializeReference, SubclassSelector]
    public SkillAction action;

    // 스킬을 언제 끝낼 지를 나타내는 Option
    [UnderlineTitle("Setting")]
    public SkillRunningFinishOption runningFinishOption;

    // ※ Skill 지속 시간
    // → runningFinishOption이 FinishWhenDurationEnded이고, duration이 0이면 무한 지속 
    [Min(0f)]
    public float duration;

    // ※ Skill이 적용될 횟수 
    // → applyCount가 0이면 무한 적용 
    [Min(0)]
    public int applyCount;

    // ※ ApplyCount가 1보다 클 때, Apply를 실행할 주기
    // → 첫 한번은 효과가 바로 적용될 것이기 때문에, 한번 적용된 후부터 ApplyCycle 시간이 지나고 적용됨
    //    예를 들어서, ApplyCycle이 1초라면, 바로 한번 적용된 후 1초마다 적용되게 됨.
    [Min(0f)]
    public float applyCycle;

    // 쿨타임 
    // → '스킬 가속' Stat의 영향을 받기 때문에 StatScaleFloat 자료형으로 선언함 
    public StatScaleFloat coolDown;

    // Skill의 적용 대상을 찾기 위한 Module
    [UnderlineTitle("Target Searcher")]
    public TargetSearcher targetSearcher;

    // ★ Casting과 Charging 둘 중 하나만 선택이 가능하도록 설정 
    // → Casting도 하면서 Charge 까지 하는 스킬은 없음 

    // 스킬 Casting
    [UnderlineTitle("Cast")]
    public bool isUseCast;
    // Casting 시간
    public StatScaleFloat castTime;

    // 스킬 Charget
    [UnderlineTitle("Charge")]
    public bool isUseCharge;
    // Charge의 지속 시간이 끝났을 때, 어떻게 행동할 것인가
    public SkillChargeFinishActionOption chargeFinishActionOption;
    // Charge의 지속 시간
    [Min(0f)]
    public float chargeDuration;
    // Full Charge까지 걸리는 시간
    [Min(0f)]
    public float chargeTime;
    // Skill을 사용하기 위해 필요한 최소 충전 시간
    [Min(0f)]
    public float needChargeTimeToUse;
    // Charge의 시작 Power
    [Range(0f, 1f)]
    public float startChargePower;

    // 스킬의 효과들
    [UnderlineTitle("Effect")]
    public EffectSelector[] effectSelectors;

    // Entity의 InSkillActionState를 언제 끝낼 지를 나타내는 Option
    [UnderlineTitle("Animation")]
    public InSkillActionFinishOption inSkillActionFinishOption;

    // AnimatorPrameter들
    public AnimatorParameter castAnimatorParameter;
    public AnimatorParameter chargeAnimatorParameter;
    public AnimatorParameter precedingActionAnimatorParameter;
    public AnimatorParameter actionAnimatorParameter;

    // CustomAction들 
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnCast;
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnCharge;
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnPrecedingAction;
    [SerializeReference, SubclassSelector]
    public CustomAction[] customActionsOnAction;
}
