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
        // 1) DefaultState �� RollingState
        // �� ?? : ������ null�� ��� �������� ��ȯ / ���� : Entity�� Movement�� �ְ�(null�� �ƴϰ�), IsRolling�� true��� ����
        MakeTransition<PlayerDefaultState, DashState>(state => Owner.Movement?.IsDashing ?? false);
        MakeTransition<PlayerDefaultState, DashState>(state => Owner.Movement?.IsDashingUp ?? false);
        MakeTransition<PlayerDefaultState, DashState>(state => Owner.Movement?.IsDashingDown ?? false);

        // Dash State
        // 1) DashState �� DefaultState / ���� : Dash�� ���� ���� �ƴ� �� ����
        MakeTransition<DashState, PlayerDefaultState>(state => !Owner.Movement.IsDashing);
        MakeTransition<DashState, PlayerDefaultState>(state => !Owner.Movement.IsDashingUp);
        MakeTransition<DashState, PlayerDefaultState>(state => !Owner.Movement.IsDashingDown);

        // Dead State
        // 1) DeadState �� DefaultState / ���� : IsDead�� false�� �� ���� 
        MakeTransition<PlayerDeadState, PlayerDefaultState>(state => !Owner.IsDead);

        // �� Any Transition : StateMachine���� ToState Command�� �Ѿ���� ��� ToState�� ���� 
        MakeAnyTransition<PlayerDefaultState>(EntityStateCommand.ToDefaultState);
        MakeAnyTransition<PlayerDeadState>(state => Owner.IsDead);
    }
}
