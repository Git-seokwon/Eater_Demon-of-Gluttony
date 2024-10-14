using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunningState : PlayerCCState
{
    public override string Description => "기절";

    public override void Enter()
    {
        Entity.GetComponent<PlayerMovement>().Stop();

        // CC기를 맞으면 모든 Skill 발동을 취소 
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
