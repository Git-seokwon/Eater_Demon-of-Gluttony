using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunningState : PlayerCCState
{
    public override string Description => "����";

    private PlayerController playerContorller;

    public override void Enter()
    {
        Entity.GetComponent<PlayerMovement>().Stop();

        // CC�⸦ ������ ��� Skill �ߵ��� ��� 
        Entity.SkillSystem.CancelAllActiveSkill();

        playerContorller = Entity.GetComponent<PlayerController>();
        if (playerContorller)
            playerContorller.enabled = false;
    }

    public override void Exit()
    {
        if (playerContorller)
            playerContorller.enabled = true;
    }
}
