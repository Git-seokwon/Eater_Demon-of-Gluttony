using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   
public class EffectStackAction 
{
    // 몇 Stack일 때, EffectStackAction이 적용될 것인가?
    [SerializeField, Min(1)]
    private int stack;

    // 다음 EffectStackAction이 적용될 때, 이전 EffectStackAction을 Release할 것인가? 
    [SerializeField]
    private bool isReleaseOnNextApply;

    // EffectStackAction을 딱 1번만 적용 시킬 것인가? 
    // → 해당 Option이 켜져 있으면, Effect의 Stack이 2가되면 EffectStackAction이 적용되고, 
    //    Stack이 3으로 증가되었다가 다시 2로 떨어져도, EffectStackAction이 이미 한번 적용되었기 때문에 다시 적용되지 않는다. 
    [SerializeField]
    private bool isApplyOnceInLifeTime;

    // 적용 효과
    [UnderlineTitle("Action")]
    [SerializeReference, SubclassSelector]
    private EffectAction action;

    // EffectStackAction이 적용된 적이 있는가? 
    private bool hasEverApplied;

    public int Stack => stack;
    public bool IsReleaseOnNextApply => isReleaseOnNextApply;

    // Action을 적용 가능한 상태인지 나타내는 변수 
    // → isAppliedOnceInLifeTime이 true라면 적용된 적이 없어야 적용 가능함
    public bool IsApplicable => !isApplyOnceInLifeTime || (isApplyOnceInLifeTime && !hasEverApplied);

    #region EffectAction 함수 Wrapping
    public void Start(Effect effect, Entity user, Entity target, int level, float scale)
        => action.Start(effect, user, target, level, scale);

    public void Apply(Effect effect, int level, Entity user, Entity target, float scale)
    {
        action.Apply(effect, user, target, level, stack, scale);
        // 적용 했음을 기록
        hasEverApplied = true;
    }

    public void Release(Effect effect, int level, Entity user, Entity target, float scale)
        => action.Release(effect, user, target, level, scale);

    public string BuildDescription(Effect effect, string baseDescription, int stackActionIndex, int effectIndex)
        // stack 인자로 StackAction에 설정된 Stack을 넣어준다.
        => action.BuildDescription(effect, baseDescription, stackActionIndex, stack, effectIndex); 
    #endregion
}