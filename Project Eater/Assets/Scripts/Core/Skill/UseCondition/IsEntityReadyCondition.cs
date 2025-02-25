using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Entity�� Skill�� ����� �� �ִ� �������� Ȯ���ϴ� Skill Condition
[System.Serializable]
public class IsEntityReadyCondition : SkillCondition
{
    public override bool IsPass(Skill skill)
    {
        // skill�� Owner�� skillsystem�� �������� 
        var entity = skill.Owner;
        var skillSystem = entity.SkillSystem;

        // skillsystem���� ���� ���� ���� Skill�� �ִٸ� �ٸ� skill�� ������� ���ϰ� ���� ���̱� ������ skillsystem���� ���� ���� ����
        // skill�� �ִ��� Ȯ�� 
        // �� ���� 
        // �� ���� ���� Skill�� �߿� Toggle Ÿ���� �ƴϰ�, Passive Ÿ���� �ƴϰ� InActionState�ε� Input Type�� �ƴ� ��ų�� �ִ��� Ȯ���Ѵ�. 
        // �� ���� ���� Skill�� Toggle Ÿ���̰ų� Passive Ÿ���̰ų� ���� InActionState�ε� Input Type�̸� ���� ���� Skill�� ġ�� �ʰڴ�. 
        //    �̷� ��ų�鸸 ���� ���� ���� �ٸ� Skill�� ����� �� �ִ�. 
        // �� ���ٽ� 
        // Any(x => {...}); : ����Ʈ�� �� �׸� x�� ���ؼ� ���� { }�� �����Ѵ�. 
        // �� RunningSkills�� ��ϵ� �� ��ų ��ü�� �� Ư�� ������ �����ϴ� ��Ұ� �ִ��� Ȯ���Ѵ�. 
        bool isRunningSkillExist = skillSystem.RunningSkills.Any(x =>
        {
            return !x.IsToggleType && !x.IsPassive &&
            !(x.IsInState<InActionState>() && x.ExecutionType == SkillExecutionType.Input);
        });

        if (entity.IsPlayer)
        {
            return (entity as PlayerEntity).IsInState<PlayerDefaultState>() && !isRunningSkillExist;
        }
        else
        {
            if (entity is EnemyEntity enemy)
            {
                return enemy.IsInState<EnemyDefaultState>() && !isRunningSkillExist;
            }
            else if (entity is BossEntity boss)
            {
                return boss.IsInState<BossDefaultState>() && !isRunningSkillExist;
            }
            else if (entity is TutorialEnemyEntity tutorialEnemy)
            {
                return tutorialEnemy.IsInState<TutorialEnemyDefaultState>() && !isRunningSkillExist;
            }
        }

        return false;
    }

    public override object Clone() 
        => new IsEntityReadyCondition();
}
