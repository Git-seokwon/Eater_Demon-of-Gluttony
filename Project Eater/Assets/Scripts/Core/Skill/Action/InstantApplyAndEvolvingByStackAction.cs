using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InstantApplyAndEvolvingByStackAction : SkillAction
{
    [SerializeField]
    private int evolvingStackCount;
    [SerializeField]
    private Skill evolvedSkill;

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

            // ���� ��ų ���� �������� 
            var owner = skill.Owner as PlayerEntity;
            int skillKeyNumber = skill.skillKeyNumber;
            int skillLevel = skill.Level;

            // ��ȭ �� ��ų ���� �� ���� 
            skill.Owner.SkillSystem.Disarm(skill, skillKeyNumber);
            skill.Owner.SkillSystem.Unregister(skill);

            // ��ȭ ��ų ȹ�� �� ���� 
            var evolveSkill = owner.SkillSystem.Register(evolvedSkill, skillLevel);
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
