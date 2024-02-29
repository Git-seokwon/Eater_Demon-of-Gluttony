using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    // 기능 구현 전
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, string animParameterName) : base(player, stateMachine, animParameterName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        // 상태 진입 시, 움직임 멈추기
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();

        // 유저 input이 감지가 되면 move로 상태전환
    }
}
