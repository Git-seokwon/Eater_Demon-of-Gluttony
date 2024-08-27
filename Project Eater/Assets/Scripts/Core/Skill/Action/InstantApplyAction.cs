using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 다른 동작 없이 Skill이 TargetSearcher로 검색한 Target들에게 즉시 Skill의 효과를 적용 시키는 Module
[System.Serializable]
public class InstantApplyAction : SkillAction
{
    public override void Apply(Skill skill)
    {
        // Skill이 가진 효과를 Targets에게 적용
        foreach (var target in skill.Targets)
            target.SkillSystem.Apply(skill);
    }

    public override object Clone() => new InstantApplyAction();
}
