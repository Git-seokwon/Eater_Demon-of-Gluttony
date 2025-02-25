using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemyStateMachineEnums : MonoStateMachine<TutorialEnemyEntity>
{
    protected override void AddStates()
    {
        AddState<TutorialEnemyDefaultState>();
        AddState<TutorialEnemyDeadState>();

        // Skill�� Casting ���� ��, Enemy�� ���� 
        AddState<TutorialEnemyCastingSkillState>();
        // Skill�� Charging ���� �� Enemy�� ����
        AddState<TutorialEnemyChargingSkillState>();
        // Skill�� Preceding Action ���� �� Enemy�� ���� 
        // �� PrecedingAction : Skill�� �ߵ��ϱ� ���� �ϴ� ���� Action 
        // Ex) ����� �����̵� : �����̵� �� ���� �����̿� �ִ� ������ ȭ���� �߻�
        //     ���⼭, �����̵��� PrecedingAction�̰�, ���� ����� ���� �����ϴ� ���� SkillAction �̴�.
        AddState<TutorialEnemyInSkillPrecedingActionState>();
        // Skill�� �ߵ� ���� �� Enemy�� ���� 
        AddState<TutorialEnemyInSkillActionState>();
    }

    protected override void MakeTransitions()
    {
        #region Default State
        // 1) DefaultState �� Skill���� State / ���� : �ش��ϴ� SkillState�� �����϶�� ����� ������ �ش� ��ɿ� �´� SkillState�� ���� 
        MakeTransition<TutorialEnemyDefaultState, TutorialEnemyCastingSkillState>(EntityStateCommand.ToCastingSkillState);
        MakeTransition<TutorialEnemyDefaultState, TutorialEnemyChargingSkillState>(EntityStateCommand.ToChargingSkillState);
        MakeTransition<TutorialEnemyDefaultState, TutorialEnemyInSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<TutorialEnemyDefaultState, TutorialEnemyInSkillActionState>(EntityStateCommand.ToInSkillActionState);

        // Skill State
        // Casting State
        // 1) Casting�� ������ PrecedingAction State�� ���̵Ǵ� ��� / ���� : Message
        MakeTransition<TutorialEnemyCastingSkillState, TutorialEnemyInSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        // 2) Casting�� ������ PrecedingAction�� ��� �ٷ� SkillAction State�� �Ѿ�� ��� / ���� : Message
        MakeTransition<TutorialEnemyCastingSkillState, TutorialEnemyInSkillActionState>(EntityStateCommand.ToInSkillActionState);
        // 3) Casting�� �߰��� ��� �ٽ� DefaultState�� �Ѿ�� ��� / ���� : ���� ���� ���� Skill�� Casting ���°� �ƴ϶�� ���� ��ȯ
        // �� ������ : CastingSkillState(Entity�� State), CastingState(Skill�� State)�� �ٸ� �� 
        // �� Skill�� Casting ���°� �ƴϴϱ� Skill�� ����ϰ� �ִ� ��� Casting ���¿��� Default ���·� ���ư��� 
        // �� transitionCondition : state => IsSkillInState<CastingState>(state)
        MakeTransition<TutorialEnemyCastingSkillState, TutorialEnemyDefaultState>(state => !IsSkillInState<CastingState>(state));

        // Charging State
        // Casting State�� ����� ���� ����. 
        MakeTransition<TutorialEnemyChargingSkillState, TutorialEnemyInSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<TutorialEnemyChargingSkillState, TutorialEnemyInSkillActionState>(EntityStateCommand.ToInSkillActionState);
        MakeTransition<TutorialEnemyChargingSkillState, TutorialEnemyDefaultState>(state => !IsSkillInState<ChargingState>(state));

        // PrecedingAction State
        // PrecedingAction�� ������ ��, Message�� �Ѿ���� InSkillActionState�� ����, �ƴϸ� PlayerDefaultState�� ���� 
        MakeTransition<TutorialEnemyInSkillPrecedingActionState, TutorialEnemyInSkillActionState>(EntityStateCommand.ToInSkillActionState);
        MakeTransition<TutorialEnemyInSkillPrecedingActionState, TutorialEnemyDefaultState>(state => !IsSkillInState<InPrecedingActionState>(state));

        // Action State
        // IsStateEnded�� true��� ��, Action�� �����ٸ� PlayerDefaultState�� ���� 
        MakeTransition<TutorialEnemyInSkillActionState, TutorialEnemyDefaultState>(state => (state as TutorialEnemyInSkillActionState).IsStateEnded);

        // Dead State
        // 1) DeadState �� DefaultState / ���� : IsDead�� false�� �� ���� 
        MakeTransition<TutorialEnemyDeadState, TutorialEnemyDefaultState>(state => !Owner.IsDead);
        #endregion

        #region Any Transition
        // �� Any Transition : StateMachine���� ToState Command�� �Ѿ���� ��� ToState�� ���� 
        MakeAnyTransition<TutorialEnemyDefaultState>(EntityStateCommand.ToDefaultState);

        // Entity�� �׾����� ��� DeadState�� ���� (Command�� �ƴ� transitionCondition�� �ִ� ����)
        // �� canTransitionToSelf�� ����Ʈ �Ű����� false �̱� ������ Dead���� �ٽ� Dead�� �Ѿ ���� ����.
        MakeAnyTransition<TutorialEnemyDeadState>(state => Owner.IsDead && !Owner.IsSelfDestructive);
        #endregion
    }

    // ���� ���� ���� Skill�� ���°� T�� �´ٸ� True�� �ƴ϶�� False�� ��ȯ
    private bool IsSkillInState<T>(State<TutorialEnemyEntity> state) where T : State<Skill>
        => (state as TutorialEnemySkillState).RunningSkill.IsInState<T>();
}
