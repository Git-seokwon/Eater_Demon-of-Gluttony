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

    public override bool IsPass(Skill skill)
    {
        Entity player = skill.Owner;
        Stats stats = player.GetComponent<Stats>();
        Stat fullnessStat = stats.GetStat(stat);

        float maxValue = fullnessStat.MaxValue;
        float value = fullnessStat.Value;
        float currentPercentage = value / maxValue;

        // 특정 스텟이 요구치를 만족하지 않음
        // ※ Mathf.Epsilon을 이용해 약간의 오차를 허용하는 방식
        if (!(currentPercentage + Mathf.Epsilon >= percentage))
        {
            foreach (var effect in skill.currentEffects)
            {
                player.SkillSystem.RemoveEffect(effect);
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
