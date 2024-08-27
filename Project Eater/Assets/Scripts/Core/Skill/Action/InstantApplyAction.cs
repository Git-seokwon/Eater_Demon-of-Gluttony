using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ٸ� ���� ���� Skill�� TargetSearcher�� �˻��� Target�鿡�� ��� Skill�� ȿ���� ���� ��Ű�� Module
[System.Serializable]
public class InstantApplyAction : SkillAction
{
    public override void Apply(Skill skill)
    {
        // Skill�� ���� ȿ���� Targets���� ����
        foreach (var target in skill.Targets)
            target.SkillSystem.Apply(skill);
    }

    public override object Clone() => new InstantApplyAction();
}
