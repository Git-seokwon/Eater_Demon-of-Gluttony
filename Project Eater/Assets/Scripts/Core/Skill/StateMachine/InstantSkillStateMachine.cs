using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� Skill���� ������ SkillStateMachine�� �Ҵ�Ǹ� �ش� StateMachine���� ���º�ȭ�� �Ͼ�� ���̴�. 
// Ex) Skill 01 : ReadyState
//     Skill 02 : CooldownState
//     Skill 03 : ChargingState
//     ...
public class InstantSkillStateMachine : StateMachine<Skill>
{
    protected override void AddStates()
    {
        AddState<ReadyState>();
        AddState<SearchingTargetState>();
        AddState<CastingState>();
        AddState<ChargingState>();
        AddState<InPrecedingActionState>();
        AddState<InActionState>();
        AddState<CooldownState>();
    }

    // �� ����� ������ �ſ� �߿�!
    // �� ���� ȿ���� �˻��� ��, �����Ų ������ �˻��ϱ� �����̴�. 
    // Ex) Ready State �������� ChargingState�� SearchingTargetState ���� ������ ���ÿ� �������� ��, 
    //     ���� ����� ChargingState���� ���� ������ �˻�Ǿ� ChargingState�� �Ѿ�� �ȴ�. 
    // �� Transition�� ������ �� �׳� �� �����ϸ� �Ǵ°� �ƴ϶� �������� �� �����غ��� �Ѵ�. 
    // �� ���� ������ �� �ϳ��� �������� ���δ�. 
    protected override void MakeTransitions()
    {
        #region Transition
        // �� ReadyState
        // �� Ready State�� �⺻������ ��� State�� ������ �Ǿ� �ִ�. (SkillState�� �⺻ ������)
        // ReadyState �� ChargingState
        MakeTransition<ReadyState, ChargingState>(SkillExecuteCommand.Use, state => Owner.IsUseCharge);
        MakeTransition<ReadyState, ChargingState>(SkillExecuteCommand.UseImmediately, state => Owner.IsUseCharge);
        // ReadyState �� SearchingTargetState
        MakeTransition<ReadyState, SearchingTargetState>(SkillExecuteCommand.Use, state =>
        Owner.IsTargetSelectionTiming(TargetSelectionTimingOption.Use));
        // ReadyState �� CastingState
        MakeTransition<ReadyState, CastingState>(SkillExecuteCommand.Use, state => Owner.IsUseCast);
        MakeTransition<ReadyState, CastingState>(SkillExecuteCommand.UseImmediately, state => Owner.IsUseCast);
        // ReadyState �� InPrecedingActionState
        MakeTransition<ReadyState, InPrecedingActionState>(SkillExecuteCommand.Use, state => Owner.HasPrecedingAction);
        MakeTransition<ReadyState, InPrecedingActionState>(SkillExecuteCommand.UseImmediately, state => Owner.HasPrecedingAction);
        // ReadyState �� InActionState
        // �� ���� ���� ���ǵ��� �������� ���� ��, ���� ���� ���� ���̵ǰ� �Ѵ�.
        MakeTransition<ReadyState, InActionState>(SkillExecuteCommand.Use);
        MakeTransition<ReadyState, InActionState>(SkillExecuteCommand.UseImmediately);
        // ReadyState �� CooldownState
        // �� CurrentCooldown�� 0�� �ƴϸ� Cooldown State�� ���� 
        MakeTransition<ReadyState, CooldownState>(state => !Owner.IsCooldownCompleted);

        // �� ChargingState
        // ChargingState �� InPrecedingActionState
        MakeTransition<ChargingState, InPrecedingActionState>(state => (state as ChargingState).IsChargeSuccessed && Owner.HasPrecedingAction);
        // ChargingState �� InActionState
        MakeTransition<ChargingState, InActionState>(state => (state as ChargingState).IsChargeSuccessed);
        // ChargingState �� CooldownState
        // �� Charge�� ���������� ��ġ�� ���ϰ�, �׳� �����ٸ� CooldownState�� ����
        MakeTransition<ChargingState, CooldownState>(state => (state as ChargingState).IsChargeEnded);

        // �� SearchingTargetState
        // SearchingTargetState �� CastingState
        MakeTransition<SearchingTargetState, CastingState>(state => Owner.IsTargetSelectSuccessful && Owner.IsUseCast);
        // SearchingTargetState �� InPrecedingActionState
        MakeTransition<SearchingTargetState, InPrecedingActionState>(state => Owner.IsTargetSelectSuccessful && Owner.HasPrecedingAction);
        // SearchingTargetState �� InActionState
        MakeTransition<SearchingTargetState, InActionState>(state => Owner.IsTargetSelectSuccessful);
        // SearchingTargetState �� CooldownState
        // �� Search �߿� Cooldown �ð��� �������� CooldownState�� ���� 
        MakeTransition<SearchingTargetState, CooldownState>(state => !Owner.IsCooldownCompleted);
        // SearchingTargetState �� ReadyState
        // �� Player�� Search�� ��������� ReadyState�� ���ư���.
        // Ex) Skill Search Timing�� ��ģ ���, Search�� ����ϴ� ���� CC�⸦ ���� ��
        MakeTransition<SearchingTargetState, ReadyState>(state => !Owner.IsSearchingTarget);

        // �� CastingState
        // CastingState �� InPrecedingActionState
        MakeTransition<CastingState, InPrecedingActionState>(state => Owner.IsCastCompleted && Owner.HasPrecedingAction);
        // CastingState �� InActionState
        MakeTransition<CastingState, InActionState>(state => Owner.IsCastCompleted);

        // �� InPrecedingActionState
        // InPrecedingActionState �� InActionState
        MakeTransition<InPrecedingActionState, InActionState>(state => (state as InPrecedingActionState).IsPrecedingActionEnded);

        // �� InActionState
        // InActionState �� CooldownState
        MakeTransition<InActionState, CooldownState>(state => Owner.IsFinished && Owner.HasCooldown);
        // InActionState �� ReadyState
        // �� Cooldown�� ���ٸ� �ٷ� Ready State�� ����
        MakeTransition<InActionState, ReadyState>(state => Owner.IsFinished);

        // �� CooldownState
        // CooldownState �� ReadyState
        MakeTransition<CooldownState, ReadyState>(state => Owner.IsCooldownCompleted);
        #endregion

        // Cancel ����� �Ѿ����, ���ǿ� ���� CooldownState�� ReadyState�� ���� 
        #region Any Transition
        // 1) Skill�� InActionState�̰�, Input Type�̶�� Cancel ����� ���� ����
        // �� Input Type�� ���, �ش� ApplyCount�� �Ҹ�ǰ� Skill Ȱ�� ���¸� �״�� �����Ѵ�
        // 2) Skill�� Activated ���°� Cooldown�� ���� Skill�̶�� CooldownState�� ����
        MakeAnyTransition<CooldownState>(SkillExecuteCommand.Cancel,
            state => !(IsInState<InActionState>() && Owner.ExecutionType == SkillExecutionType.Input) && Owner.IsActivated && Owner.HasCooldown);

        // �� ������ �������� �ʴٸ� Cancel ����� �Ѿ���� ��, ReadyState�� ���� 
        // �� Activated ���°� �ƴϰų� Cooldown�� ������ ������ ReadyState�� ���� 
        MakeAnyTransition<ReadyState>(SkillExecuteCommand.Cancel,
            state => !(IsInState<InActionState>() && Owner.ExecutionType == SkillExecutionType.Input));

        // CancelImmediately ����� ������ InActionState�� �ִ� Input Type Skill�̶� ��� �ٸ� State�� ���̵ȴ�. (��ų ���� ����)
        MakeAnyTransition<CooldownState>(SkillExecuteCommand.CancelImmediately, state => Owner.IsActivated && Owner.HasCooldown);
        MakeAnyTransition<ReadyState>(SkillExecuteCommand.CancelImmediately);
        #endregion
    }
}
