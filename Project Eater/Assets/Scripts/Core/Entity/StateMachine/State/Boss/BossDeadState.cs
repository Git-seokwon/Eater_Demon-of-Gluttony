using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeadState : State<BossEntity>
{
    private BossMovement bossMovement;

    protected override void Setup() => bossMovement = Entity.BossMovement;

    public override void Enter()
    {
        if (bossMovement != null)
        {
            Entity.rigidbody.velocity = Vector2.zero;
            bossMovement.enabled = false;
        }
    }

    public override void Exit()
    {
        if (bossMovement)
            bossMovement.enabled = true;
    }
}
