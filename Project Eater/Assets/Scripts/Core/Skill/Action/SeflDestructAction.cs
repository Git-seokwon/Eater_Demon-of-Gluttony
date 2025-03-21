using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SeflDestructAction : SkillAction
{
    public override void Apply(Skill skill)
    {
        // Skill�� ���� ȿ���� Targets���� ����
        foreach (var target in skill.Targets)
        {
            target.SkillSystem.Apply(skill);
        }
    }

    public override object Clone() => new SeflDestructAction();
}
