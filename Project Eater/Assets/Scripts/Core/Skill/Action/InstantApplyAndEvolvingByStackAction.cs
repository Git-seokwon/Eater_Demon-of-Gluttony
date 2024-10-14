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
        // Skill�� ���� ȿ���� Targets���� ����
        foreach (var target in skill.Targets)
        {
            target.SkillSystem.Apply(skill);
        }
    }

    // ������ �Ҹ��ϰ� ���� ��ų�� ���� ��ų�� �����Ѵ�. 
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
