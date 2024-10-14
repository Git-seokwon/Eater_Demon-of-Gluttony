using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInSkillActionState : EnemySkillState
{
    // ���� State�� ������ ���� ���� ���� 
    // �� �ش� ���� true�� �Ǹ� EntityStateMachine���� Transition�� ���� �ٸ� State�� ��ȯ 
    public bool IsStateEnded { get; private set; }

    public override void Update()
    {
        // InSkillActionFinishOption�� FinishWhenAnimationEnded �̶��, ���� Entity�� ���� ���� Animation�� 
        // ���� ���� AnimatorParameter�� false�� �Ǹ� State�� ����(= IsStateEnded = true)
        if (RunningSkill.InSkillActionFinishOption == InSkillActionFinishOption.FinishWhenAnimationEnded)
            IsStateEnded = !Entity.Animator.GetBool(AnimatorParameterHash);
    }

    public override bool OnReceiveMessage(int message, object data)
    {
        // EntityStateMessage Type�� UsingSkill�� �ƴϸ� return 
        // UsingSkill �̶��, base.OnReceiveMessage(message, data)���� Animation�� ����ϰ� 
        // �Ʒ� �ڵ带 �����Ѵ�. 
        if (!base.OnReceiveMessage(message, data))
            return false;

        // Animation ����� �Բ� ������ Skill�� �ƴ϶��, �� Animation�� ���� �� ������ Skill �̶��
        // Ex) ���̾� ���� 3ȸ ��ô�ϴ� Skill
        if (RunningSkill.InSkillActionFinishOption != InSkillActionFinishOption.FinishWhenAnimationEnded)
            // �� onApplied Event : Skill�� �ߵ��� ������ ȣ��Ǵ� Event
            // Ex) ���̾� ���� ��ô�� ������ onApplied Event�� ����
            RunningSkill.onApplied += OnSkillApplied;

        return true;
    }

    public override void Exit()
    {
        IsStateEnded = false;
        RunningSkill.onApplied -= OnSkillApplied;

        base.Exit();
    }

    // 1) skill             : Event�� ȣ���� Skill
    // 2) currentApplyCount : Skill�� ���� ApplyCount
    private void OnSkillApplied(Skill skill, int currentApplyCount)
    {
        switch (skill.InSkillActionFinishOption)
        {
            // Skill�� �� ���̶� ����Ǿ��ٸ� State�� ���� 
            case InSkillActionFinishOption.FinishOnceApplied:
                IsStateEnded = true;
                break;

            // Skill�� ��� ����Ǿ��ٸ� State�� ���� 
            case InSkillActionFinishOption.FinishWhenFullyApplied:
                // �� IsFinished : Skill�� ��� ������ ���´� �� ���� 
                IsStateEnded = skill.IsFinished;
                break;
        }
    }
}
