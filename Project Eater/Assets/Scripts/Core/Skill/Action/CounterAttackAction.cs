using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CounterAttackAction : SkillAction
{
    public override void Start(Skill skill)
    {
        if (skill.Owner is BossEntity boss)
        {
            boss.IsFlipped = false;
            boss.IsCounter = true;
            boss.Animator.speed = 0;
            boss.SetCounterAttackEvent();
            boss.StartCoroutine(boss.CancelCounterAttack(boss, skill));
        }
    }

    public override void Apply(Skill skill)
    {
        foreach (var target in skill.Targets)
        {
            target.SkillSystem.Apply(skill);
        }
    }

    public override void Release(Skill skill)
    {
        if (skill.Owner is BossEntity boss)
        {
            boss.IsFlipped = true;
            boss.IsCounter = false;
            boss.Animator.speed = 1;
            boss.UnSetCounterAttackEvent();
            boss.IsCounterApply = false;
        }
    }

    public override object Clone() => new CounterAttackAction();
}
