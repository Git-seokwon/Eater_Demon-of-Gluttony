using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : State<PlayerEntity>
{
    private PlayerController controller;

    protected override void Setup()
    {
        controller = Entity.GetComponent<PlayerController>();
    }

    public override void Enter()
    {
        // playerController를 꺼서 대쉬 도중 방향 전환이 안되게 한다. 
        if (controller)
            controller.enabled = false;
    }

    public override void Exit()
    {
        if (controller)
            controller.enabled = true;
    }
}
