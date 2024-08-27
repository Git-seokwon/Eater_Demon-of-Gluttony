using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingState : SkillState
{
    public override void Enter()
    {
        Entity.Activate();

        Entity.StartCustomActions(SkillCustomActoinType.Cast);

        // Skill�� Owner���� Casting ���·� �����϶�� ToCastingSkillState ��ɰ� �Բ� CastAnimationParameter�� ������. 
        // 1) CastAnimationParameter == Bool
        // Owner�� Casting ���·� ���̵ǰ� ���޹��� CastAnimationParameter�� ���� Animation�� ���� 
        // 2) CastAnimationParameter == Trigger 
        // ToDefaultState ��� ����, Command�� �޾Ҵ��� �� �޾Ҵ����� �߿����� ���� ( Bool�� ���� �Ȱ��� Skill���� �����͵��� ����)
        // �� Command�� �޾Ҵٸ� Entity DefaultState���� Message�� �޾Ƽ� ó���� �̷���� 
        TrySendCommandToOwner(Entity, EntityStateCommand.ToCastingSkillState, Entity.CastAnimatorParameter);
    }

    public override void Update()
    {
        // Update�� ����ϴ� Ư�� ������ �����ϸ� ���̰� �߻� �� Exit �Լ� ���� 
        Entity.CurrentCastTime += Time.deltaTime;
        Entity.RunCustomActions(SkillCustomActoinType.Cast);
    }

    public override void Exit()
        => Entity.ReleaseCustomActions(SkillCustomActoinType.Cast);
}
