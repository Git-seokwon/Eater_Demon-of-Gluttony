using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Effect의 Level별 Data를 담고 있는 구조체 
[System.Serializable]
public struct EffectData 
{
    // 현재 Data가 몇 Level Data인지
    public int level;

    [UnderlineTitle("Stack")]
    [Min(1)]
    // 최대 Stack
    public int maxStack;
    // ※ EffectStackAction : 특정 Stack일 때, 적용할 효과를 담고 있는 Class
    // Stack에 따른 추가 효과들 (Stack별 효과들을 담고 있는 배열)
    public EffectStackAction[] stackActions;

    [UnderlineTitle("Action")]
    [SerializeReference, SubclassSelector]
    // ※ EffectAction : Effect의 실제 효과를 담당하는 Module
    // 해당 Class를 상속받아 필요한 효과를 구현한다. 
    // Ex) 공격, 치유, 버프 같은 실제 효과
    public EffectAction action;

    [UnderlineTitle("Setting")]
    // Effect를 완료할 시점
    public EffectRunningFinishOption runningFinishOption;

    // Effect의 지속 시간이 만료되었을 때, 남은 적용 횟수가 있다면 모두 적용할 것인지 여부
    // → 스킬 지속 시간이 끝났을 때, 적용된 스킬을 지속시킬지, 아니면 모두 삭제할 지 여부 
    public bool isApplyAllWhenDurationExpires;

    // Effect의 지속시간, StatScaleFloat로 선언하여 특정 Stat(스킬 가속)을 통해 지속 시간을 늘리거나 줄일 수 있다. 
    public StatScaleFloat duration;

    // Effect를 적용할 횟수
    // → applyCount 값이 0이면 매 Frame마다 적용
    [Min(0)]
    public int applyCount;

    // Effect를 적용할 주기 
    // → 첫번째 횟수는 효과가 바로 적용될 것이기 때문에, 한번 적용된 후부터 ApplyCycle에 따라 적용
    // Ex) applyCount가 0이고 applyCycle이 1이면, 처음 바로 적용이 되고, 1초 마다 Effect가 계속 적용되게 된다.
    [Min(0)]
    public float applyCycle;
    public EffectStartDelayByApplyCycle startDelayByApplyCycle;

    [UnderlineTitle("Custom Action")]
    [SerializeReference, SubclassSelector]
    // Effect에 다양한 연출을 주기위한 Module
    // Ex) Sound 출력, 스킬 이펙트 출력, Camera Shake 등
    public CustomAction[] customActions;
}
