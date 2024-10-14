using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InstantApplyAndReturnAction : SkillAction
{
    public override void Apply(Skill skill)
    {
        // Skill이 가진 효과를 Targets에게 적용
        foreach (var target in skill.Targets)
        {
            target.SkillSystem.Apply(skill);
        }
    }

    public override void Release(Skill skill)
    {
        var owner = skill.Owner as PlayerEntity;
        var skillKeyNumber = skill.skillKeyNumber;

        owner.SkillSystem.Disarm(skill, skillKeyNumber);
        var originalSkill = owner.SkillSystem.FindOwnSkill(x => x.CodeName == "DEATHSCYTHE");
       owner.SkillSystem.Equip(originalSkill, skillKeyNumber);
    }

    public override object Clone()
    {
        return new InstantApplyAndReturnAction()
        {
        };
    }
}
