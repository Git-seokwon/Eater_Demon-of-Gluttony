using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class EffectAction : ICloneable // Clone �Լ��� Module�� �����ϱ� ���ؼ� ICloneable �������̽��� ��� 
{
    // Effect�� ���۵� ��, ȣ��Ǵ� ���� �Լ� 
    // �� �ʿ��� ���� �����ϸ� �Ǳ� ������ ���� �Լ��� ����
    // �� effect : Action�� �����ϰ� �ִ� Effect 
    // �� user : Effect�� ����� Entity
    // �� target : Effect�� ����� Entity
    // �� level : Effect�� level
    // �� scale : Effect�� ������ �����ϴ� �뵵 �� �ַ�, ���� �ð� �����ؼ� ���� Charge Skill�� ���
    public virtual void Start(Effect effect, Entity user, Entity target, int level, float scale) { }

    // ���� Effect�� ȿ���� �����ϴ� �Լ� 
    // �� stack : Effect�� ���� Stack �� 
    public abstract bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale);

    // Effect�� ����� ��, ȣ��Ǵ� ���� �Լ� 
    // �� �ʿ��� ���� �����ϸ� �Ǳ� ������ ���� �Լ��� ����
    public virtual void Release(Effect effect, Entity user, Entity target, int level, float scale) { }

    // Effect�� Stack�� �ٲ���� ��, ȣ��Ǵ� �Լ� 
    // �� Stack�� �ٲ���� ��, �� �۾��� �ۼ��ϸ� �ȴ�. 
    // ex) Stack���� �� 10�� ���������ִ� ȿ�����, ���ο� Stack ���� ���� ������Ų ���� �������ָ� �ȴ�. 
    // Stack���� Bonus ���� �ִ� Action�� ���, �� �Լ��� ���ؼ� Bonus ���� ���� ������ �� �ִ�. 
    public virtual void OnEffectStackChanged(Effect effect, Entity user, Entity target, int level, int stack, float scale) { }

    protected virtual IReadOnlyDictionary<string, string> GetStringByKeyword(Effect effect) => null;

    // Effect�� ������ Description Text�� GetStringByKeyword �Լ��� ���� ���� Dictionary�� Replace �۾��� �ϴ� �Լ� 
    // �� effect : Action�� ������ Effect
    // �� description : Replace�� ������ Text
    // �� stackActionIndex : Stack���� ������ ȿ���� ���� ���� ���, �迭ó�� �� ȿ������ Index ��ȣ(stackActionIndex)�� �ο�
    // �� stack : Action�� �� Stack�� Action���� ��Ÿ���� ���� 
    // �� Stack�� ��ų�� �ƴ� ���, stack ���� 0�� �ȴ�. 
    // �� effectIndex : Skill�� ���� ���� Effect �߿� �� Action�� ������ Effect�� �� ��° Effect ������ ��Ÿ����.
    public string BuildDescription(Effect effect, string description, int skillIndex, int stackActionIndex, int stack, int effectIndex)
    {
        // Replace Data�� �� Dictionary�� �������� 
        var stringByKeyword = GetStringByKeyword(effect);
        // Replace Data�� ������ description�� �״�� ��� 
        if (stringByKeyword == null)
            return description;

        // Stack�� ��ų�� �ƴ� ��� 
        if (stack == 0)
            // �� prefix : "effectAction"
            // �� suffix : Effect�� Index �� (Skill���� �� ��° Index�ΰ�?)
            // Ex) description = "������ $[EffectAction.defaultDamage.0] ���ظ� �ݴϴ�."
            // defaultDamage = 300, effectIndex = 0 �� stringsByKeyword = new() { { "defaultDamage", defaultDamage.ToString() } };
            //                                                                            KEY                   VALUE
            // description.Replace("$[EffectAction.defaultDamage.0]", "300") => "������ 300 ���ظ� �ݴϴ�."
            description = TextReplacer.Replace(description, skillIndex + ".effectAction", stringByKeyword, effectIndex.ToString());
        else
            // Ex) Mark = $[0.effectAction.defaultDamage.StackActionIndex.Stack.EffectIndex]
            description = TextReplacer.Replace(description, skillIndex + ".effectAction", stringByKeyword, $"{stackActionIndex}.{stack}.{effectIndex}");

        return description;
    }

    // �� Prototype Pattern : �ڽ� ��ü�� �θ� Class Type���� Upcasting�� ���¿��� ���� �ڷ����� ���� ���纻�� ������ �� �ִ�.
    // �� EffecAction�� ��ӹ޴� �ڽ� Class ��ü�� ���纻�� ����� return ���ִ� �Լ� 
    public abstract object Clone(); 
}
