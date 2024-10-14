using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InstantApplyAndEvolvingByStackAction : SkillAction
{
    [SerializeField]
    private int evolvingStackCount;

    public override void Apply(Skill skill)
    {
        // Skill이 가진 효과를 Targets에게 적용
        foreach (var target in skill.Targets)
        {
            target.SkillSystem.Apply(skill);
        }
    }

    // 스택을 소모하고 현재 스킬을 각성 스킬로 변경한다. 
    public override void Release(Skill skill)
    {
        if ((skill.Owner as PlayerEntity).CurrentStackCount >= evolvingStackCount)
        {
            (skill.Owner as PlayerEntity).CurrentStackCount -= evolvingStackCount;

            var owner = skill.Owner as PlayerEntity;
            var skillKeyNumber = skill.skillKeyNumber;

            skill.Owner.SkillSystem.Disarm(skill, skillKeyNumber);
            var evolveSkill = owner.SkillSystem.FindOwnSkill(x => x.CodeName == "DEATHSCYTHE_EVOLVE");
            owner.SkillSystem.Equip(evolveSkill, skillKeyNumber);
        }
    }

    protected override IReadOnlyDictionary<string, string> GetStringByKeyword()
    {
        var descriptionValueByKeyword = new Dictionary<string, string>()
        {
            ["evolvingStackCount"] = evolvingStackCount.ToString(),
        };

        return descriptionValueByKeyword;
    }

    public override object Clone()
    {
        return new InstantApplyAndEvolvingByStackAction()
        {
            evolvingStackCount = evolvingStackCount
        };
    }
}
