using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        // Skill이 가진 효과를 Targets에게 적용
        foreach (var target in skill.Targets)
        {
            target.SkillSystem.Apply(skill);
        }
    }

    // 스택을 소모하고 현재 스킬을 각성 스킬로 변경한다. 
    public override void Release(Skill skill)
    {
        if ((skill.Owner as PlayerEntity).DeathStack >= evolvingStackCount)
        {
            (skill.Owner as PlayerEntity).DeathStack -= evolvingStackCount;

            // 이전 스킬 정보 가져오기 
            var owner = skill.Owner as PlayerEntity;
            int skillKeyNumber = skill.skillKeyNumber;

            // 스킬 업그레이드 리스트에서 제거하기 
            skill.Owner.SkillSystem.RemoveUpgradableSkills(skill);

            // 진화 전 스킬 해제 및 삭제 
            skill.Owner.SkillSystem.Disarm(skill, skillKeyNumber);
            skill.Owner.SkillSystem.Unregister(skill);

            // 진화 스킬 획득 및 장착 
            var evolveSkill = owner.SkillSystem.Register(evolvedSkill);
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
