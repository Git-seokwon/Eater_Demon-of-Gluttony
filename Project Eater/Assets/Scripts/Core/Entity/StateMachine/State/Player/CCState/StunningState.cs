using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunningState : PlayerCCState
{
    private static readonly int kAnimationHash = Animator.StringToHash("isStuning");

    public override string Description => "����";
    protected override int AnimationHash => kAnimationHash;

    public override void Enter()
    {
        Entity.Animator?.SetBool(AnimationHash, true);

        Entity.GetComponent<PlayerMovement>().Stop();

        // CC�⸦ ������ ��� Skill �ߵ��� ��� 
        Entity.SkillSystem.CancleAll();

        var playerContorller = Entity.GetComponent<PlayerController>();
        if (playerContorller)
            playerContorller.enabled = false;
    }

    public override void Exit()
    {
        Entity.Animator?.SetBool(AnimationHash, false);

        var playerContorller = Entity.GetComponent<PlayerController>();
        if (playerContorller)
            playerContorller.enabled = true;
    }
}
