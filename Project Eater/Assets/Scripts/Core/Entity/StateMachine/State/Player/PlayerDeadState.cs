using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : State<PlayerEntity>
{
    private PlayerController controller;
    private PlayerMovement playerMovement;

    protected override void Setup()
    {
        controller = Entity.GetComponent<PlayerController>();
        playerMovement = Entity.GetComponent<PlayerMovement>();
    }

    public override void Enter()
    {
        if (controller)
            controller.enabled = false;
        if (playerMovement)
            playerMovement.enabled = false;
    }

    // Entity�� �ٽ� ��Ƴ�
    public override void Exit()
    {
        if (controller)
            controller.enabled = true;
        if (playerMovement)
            playerMovement.enabled = true;
    }
}
