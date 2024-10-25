using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InPrecedingActionState : SkillState
{
    // PrecedingAction�� �������� ���� 
    // �� �ش� Property�� true��� InActionState�� �Ѿ�� �ȴ�. 
    public bool IsPrecedingActionEnded { get; private set; }

    public override void Enter()
    {
        base.Enter();

        // Cast�� ChargeState�� �� ��ġ�� �ٷ� InPrecedingActionState�� �Ѿ���� ���� ������ 
        // Activate ���°� �ƴ϶�� Activate ���ش�. 
        if (!Entity.IsActivated)
            Entity.Activate();

        // Owner�� ToInSkillPrecedingActionState Command�� ����
        TrySendCommandToOwner(Entity, EntityStateCommand.ToInSkillPrecedingActionState, Entity.PrecedingActionAnimationParameter);

        Entity.StartPrecedingAction();
    }

    public override void Update()
    {
        // PrecedingAction.Run �Լ��� true�� return �ϸ� PrecedingAction�� �Ϸ��ߴٴ� �ǹ̷� 
        // �ش� ���� IsPrecedingActionEnded�� �Ѿ�� ���� State�� �Ѿ��. 
        IsPrecedingActionEnded = Entity.RunPrecedingAction();
    }

    public override void FixedUpdate()
    {
        if (!IsPrecedingActionEnded)
            Entity.FixedRunPrecedingAction();
    }

    public override void Exit()
    {
        base.Exit();

        IsPrecedingActionEnded = false;

        Entity.ReleasePrecedingAction();
    }
}
