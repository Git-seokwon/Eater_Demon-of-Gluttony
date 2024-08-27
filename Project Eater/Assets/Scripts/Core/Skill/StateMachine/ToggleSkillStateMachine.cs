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

        // �� 1 Layer ���� : Skill�� ����� ��, �ٷ� Cooldown �ð��� �ִ� �� 
        // �� Skill�� ����, �״�, ����, �״� ������ �ݺ����� ���ϵ��� Toggle�� �� ������ �ణ�� Cooldown�� �ش�. 
        AddState<ReadyState>(1);
        AddState<CooldownState>(1);
    }

    // �� Logic
    // ���ʿ� Use ����� �Ѿ����, 0�� Layer������ ���� Process�� �����ϰ�, 1�� Layer������ Toggle�� ���� Cooldown�� �ο��ϰ� �ȴ�. 
    protected override void MakeTransitions()
    {
        #region Transition
        // �� ReadyState 
        // ReadyState �� SearchingTargetState
        MakeTransition<ReadyState, SearchingTargetState>(SkillExecuteCommand.Use);
        MakeTransition<ReadyState, CooldownState>(state => !Owner.IsCooldownCompleted);

        // �� SearchingTargetState
        // SearchingTargetState �� InActionState
        MakeTransition<SearchingTargetState, InActionState>(state => Owner.IsTargetSelectSuccessful);

        // �� InActionState
        // InActionState �� CooldownState
        MakeTransition<InActionState, CooldownState>(state => Owner.IsFinished && Owner.HasCooldown);
        MakeTransition<InActionState, CooldownState>(SkillExecuteCommand.Use, state => Owner.HasCooldown);
        // InActionState �� CooldownState
        // �� Cooldown�� ���� ���, Ready State�� ����
        MakeTransition<InActionState, ReadyState>(state => Owner.IsFinished);
        MakeTransition<InActionState, ReadyState>(SkillExecuteCommand.Use);

        // �� CooldownState
        // CooldownState �� ReadyState
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
