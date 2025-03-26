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

        // 특정 스텟이 요구치를 만족하지 않음
        // → 해당 effect의 ID를 가진 모든 effect 효과 제거 
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
