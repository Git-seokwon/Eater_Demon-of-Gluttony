using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingState : SkillState
{
    public override void Enter()
    {
        Entity.Activate();

        Entity.StartCustomActions(SkillCustomActoinType.Cast);

        // Skill의 Owner에게 Casting 상태로 전이하라는 ToCastingSkillState 명령과 함께 CastAnimationParameter를 보낸다. 
        // 1) CastAnimationParameter == Bool
        // Owner가 Casting 상태로 전이되고 전달받은 CastAnimationParameter를 통해 Animation을 실행 
        // 2) CastAnimationParameter == Trigger 
        // ToDefaultState 명령 전달, Command를 받았는지 안 받았는지는 중요하지 않음 ( Bool일 때와 똑같이 Skill관련 데이터들을 보냄)
        // → Command를 받았다면 Entity DefaultState에서 Message를 받아서 처리가 이루어짐 
        TrySendCommandToOwner(Entity, EntityStateCommand.ToCastingSkillState, Entity.CastAnimatorParameter);
    }

    public override void Update()
    {
        // Update를 계속하다 특정 조건을 만족하면 전이가 발생 → Exit 함수 실행 
        Entity.CurrentCastTime += Time.deltaTime;
        Entity.RunCustomActions(SkillCustomActoinType.Cast);
    }

    public override void Exit()
        => Entity.ReleaseCustomActions(SkillCustomActoinType.Cast);
}
