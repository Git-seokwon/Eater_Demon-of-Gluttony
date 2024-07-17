using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   
public class EffectStackAction 
{
    // �� Stack�� ��, EffectStackAction�� ����� ���ΰ�?
    [SerializeField, Min(1)]
    private int stack;

    // ���� EffectStackAction�� ����� ��, ���� EffectStackAction�� Release�� ���ΰ�? 
    [SerializeField]
    private bool isReleaseOnNextApply;

    // EffectStackAction�� �� 1���� ���� ��ų ���ΰ�? 
    // �� �ش� Option�� ���� ������, Effect�� Stack�� 2���Ǹ� EffectStackAction�� ����ǰ�, 
    //    Stack�� 3���� �����Ǿ��ٰ� �ٽ� 2�� ��������, EffectStackAction�� �̹� �ѹ� ����Ǿ��� ������ �ٽ� ������� �ʴ´�. 
    [SerializeField]
    private bool isApplyOnceInLifeTime;

    // ���� ȿ��
    [UnderlineTitle("Action")]
    [SerializeReference, SubclassSelector]
    private EffectAction action;

    // EffectStackAction�� ����� ���� �ִ°�? 
    private bool hasEverApplied;

    public int Stack => stack;
    public bool IsReleaseOnNextApply => isReleaseOnNextApply;

    // Action�� ���� ������ �������� ��Ÿ���� ���� 
    // �� isAppliedOnceInLifeTime�� true��� ����� ���� ����� ���� ������
    public bool IsApplicable => !isApplyOnceInLifeTime || (isApplyOnceInLifeTime && !hasEverApplied);

    #region EffectAction �Լ� Wrapping
    public void Start(Effect effect, Entity user, Entity target, int level, float scale)
        => action.Start(effect, user, target, level, scale);

    public void Apply(Effect effect, int level, Entity user, Entity target, float scale)
    {
        action.Apply(effect, user, target, level, stack, scale);
        // ���� ������ ���
        hasEverApplied = true;
    }

    public void Release(Effect effect, int level, Entity user, Entity target, float scale)
        => action.Release(effect, user, target, level, scale);

    public string BuildDescription(Effect effect, string baseDescription, int stackActionIndex, int effectIndex)
        // stack ���ڷ� StackAction�� ������ Stack�� �־��ش�.
        => action.BuildDescription(effect, baseDescription, stackActionIndex, stack, effectIndex); 
    #endregion
}