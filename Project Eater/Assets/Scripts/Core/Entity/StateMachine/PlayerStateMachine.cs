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

        // Skill�� Casting ���� ��, Player�� ���� 
        AddState<CastingSkillState>();
        // Skill�� Charging ���� �� Player�� ����
        AddState<ChargingSkillState>();
        // Skill�� Preceding Action ���� �� Player�� ���� 
        // �� PrecedingAction : Skill�� �ߵ��ϱ� ���� �ϴ� ���� Action 
        // Ex) ����� �����̵� : �����̵� �� ���� �����̿� �ִ� ������ ȭ���� �߻�
        //     ���⼭, �����̵��� PrecedingAction�̰�, ���� ����� ���� �����ϴ� ���� SkillAction �̴�.
        AddState<InSkillPrecedingActionState>();
        // Skill�� �ߵ� ���� �� Player�� ���� 
        AddState<InSkillActionState>();
        // Player�� Stun CC�⸦ �¾��� ���� ���� 
        AddState<StunningState>();
        // Player�� ���� ��ų�� �ߵ����� ���� ���� 
        AddState<PlayerSuperArmorState>();
    }

    protected override void MakeTransitions()
    {
        #region Default State
        // 1) DefaultState �� RollingState / ���� : Entity�� Movement�� �ְ�(null�� �ƴϰ�), IsRolling�� true��� ����
        // �� ?? : Movement�� null�� ��� false�� ��ȯ
        MakeTransition<PlayerDefaultState, DashState>(state => Owner.PlayerMovement?.IsDashing ?? false);
        // 2) DefaultState �� Skill���� State / ���� : �ش��ϴ� SkillState�� �����϶�� ����� ������ �ش� ��ɿ� �´� SkillState�� ���� 
        MakeTransition<PlayerDefaultState, CastingSkillState>(EntityStateCommand.ToCastingSkillState);
        MakeTransition<PlayerDefaultState, ChargingSkillState>(EntityStateCommand.ToChargingSkillState);
        MakeTransition<PlayerDefaultState, InSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<PlayerDefaultState, InSkillActionState>(EntityStateCommand.ToInSkillActionState);

        // Dash State
        // 1) DashState �� DefaultState / ���� : Dash�� ���� ���� �ƴ� �� ����
        MakeTransition<DashState, PlayerDefaultState>(state => !Owner.PlayerMovement.IsDashing);

        // Skill State
            // Casting State
        // 1) Casting�� ������ PrecedingAction State�� ���̵Ǵ� ��� / ���� : Message
        MakeTransition<CastingSkillState, InSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        // 2) Casting�� ������ PrecedingAction�� ��� �ٷ� SkillAction State�� �Ѿ�� ��� / ���� : Message
        MakeTransition<CastingSkillState, InSkillActionState>(EntityStateCommand.ToInSkillActionState);
        // 3) Casting�� �߰��� ��� �ٽ� DefaultState�� �Ѿ�� ��� / ���� : ���� ���� ���� Skill�� Casting ���°� �ƴ϶�� ���� ��ȯ
        // �� ������ : CastingSkillState(Entity�� State), CastingState(Skill�� State)�� �ٸ� �� 
        // �� Skill�� Casting ���°� �ƴϴϱ� Skill�� ����ϰ� �ִ� ��� Casting ���¿��� Default ���·� ���ư��� 
        // �� transitionCondition : state => IsSkillInState<CastingState>(state)
        MakeTransition<CastingSkillState, PlayerDefaultState>(state => !IsSkillInState<CastingState>(state));

            // Charging State
        // Casting State�� ����� ���� ����. 
        MakeTransition<ChargingSkillState, InSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<ChargingSkillState, InSkillActionState>(EntityStateCommand.ToInSkillActionState);
        MakeTransition<ChargingSkillState, PlayerDefaultState>(state => !IsSkillInState<ChargingState>(state));

            // PrecedingAction State
        // PrecedingAction�� ������ ��, Message�� �Ѿ���� InSkillActionState�� ����, �ƴϸ� PlayerDefaultState�� ���� 
        MakeTransition<InSkillPrecedingActionState, InSkillActionState>(EntityStateCommand.ToInSkillActionState);
        MakeTransition<InSkillPrecedingActionState, PlayerDefaultState>(state => !IsSkillInState<InPrecedingActionState>(state));

            // Action State
        // IsStateEnded�� true��� ��, Action�� �����ٸ� PlayerDefaultState�� ���� 
        MakeTransition<InSkillActionState, PlayerDefaultState>(state => (state as InSkillActionState).IsStateEnded);

        // Dead State
        // 1) DeadState �� DefaultState / ���� : IsDead�� false�� �� ���� 
        MakeTransition<PlayerDeadState, PlayerDefaultState>(state => !Owner.IsDead);

        // SuperArmor State
        MakeTransition<PlayerSuperArmorState, PlayerDefaultState>(state => (state as PlayerSuperArmorState).isTimeOver);
        #endregion

        #region Any Transition
        // �� Any Transition : StateMachine���� ToState Command�� �Ѿ���� ��� ToState�� ���� 
        MakeAnyTransition<PlayerDefaultState>(EntityStateCommand.ToDefaultState);

        // Entity�� �׾����� ��� DeadState�� ���� (Command�� �ƴ� transitionCondition�� �ִ� ����)
        // �� canTransitionToSelf�� ����Ʈ �Ű����� false �̱� ������ Dead���� �ٽ� Dead�� �Ѿ ���� ����.
        MakeAnyTransition<PlayerDeadState>(state => Owner.IsDead);

        // CC State
            // Stuning State
        MakeAnyTransition<StunningState>(EntityStateCommand.ToStunningState);

        // ���� ��ų�� ���� SuperArmor ���·� ���� 
        MakeAnyTransition<PlayerSuperArmorState>(EntityStateCommand.ToSuperArmorState);
        #endregion
    }

    // ���� ���� ���� Skill�� ���°� T�� �´ٸ� True�� �ƴ϶�� False�� ��ȯ
    private bool IsSkillInState<T>(State<PlayerEntity> state) where T : State<Skill>
        => (state as PlayerSkillState).RunningSkill.IsInState<T>();
}
