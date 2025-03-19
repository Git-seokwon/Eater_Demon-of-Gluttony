using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine : MonoStateMachine<BossEntity>
{
    protected override void AddStates()
    {
        AddState<BossDefaultState>();
        AddState<BossDeadState>();

        // Skill�� Casting ���� ��, Enemy�� ���� 
        AddState<BossCastingSkillState>(); 
        // Skill�� Charging ���� �� Enemy�� ����
        AddState<BossChargingSkillState>(); 
        // Skill�� Preceding Action ���� �� Enemy�� ���� 
        // �� PrecedingAction : Skill�� �ߵ��ϱ� ���� �ϴ� ���� Action 
        // Ex) ����� �����̵� : �����̵� �� ���� �����̿� �ִ� ������ ȭ���� �߻�
        //     ���⼭, �����̵��� PrecedingAction�̰�, ���� ����� ���� �����ϴ� ���� SkillAction �̴�.
        AddState<BossInSkillPrecedingActionState>();
        // Skill�� �ߵ� ���� �� Enemy�� ���� 
        AddState<BossInSkillActionState>();
        // Enemy�� Stun CC�⸦ �¾��� ���� ���� 
        AddState<BossStunningState>();
    }

    protected override void MakeTransitions()
    {
        #region Default State
        // 1) DefaultState �� Skill���� State / ���� : �ش��ϴ� SkillState�� �����϶�� ����� ������ �ش� ��ɿ� �´� SkillState�� ���� 
        MakeTransition<BossDefaultState, BossCastingSkillState>(EntityStateCommand.ToCastingSkillState);
        MakeTransition<BossDefaultState, BossChargingSkillState>(EntityStateCommand.ToChargingSkillState);
        MakeTransition<BossDefaultState, BossInSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<BossDefaultState, BossInSkillActionState>(EntityStateCommand.ToInSkillActionState);

        // Skill State
        // Casting State
        // 1) Casting�� ������ PrecedingAction State�� ���̵Ǵ� ��� / ���� : Message
        MakeTransition<BossCastingSkillState, BossInSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        // 2) Casting�� ������ PrecedingAction�� ��� �ٷ� SkillAction State�� �Ѿ�� ��� / ���� : Message
        MakeTransition<BossCastingSkillState, BossInSkillActionState>(EntityStateCommand.ToInSkillActionState);
        // 3) Casting�� �߰��� ��� �ٽ� DefaultState�� �Ѿ�� ��� / ���� : ���� ���� ���� Skill�� Casting ���°� �ƴ϶�� ���� ��ȯ
        // �� ������ : CastingSkillState(Entity�� State), CastingState(Skill�� State)�� �ٸ� �� 
        // �� Skill�� Casting ���°� �ƴϴϱ� Skill�� ����ϰ� �ִ� ��� Casting ���¿��� Default ���·� ���ư��� 
        // �� transitionCondition : state => IsSkillInState<CastingState>(state)
        MakeTransition<BossCastingSkillState, BossDefaultState>(state => !IsSkillInState<CastingState>(state));

        // Charging State
        // Casting State�� ����� ���� ����. 
        MakeTransition<BossChargingSkillState, BossInSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<BossChargingSkillState, BossInSkillActionState>(EntityStateCommand.ToInSkillActionState);
        MakeTransition<BossChargingSkillState, BossDefaultState>(state => !IsSkillInState<ChargingState>(state));

        // PrecedingAction State
        // PrecedingAction�� ������ ��, Message�� �Ѿ���� InSkillActionState�� ����, �ƴϸ� PlayerDefaultState�� ���� 
        MakeTransition<BossInSkillPrecedingActionState, BossInSkillActionState>(EntityStateCommand.ToInSkillActionState);
        MakeTransition<BossInSkillPrecedingActionState, BossDefaultState>(state => !IsSkillInState<InPrecedingActionState>(state));

        // Action State
        // IsStateEnded�� true��� ��, Action�� �����ٸ� PlayerDefaultState�� ���� 
        MakeTransition<BossInSkillActionState, BossDefaultState>(state => (state as BossInSkillActionState).IsStateEnded);

        // Dead State
        // 1) DeadState �� DefaultState / ���� : IsDead�� false�� �� ���� 
        MakeTransition<BossDeadState, BossDefaultState>(state => !Owner.IsDead);
        #endregion

        #region Any Transition
        // �� Any Transition : StateMachine���� ToState Command�� �Ѿ���� ��� ToState�� ���� 
        MakeAnyTransition<BossDefaultState>(EntityStateCommand.ToDefaultState);

        // Entity�� �׾����� ��� DeadState�� ���� (Command�� �ƴ� transitionCondition�� �ִ� ����)
        // �� canTransitionToSelf�� ����Ʈ �Ű����� false �̱� ������ Dead���� �ٽ� Dead�� �Ѿ ���� ����.
        MakeAnyTransition<BossDeadState>(state => Owner.IsDead && !Owner.IsSelfDestructive);

        // CC State
        // �� Stuning State
        MakeAnyTransition<BossStunningState>(EntityStateCommand.ToStunningState);
        #endregion
    }

    // ���� ���� ���� Skill�� ���°� T�� �´ٸ� True�� �ƴ϶�� False�� ��ȯ
    private bool IsSkillInState<T>(State<BossEntity> state) where T : State<Skill>
        => (state as BossSkillState).RunningSkill.IsInState<T>();
}
