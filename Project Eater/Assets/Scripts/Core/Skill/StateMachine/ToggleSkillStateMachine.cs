using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSkillStateMachine : StateMachine<Skill>
{
    protected override void AddStates()
    {
        AddState<ReadyState>();
        AddState<SearchingTargetState>();
        AddState<InActionState>();
        AddState<CooldownState>();

        // ※ 1 Layer 역할 : Skill을 사용할 때, 바로 Cooldown 시간을 주는 것 
        // → Skill을 껐다, 켰다, 껐다, 켰다 빠르게 반복하지 못하도록 Toggle을 할 때마다 약간의 Cooldown을 준다. 
        AddState<ReadyState>(1);
        AddState<CooldownState>(1);
    }

    // ※ Logic
    // 최초에 Use 명령이 넘어오면, 0번 Layer에서는 동작 Process를 진행하고, 1번 Layer에서는 Toggle에 따른 Cooldown을 부여하게 된다. 
    protected override void MakeTransitions()
    {
        #region Transition
        // ★ ReadyState 
        // ReadyState → SearchingTargetState
        MakeTransition<ReadyState, SearchingTargetState>(SkillExecuteCommand.Use);
        MakeTransition<ReadyState, CooldownState>(state => !Owner.IsCooldownCompleted);

        // ★ SearchingTargetState
        // SearchingTargetState → InActionState
        MakeTransition<SearchingTargetState, InActionState>(state => Owner.IsTargetSelectSuccessful);

        // ★ InActionState
        // InActionState → CooldownState
        MakeTransition<InActionState, CooldownState>(state => Owner.IsFinished && Owner.HasCooldown);
        MakeTransition<InActionState, CooldownState>(SkillExecuteCommand.Use, state => Owner.HasCooldown);
        // InActionState → CooldownState
        // ※ Cooldown이 없는 경우, Ready State로 전이
        MakeTransition<InActionState, ReadyState>(state => Owner.IsFinished);
        MakeTransition<InActionState, ReadyState>(SkillExecuteCommand.Use);

        // ★ CooldownState
        // CooldownState → ReadyState
        MakeTransition<CooldownState, ReadyState>(state => Owner.IsCooldownCompleted);

        #region Layer 1
        MakeTransition<ReadyState, CooldownState>(SkillExecuteCommand.Use, state => Owner.HasCooldown, 1);
        MakeTransition<CooldownState, ReadyState>(state => Owner.IsCooldownCompleted, 1);
        #endregion
        #endregion

        #region Any Transition
        MakeAnyTransition<CooldownState>(SkillExecuteCommand.CancelImmediately, state => Owner.IsActivated && Owner.HasCooldown);
        MakeAnyTransition<ReadyState>(SkillExecuteCommand.CancelImmediately);
        #endregion
    }
}
