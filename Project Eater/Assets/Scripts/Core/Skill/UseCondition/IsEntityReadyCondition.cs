using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Entity가 Skill을 사용할 수 있는 상태인지 확인하는 Skill Condition
[System.Serializable]
public class IsEntityReadyCondition : SkillCondition
{
    public override bool IsPass(Skill skill)
    {
        // skill의 Owner와 skillsystem을 가져오기 
        var entity = skill.Owner;
        var skillSystem = entity.SkillSystem;

        // skillsystem에서 현재 실행 중인 Skill이 있다면 다른 skill을 사용하지 못하게 해줄 것이기 때문에 skillsystem에서 현재 실행 중인
        // skill이 있는지 확인 
        // ※ 조건 
        // → 실행 중인 Skill들 중에 Toggle 타입이 아니고, Passive 타입이 아니고 InActionState인데 Input Type이 아닌 스킬이 있는지 확인한다. 
        // → 실행 중인 Skill이 Toggle 타입이거나 Passive 타입이거나 현재 InActionState인데 Input Type이면 실행 중인 Skill로 치지 않겠다. 
        //    이런 스킬들만 실행 중일 때는 다른 Skill을 사용할 수 있다. 
        // ※ 람다식 
        // Any(x => {...}); : 리스트의 각 항목 x에 대해서 조건 { }을 정의한다. 
        // → RunningSkills에 등록된 각 스킬 객체들 중 특정 조건을 만족하는 요소가 있는지 확인한다. 
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
