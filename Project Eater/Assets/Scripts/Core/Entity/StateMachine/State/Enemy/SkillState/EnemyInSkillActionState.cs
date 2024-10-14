using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInSkillActionState : EnemySkillState
{
    // 현재 State가 끝났는 지에 대한 여부 
    // → 해당 값이 true가 되면 EntityStateMachine에서 Transition을 통해 다른 State로 전환 
    public bool IsStateEnded { get; private set; }

    public override void Update()
    {
        // InSkillActionFinishOption이 FinishWhenAnimationEnded 이라면, 현재 Entity가 실행 중인 Animation이 
        // 끝난 다음 AnimatorParameter가 false가 되면 State를 종료(= IsStateEnded = true)
        if (RunningSkill.InSkillActionFinishOption == InSkillActionFinishOption.FinishWhenAnimationEnded)
            IsStateEnded = !Entity.Animator.GetBool(AnimatorParameterHash);
    }

    public override bool OnReceiveMessage(int message, object data)
    {
        // EntityStateMessage Type이 UsingSkill이 아니면 return 
        // UsingSkill 이라면, base.OnReceiveMessage(message, data)에서 Animation을 재생하고 
        // 아래 코드를 실행한다. 
        if (!base.OnReceiveMessage(message, data))
            return false;

        // Animation 종료와 함께 끝나는 Skill이 아니라면, 즉 Animation이 여러 번 나오는 Skill 이라면
        // Ex) 파이어 볼을 3회 투척하는 Skill
        if (RunningSkill.InSkillActionFinishOption != InSkillActionFinishOption.FinishWhenAnimationEnded)
            // ※ onApplied Event : Skill이 발동될 때마다 호출되는 Event
            // Ex) 파이어 볼을 투척할 때마다 onApplied Event가 실행
            RunningSkill.onApplied += OnSkillApplied;

        return true;
    }

    public override void Exit()
    {
        IsStateEnded = false;
        RunningSkill.onApplied -= OnSkillApplied;

        base.Exit();
    }

    // 1) skill             : Event를 호출한 Skill
    // 2) currentApplyCount : Skill의 현재 ApplyCount
    private void OnSkillApplied(Skill skill, int currentApplyCount)
    {
        switch (skill.InSkillActionFinishOption)
        {
            // Skill이 한 번이라도 적용되었다면 State를 종료 
            case InSkillActionFinishOption.FinishOnceApplied:
                IsStateEnded = true;
                break;

            // Skill이 모두 적용되었다면 State를 종료 
            case InSkillActionFinishOption.FinishWhenFullyApplied:
                // ※ IsFinished : Skill이 모든 동작을 끝냈는 지 여부 
                IsStateEnded = skill.IsFinished;
                break;
        }
    }
}
