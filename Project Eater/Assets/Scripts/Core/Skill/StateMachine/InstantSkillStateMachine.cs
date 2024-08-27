using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 각 Skill마다 고유한 SkillStateMachine이 할당되며 해당 StateMachine에서 상태변화가 일어나는 것이다. 
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

    // ★ 연결된 순서가 매우 중요!
    // → 전이 효과를 검사할 때, 연결시킨 순서로 검사하기 때문이다. 
    // Ex) Ready State 기준으로 ChargingState와 SearchingTargetState 전이 조건을 동시에 만족했을 때, 
    //     먼저 연결된 ChargingState부터 전이 조건이 검사되어 ChargingState로 넘어가게 된다. 
    // ★ Transition을 구성할 때 그냥 막 연결하면 되는게 아니라 순서까지 잘 생각해봐야 한다. 
    // → 연결 순서가 또 하나의 조건으로 쓰인다. 
    protected override void MakeTransitions()
    {
        #region Transition
        // ★ ReadyState
        // ※ Ready State는 기본적으로 모든 State와 연결이 되어 있다. (SkillState의 기본 상태임)
        // ReadyState → ChargingState
        MakeTransition<ReadyState, ChargingState>(SkillExecuteCommand.Use, state => Owner.IsUseCharge);
        MakeTransition<ReadyState, ChargingState>(SkillExecuteCommand.UseImmediately, state => Owner.IsUseCharge);
        // ReadyState → SearchingTargetState
        MakeTransition<ReadyState, SearchingTargetState>(SkillExecuteCommand.Use, state =>
        Owner.IsTargetSelectionTiming(TargetSelectionTimingOption.Use));
        // ReadyState → CastingState
        MakeTransition<ReadyState, CastingState>(SkillExecuteCommand.Use, state => Owner.IsUseCast);
        MakeTransition<ReadyState, CastingState>(SkillExecuteCommand.UseImmediately, state => Owner.IsUseCast);
        // ReadyState → InPrecedingActionState
        MakeTransition<ReadyState, InPrecedingActionState>(SkillExecuteCommand.Use, state => Owner.HasPrecedingAction);
        MakeTransition<ReadyState, InPrecedingActionState>(SkillExecuteCommand.UseImmediately, state => Owner.HasPrecedingAction);
        // ReadyState → InActionState
        // ※ 위의 전이 조건들이 부합하지 않을 때, 따로 조건 없이 전이되게 한다.
        MakeTransition<ReadyState, InActionState>(SkillExecuteCommand.Use);
        MakeTransition<ReadyState, InActionState>(SkillExecuteCommand.UseImmediately);
        // ReadyState → CooldownState
        // ※ CurrentCooldown이 0이 아니면 Cooldown State로 전이 
        MakeTransition<ReadyState, CooldownState>(state => !Owner.IsCooldownCompleted);

        // ★ ChargingState
        // ChargingState → InPrecedingActionState
        MakeTransition<ChargingState, InPrecedingActionState>(state => (state as ChargingState).IsChargeSuccessed && Owner.HasPrecedingAction);
        // ChargingState → InActionState
        MakeTransition<ChargingState, InActionState>(state => (state as ChargingState).IsChargeSuccessed);
        // ChargingState → CooldownState
        // ※ Charge를 성공적으로 마치지 못하고, 그냥 끝났다면 CooldownState로 전이
        MakeTransition<ChargingState, CooldownState>(state => (state as ChargingState).IsChargeEnded);

        // ★ SearchingTargetState
        // SearchingTargetState → CastingState
        MakeTransition<SearchingTargetState, CastingState>(state => Owner.IsTargetSelectSuccessful && Owner.IsUseCast);
        // SearchingTargetState → InPrecedingActionState
        MakeTransition<SearchingTargetState, InPrecedingActionState>(state => Owner.IsTargetSelectSuccessful && Owner.HasPrecedingAction);
        // SearchingTargetState → InActionState
        MakeTransition<SearchingTargetState, InActionState>(state => Owner.IsTargetSelectSuccessful);
        // SearchingTargetState → CooldownState
        // ※ Search 중에 Cooldown 시간이 생겼으면 CooldownState로 전이 
        MakeTransition<SearchingTargetState, CooldownState>(state => !Owner.IsCooldownCompleted);
        // SearchingTargetState → ReadyState
        // ※ Player가 Search를 취소했으면 ReadyState로 돌아간다.
        // Ex) Skill Search Timing을 놓친 경우, Search를 사용하는 도중 CC기를 맞음 등
        MakeTransition<SearchingTargetState, ReadyState>(state => !Owner.IsSearchingTarget);

        // ★ CastingState
        // CastingState → InPrecedingActionState
        MakeTransition<CastingState, InPrecedingActionState>(state => Owner.IsCastCompleted && Owner.HasPrecedingAction);
        // CastingState → InActionState
        MakeTransition<CastingState, InActionState>(state => Owner.IsCastCompleted);

        // ★ InPrecedingActionState
        // InPrecedingActionState → InActionState
        MakeTransition<InPrecedingActionState, InActionState>(state => (state as InPrecedingActionState).IsPrecedingActionEnded);

        // ★ InActionState
        // InActionState → CooldownState
        MakeTransition<InActionState, CooldownState>(state => Owner.IsFinished && Owner.HasCooldown);
        // InActionState → ReadyState
        // ※ Cooldown이 없다면 바로 Ready State로 전이
        MakeTransition<InActionState, ReadyState>(state => Owner.IsFinished);

        // ★ CooldownState
        // CooldownState → ReadyState
        MakeTransition<CooldownState, ReadyState>(state => Owner.IsCooldownCompleted);
        #endregion

        // Cancel 명령이 넘어오면, 조건에 따라서 CooldownState나 ReadyState로 전이 
        #region Any Transition
        // 1) Skill이 InActionState이고, Input Type이라면 Cancel 명령을 듣지 않음
        // → Input Type인 경우, 해당 ApplyCount만 소모되고 Skill 활성 상태를 그대로 유지한다
        // 2) Skill이 Activated 상태고 Cooldown을 가진 Skill이라면 CooldownState로 전이
        MakeAnyTransition<CooldownState>(SkillExecuteCommand.Cancel,
            state => !(IsInState<InActionState>() && Owner.ExecutionType == SkillExecutionType.Input) && Owner.IsActivated && Owner.HasCooldown);

        // 위 조건이 부합하지 않다면 Cancel 명령이 넘어왔을 때, ReadyState로 전이 
        // → Activated 상태가 아니거나 Cooldown을 가지지 않으면 ReadyState로 전이 
        MakeAnyTransition<ReadyState>(SkillExecuteCommand.Cancel,
            state => !(IsInState<InActionState>() && Owner.ExecutionType == SkillExecutionType.Input));

        // CancelImmediately 명령을 받으면 InActionState에 있는 Input Type Skill이라도 즉시 다른 State로 전이된다. (스킬 강제 종료)
        MakeAnyTransition<CooldownState>(SkillExecuteCommand.CancelImmediately, state => Owner.IsActivated && Owner.HasCooldown);
        MakeAnyTransition<ReadyState>(SkillExecuteCommand.CancelImmediately);
        #endregion
    }
}
