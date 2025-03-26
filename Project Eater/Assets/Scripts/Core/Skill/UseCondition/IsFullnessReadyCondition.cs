using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IsFullnessReadyCondition : SkillCondition
{
    [SerializeField]
    private float percentage;
    [SerializeField]
    private Stat stat;
    [SerializeField]
    private Effect[] removeEffects; 

    public override bool IsPass(Skill skill)
    {
        Entity player = skill.Owner;
        Stats stats = player.GetComponent<Stats>();
        Stat fullnessStat = stats.GetStat(stat);

        float maxValue = fullnessStat.MaxValue;
        float value = fullnessStat.DefaultValue;
        float currentPercentage = value / maxValue;

        // Ư�� ������ �䱸ġ�� �������� ����
        // �� �ش� effect�� ID�� ���� ��� effect ȿ�� ���� 
        if (currentPercentage < percentage - Mathf.Epsilon)
        {
            foreach (var effect in removeEffects)
            {
                player.SkillSystem.RemoveEffectAll(effect);
            }
            return false;
        }

        return true;
    }

    public override object Clone()
    {
        return new IsFullnessReadyCondition()
        {
            percentage = percentage,
            stat = stat
        };
    }
}
