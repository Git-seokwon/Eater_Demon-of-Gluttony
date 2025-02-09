using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SeflDestructAction : SkillAction
{
    public override void Apply(Skill skill)
    {
        // Skill이 가진 효과를 Targets에게 적용
        foreach (var target in skill.Targets)
        {
            target.SkillSystem.Apply(skill);
        }
    }

    // 자폭 후처리 
    public override void Release(Skill skill)
    {
        skill.Owner.OnDead();
    }

    public override object Clone() => new SeflDestructAction();
}
