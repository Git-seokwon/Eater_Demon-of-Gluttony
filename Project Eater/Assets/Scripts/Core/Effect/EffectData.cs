using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Effect�� Level�� Data�� ��� �ִ� ����ü 
[System.Serializable]
public struct EffectData 
{
    // ���� Data�� �� Level Data����
    public int level;

    [UnderlineTitle("Stack")]
    [Min(1)]
    // �ִ� Stack
    public int maxStack;
    // �� EffectStackAction : Ư�� Stack�� ��, ������ ȿ���� ��� �ִ� Class
    // Stack�� ���� �߰� ȿ���� (Stack�� ȿ������ ��� �ִ� �迭)
    public EffectStackAction[] stackActions;

    [UnderlineTitle("Action")]
    [SerializeReference, SubclassSelector]
    // �� EffectAction : Effect�� ���� ȿ���� ����ϴ� Module
    // �ش� Class�� ��ӹ޾� �ʿ��� ȿ���� �����Ѵ�. 
    // Ex) ����, ġ��, ���� ���� ���� ȿ��
    public EffectAction action;

    [UnderlineTitle("Setting")]
    // Effect�� �Ϸ��� ����
    public EffectRunningFinishOption runningFinishOption;

    // Effect�� ���� �ð��� ����Ǿ��� ��, ���� ���� Ƚ���� �ִٸ� ��� ������ ������ ����
    // �� ��ų ���� �ð��� ������ ��, ����� ��ų�� ���ӽ�ų��, �ƴϸ� ��� ������ �� ���� 
    public bool isApplyAllWhenDurationExpires;

    // Effect�� ���ӽð�, StatScaleFloat�� �����Ͽ� Ư�� Stat(��ų ����)�� ���� ���� �ð��� �ø��ų� ���� �� �ִ�. 
    public StatScaleFloat duration;

    // Effect�� ������ Ƚ��
    // �� applyCount ���� 0�̸� �� Frame���� ����
    [Min(0)]
    public int applyCount;

    // Effect�� ������ �ֱ� 
    // �� ù��° Ƚ���� ȿ���� �ٷ� ����� ���̱� ������, �ѹ� ����� �ĺ��� ApplyCycle�� ���� ����
    // Ex) applyCount�� 0�̰� applyCycle�� 1�̸�, ó�� �ٷ� ������ �ǰ�, 1�� ���� Effect�� ��� ����ǰ� �ȴ�.
    [Min(0)]
    public float applyCycle;
    public EffectStartDelayByApplyCycle startDelayByApplyCycle;

    [UnderlineTitle("Custom Action")]
    [SerializeReference, SubclassSelector]
    // Effect�� �پ��� ������ �ֱ����� Module
    // Ex) Sound ���, ��ų ����Ʈ ���, Camera Shake ��
    public CustomAction[] customActions;
}
