using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoStateMachine<PlayerEntity>
{
    protected override void AddStates()
    {
        AddState<PlayerDefaultState>();
        AddState<DashState>();
        AddState<PlayerDeadState>();
    }

    protected override void MakeTransitions()
    {
        // Default State
        // 1) DefaultState → RollingState
        // ※ ?? : 왼쪽이 null일 경우 오른쪽을 반환 / 조건 : Entity의 Movement가 있고(null이 아니고), IsRolling이 true라면 전이
        MakeTransition<PlayerDefaultState, DashState>(state => Owner.Movement?.IsDashing ?? false);
        MakeTransition<PlayerDefaultState, DashState>(state => Owner.Movement?.IsDashingUp ?? false);
        MakeTransition<PlayerDefaultState, DashState>(state => Owner.Movement?.IsDashingDown ?? false);

        // Dash State
        // 1) DashState → DefaultState / 조건 : Dash가 실행 중이 아닐 때 전이
        MakeTransition<DashState, PlayerDefaultState>(state => !Owner.Movement.IsDashing);
        MakeTransition<DashState, PlayerDefaultState>(state => !Owner.Movement.IsDashingUp);
        MakeTransition<DashState, PlayerDefaultState>(state => !Owner.Movement.IsDashingDown);

        // Dead State
        // 1) DeadState → DefaultState / 조건 : IsDead가 false일 때 전이 
        MakeTransition<PlayerDeadState, PlayerDefaultState>(state => !Owner.IsDead);

        // ※ Any Transition : StateMachine으로 ToState Command가 넘어오면 즉시 ToState로 전이 
        MakeAnyTransition<PlayerDefaultState>(EntityStateCommand.ToDefaultState);
        MakeAnyTransition<PlayerDeadState>(state => Owner.IsDead);
    }
}
