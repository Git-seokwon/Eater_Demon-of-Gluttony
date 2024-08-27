using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InPrecedingActionState : SkillState
{
    // PrecedingAction이 끝났는지 여부 
    // → 해당 Property가 true라면 InActionState로 넘어가게 된다. 
    public bool IsPrecedingActionEnded { get; private set; }

    public override void Enter()
    {
        // Cast나 ChargeState를 안 거치고 바로 InPrecedingActionState로 넘어왔을 수도 있으니 
        // Activate 상태가 아니라면 Activate 해준다. 
        if (!Entity.IsActivated)
            Entity.Activate();

        // Owner로 ToInSkillPrecedingActionState Command를 보냄
        TrySendCommandToOwner(Entity, EntityStateCommand.ToInSkillPrecedingActionState, Entity.PrecedingActionAnimationParameter);

        Entity.StartPrecedingAction();
    }

    public override void Update()
    {
        // PrecedingAction.Run 함수가 true를 return 하면 PrecedingAction이 완료했다는 의미로 
        // 해당 값이 IsPrecedingActionEnded로 넘어가고 다음 State로 넘어간다. 
        IsPrecedingActionEnded = Entity.RunPrecedingAction();
    }

    public override void Exit()
    {
        IsPrecedingActionEnded = false;

        Entity.ReleasePrecedingAction();
    }
}
