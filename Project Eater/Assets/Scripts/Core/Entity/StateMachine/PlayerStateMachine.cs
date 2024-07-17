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

        // Dash State
        // 1) DashState �� DefaultState / ���� : Dash�� ���� ���� �ƴ� �� ����
        MakeTransition<DashState, PlayerDefaultState>(state => !Owner.Movement.IsDashing);

        // Dead State
        // 1) DeadState �� DefaultState / ���� : IsDead�� false�� �� ���� 
        MakeTransition<PlayerDeadState, PlayerDefaultState>(state => !Owner.IsDead);

        // �� Any Transition : StateMachine���� ToState Command�� �Ѿ���� ��� ToState�� ���� 
        MakeAnyTransition<PlayerDefaultState>(EntityStateCommand.ToDefaultState);
        MakeAnyTransition<PlayerDeadState>(state => Owner.IsDead);
    }
}
