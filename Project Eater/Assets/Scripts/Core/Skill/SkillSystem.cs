using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class SkillSystem : MonoBehaviour
{
    // �ʱ�ȭ �Լ� : SkillSystem�� �����ڸ� Setting 
    public void Setup(Entity entity)
    {

    }

    // Effect�� Owner���� �����Ű�� �Լ� 
    // �� �̹� ����� Effect�� �ִٸ�, Effect Option�� ���� �ٸ� �۾� Ȥ�� �߰����� �۾��� �Ѵ�. 
    // �� ApplyNewEffect �Լ��� private �Լ��̰� Apply �Լ��� public �Լ��̱� ������ �⺻������ Effect�� �����ų ���� �� �Լ��� ����. 
    public void Apply(Effect effect)
    {

    }

    // Effect List�� ���ڷ� �޴� Apply Overloading �Լ� 
    public void Apply(IReadOnlyList<Effect> effects)
    {
        foreach (var effect in effects)
            Apply(effect);
    }

    // Skill�� ���ڷ� �޴� Apply Overloading �Լ� 
    public void Apply(Skill skill)
    {
        Apply(skill.Effects);
    }

    // ���� ���� ��� Skill�� ������ִ� �Լ� 
    // 1) isForce : ������ ��ų�� ����� �� ���� 
    public void CancleAll(bool isForce = false)
    {

    }
}
