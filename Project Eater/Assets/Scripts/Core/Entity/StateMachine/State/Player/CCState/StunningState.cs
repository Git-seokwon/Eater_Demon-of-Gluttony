using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunningState : PlayerCCState
{
    public override string Description => "����";

    public override void Enter()
    {
        Entity.GetComponent<PlayerMovement>().Stop();

        // CC�⸦ ������ ��� Skill �ߵ��� ��� 
        Entity.SkillSystem.CancelAllActiveSkill();

        var playerContorller = Entity.GetComponent<PlayerController>();
        if (playerContorller)
            playerContorller.enabled = false;
    }

    public override void Exit()
    {
        var playerContorller = Entity.GetComponent<PlayerController>();
        if (playerContorller)
            playerContorller.enabled = true;
    }
}
