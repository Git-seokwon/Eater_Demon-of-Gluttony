using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunningState : EnemyCCState
{
    private static readonly int kAnimationHash = Animator.StringToHash("IsStunning");

    public override string Description => "����";
    protected override int AnimationHash => kAnimationHash;


    public override void Enter()
    {
        Entity.Animator?.SetBool(AnimationHash, true);

        Entity.GetComponent<EnemyMovement>().Stop();

        // CC�⸦ ������ ��� Aactive Skill �ߵ��� ��� 
        Entity.SkillSystem.CancelAllActiveSkill();

        Entity.GetComponent<EnemyMovement>().enabled = false;
    }

    public override void Exit()
    {
        Entity.Animator?.SetBool(AnimationHash, false);

        Entity.GetComponent<EnemyMovement>().enabled = true;
    }
}
