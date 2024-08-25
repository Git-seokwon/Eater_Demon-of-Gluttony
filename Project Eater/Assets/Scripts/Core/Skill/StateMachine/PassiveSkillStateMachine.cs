using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkillStateMachine : StateMachine<Skill>
{
    protected override void AddStates()
    {
        // Passive Skill일 때는 Cast와 Charge Option을 사용하지 않음
        AddState<ReadyState>();
        AddState<SearchingTargetState>();
        AddState<InPrecedingActionState>();
        AddState<InActionState>();
        AddState<CooldownState>();
    }

    protected override void MakeTransitions()
    {
        #region Transition
        // ★ ReadyState
        // ReadyState → SearchingTargetState
        // ※ 따로 명령이 없더라도 SearchingTargetState로 전이 (자동 사용)
        MakeTransition<ReadyState, SearchingTargetState>(state => Owner.IsUseable);

        // ★ SearchingTargetState
        // SearchingTargetState → InPrecedingActionState
        MakeTransition<SearchingTargetState, InPrecedingActionState>(state => Owner.IsTargetSelectSuccessful && Owner.HasPrecedingAction);
        // SearchingTargetState → InActionState
        MakeTransition<SearchingTargetState, InActionState>(state => Owner.IsTargetSelectSuccessful);

        // ★ InPrecedingActionState
        // InPrecedingActionState → InActionState
        MakeTransition<InPrecedingActionState, InActionState>(state => (state as InPrecedingActionState).IsPrecedingActionEnded);

        // ★ InActionState
        // InActionState → CooldownState
        MakeTransition<InActionState, CooldownState>(state => Owner.IsFinished && Owner.HasCooldown);
        // InActionState → ReadyState
        MakeTransition<InActionState, ReadyState>(state => Owner.IsFinished);

        // ★ CooldownState
        // CooldownState → ReadyState
        MakeTransition<CooldownState, ReadyState>(state => Owner.IsCooldownCompleted);
        #endregion

        #region Any Transition
        MakeAnyTransition<CooldownState>(SkillExecuteCommand.CancelImmediately, state => Owner.IsActivated && Owner.HasCooldown);
        MakeAnyTransition<ReadyState>(SkillExecuteCommand.CancelImmediately);
        #endregion
    }
}
