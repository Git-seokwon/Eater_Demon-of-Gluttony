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
        // playerController�� ���� �뽬 ���� ���� ��ȯ�� �ȵǰ� �Ѵ�. 
        if (controller)
            controller.enabled = false;
    }

    public override void Exit()
    {
        if (controller)
            controller.enabled = true;
    }
}
