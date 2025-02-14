using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunningState : PlayerCCState
{
    private static readonly int kAnimationHash = Animator.StringToHash("IsStunning");

    public override string Description => "기절";
    protected override int AnimationHash => kAnimationHash;

    private PlayerController playerContorller;

    public override void Enter()
    {
        Entity.Animator?.SetBool(AnimationHash, true);

        Entity.GetComponent<PlayerMovement>().Stop();

        // CC기를 맞으면 모든 Skill 발동을 취소 
        Entity.SkillSystem.CancelAllActiveSkill();

        playerContorller = Entity.GetComponent<PlayerController>();
        if (playerContorller)
            playerContorller.enabled = false;
    }

    public override void Exit()
    {
        Entity.Animator?.SetBool(AnimationHash, false);

        if (playerContorller)
            playerContorller.enabled = true;
    }
}
