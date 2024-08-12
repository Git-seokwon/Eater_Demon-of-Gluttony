using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CC기를 맞으면 모든 행동과 조작이 멈췄다가 CC가 끝나면 다시 조작이 가능해진다. 
public abstract class PlayerCCState : State<PlayerEntity>
{
    // 현재 상태의 설명 or 이름 
    public abstract string Description { get; }
    // 현재 상태에서 실행할 Animation의 Parameter
    protected abstract int AnimationHash { get; }

    public override void Enter()
    {
        Entity.Animator?.SetBool(AnimationHash, true);

        Entity.GetComponent<PlayerMovement>().Stop();

        // CC기를 맞으면 모든 Skill 발동을 취소 
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
