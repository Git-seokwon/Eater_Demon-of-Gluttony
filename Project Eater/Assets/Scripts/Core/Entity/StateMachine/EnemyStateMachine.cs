using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoStateMachine<EnemyEntity>
{
    protected override void AddStates()
    {
        AddState<EnemyDefaultState>();
        AddState<EnemyDeadState>();

        // Skill�� Casting ���� ��, Enemy�� ���� 
        AddState<EnemyCastingSkillState>();
        // Skill�� Charging ���� �� Enemy�� ����
        AddState<EnemyChargingSkillState>();
        // Skill�� Preceding Action ���� �� Enemy�� ���� 
        // �� PrecedingAction : Skill�� �ߵ��ϱ� ���� �ϴ� ���� Action 
        // Ex) ����� �����̵� : �����̵� �� ���� �����̿� �ִ� ������ ȭ���� �߻�
        //     ���⼭, �����̵��� PrecedingAction�̰�, ���� ����� ���� �����ϴ� ���� SkillAction �̴�.
        AddState<EnemyInSkillPrecedingActionState>();
        // Skill�� �ߵ� ���� �� Enemy�� ���� 
        AddState<EnemyInSkillActionState>();
        // Enemy�� Stun CC�⸦ �¾��� ���� ���� 
        AddState<EnemyStunningState>();
    }

    protected override void MakeTransitions()
    {
        #region Default State
        // 1) DefaultState �� Skill���� State / ���� : �ش��ϴ� SkillState�� �����϶�� ����� ������ �ش� ��ɿ� �´� SkillState�� ���� 
        MakeTransition<EnemyDefaultState, EnemyCastingSkillState>(EntityStateCommand.ToCastingSkillState);
        MakeTransition<EnemyDefaultState, EnemyChargingSkillState>(EntityStateCommand.ToChargingSkillState);
        MakeTransition<EnemyDefaultState, EnemyInSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<EnemyDefaultState, EnemyInSkillActionState>(EntityStateCommand.ToInSkillActionState);

        // Skill State
            // Casting State
        // 1) Casting�� ������ PrecedingAction State�� ���̵Ǵ� ��� / ���� : Message
        MakeTransition<EnemyCastingSkillState, EnemyInSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        // 2) Casting�� ������ PrecedingAction�� ��� �ٷ� SkillAction State�� �Ѿ�� ��� / ���� : Message
        MakeTransition<EnemyCastingSkillState, EnemyInSkillActionState>(EntityStateCommand.ToInSkillActionState);
        // 3) Casting�� �߰��� ��� �ٽ� DefaultState�� �Ѿ�� ��� / ���� : ���� ���� ���� Skill�� Casting ���°� �ƴ϶�� ���� ��ȯ
        // �� ������ : CastingSkillState(Entity�� State), CastingState(Skill�� State)�� �ٸ� �� 
        // �� Skill�� Casting ���°� �ƴϴϱ� Skill�� ����ϰ� �ִ� ��� Casting ���¿��� Default ���·� ���ư��� 
        // �� transitionCondition : state => IsSkillInState<CastingState>(state)
        MakeTransition<EnemyCastingSkillState, EnemyDefaultState>(state => !IsSkillInState<CastingState>(state));

            // Charging State
        // Casting State�� ����� ���� ����. 
        MakeTransition<EnemyChargingSkillState, EnemyInSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<EnemyChargingSkillState, EnemyInSkillActionState>(EntityStateCommand.ToInSkillActionState);
        MakeTransition<EnemyChargingSkillState, EnemyDefaultState>(state => !IsSkillInState<ChargingState>(state));

            // PrecedingAction State
        // PrecedingAction�� ������ ��, Message�� �Ѿ���� InSkillActionState�� ����, �ƴϸ� PlayerDefaultState�� ���� 
        MakeTransition<EnemyInSkillPrecedingActionState, EnemyInSkillActionState>(EntityStateCommand.ToInSkillActionState);
        MakeTransition<EnemyInSkillPrecedingActionState, EnemyDefaultState>(state => !IsSkillInState<InPrecedingActionState>(state));

            // Action State
        // IsStateEnded�� true��� ��, Action�� �����ٸ� PlayerDefaultState�� ���� 
        MakeTransition<EnemyInSkillActionState, EnemyDefaultState>(state => (state as EnemyInSkillActionState).IsStateEnded);

        // Dead State
        // 1) DeadState �� DefaultState / ���� : IsDead�� false�� �� ���� 
        MakeTransition<EnemyDefaultState, EnemyDeadState>(state => !Owner.IsDead);
        #endregion

        #region Any Transition
        // �� Any Transition : StateMachine���� ToState Command�� �Ѿ���� ��� ToState�� ���� 
        MakeAnyTransition<EnemyDefaultState>(EntityStateCommand.ToDefaultState);

        // Entity�� �׾����� ��� DeadState�� ���� (Command�� �ƴ� transitionCondition�� �ִ� ����)
        // �� canTransitionToSelf�� ����Ʈ �Ű����� false �̱� ������ Dead���� �ٽ� Dead�� �Ѿ ���� ����.
        MakeAnyTransition<EnemyDeadState>(state => Owner.IsDead);

        // CC State
        // �� Stuning State
        MakeAnyTransition<EnemyStunningState>(EntityStateCommand.ToStunningState);
        #endregion
    }

    // ���� ���� ���� Skill�� ���°� T�� �´ٸ� True�� �ƴ϶�� False�� ��ȯ
    private bool IsSkillInState<T>(State<EnemyEntity> state) where T : State<Skill>
        => (state as EnemySkillState).RunningSkill.IsInState<T>();
}
