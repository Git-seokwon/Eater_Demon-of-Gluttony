using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CC�⸦ ������ ��� �ൿ�� ������ ����ٰ� CC�� ������ �ٽ� ������ ����������. 
public abstract class PlayerCCState : State<PlayerEntity>
{
    // ���� ������ ���� or �̸� 
    public abstract string Description { get; }
    // ���� ���¿��� ������ Animation�� Parameter
    protected abstract int AnimationHash { get; }

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
