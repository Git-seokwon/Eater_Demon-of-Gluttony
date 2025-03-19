using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine : MonoStateMachine<BossEntity>
{
    protected override void AddStates()
    {
        AddState<BossDefaultState>();
        AddState<BossDeadState>();

        // Skill이 Casting 중일 때, Enemy의 상태 
        AddState<BossCastingSkillState>(); 
        // Skill이 Charging 중일 때 Enemy의 상태
        AddState<BossChargingSkillState>(); 
        // Skill이 Preceding Action 중일 때 Enemy의 상태 
        // ※ PrecedingAction : Skill을 발동하기 전에 하는 사전 Action 
        // Ex) 이즈리얼 비전이동 : 순간이동 후 가장 가까이에 있는 적에게 화살을 발사
        //     여기서, 순간이동이 PrecedingAction이고, 가장 가까운 적을 공격하는 것이 SkillAction 이다.
        AddState<BossInSkillPrecedingActionState>();
        // Skill이 발동 중일 때 Enemy의 상태 
        AddState<BossInSkillActionState>();
        // Enemy가 Stun CC기를 맞았을 때의 상태 
        AddState<BossStunningState>();
    }

    protected override void MakeTransitions()
    {
        #region Default State
        // 1) DefaultState → Skill관련 State / 조건 : 해당하는 SkillState로 전이하라는 명령을 받으면 해당 명령에 맞는 SkillState로 전이 
        MakeTransition<BossDefaultState, BossCastingSkillState>(EntityStateCommand.ToCastingSkillState);
        MakeTransition<BossDefaultState, BossChargingSkillState>(EntityStateCommand.ToChargingSkillState);
        MakeTransition<BossDefaultState, BossInSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<BossDefaultState, BossInSkillActionState>(EntityStateCommand.ToInSkillActionState);

        // Skill State
        // Casting State
        // 1) Casting이 끝나서 PrecedingAction State로 전이되는 경우 / 조건 : Message
        MakeTransition<BossCastingSkillState, BossInSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        // 2) Casting이 끝나고 PrecedingAction이 없어서 바로 SkillAction State로 넘어가는 경우 / 조건 : Message
        MakeTransition<BossCastingSkillState, BossInSkillActionState>(EntityStateCommand.ToInSkillActionState);
        // 3) Casting을 중간에 끊어서 다시 DefaultState로 넘어가는 경우 / 조건 : 현재 실행 중인 Skill이 Casting 상태가 아니라면 상태 전환
        // ※ 주의점 : CastingSkillState(Entity의 State), CastingState(Skill의 State)는 다른 것 
        // → Skill이 Casting 상태가 아니니깐 Skill을 사용하고 있는 대상도 Casting 상태에서 Default 상태로 돌아가라 
        // ※ transitionCondition : state => IsSkillInState<CastingState>(state)
        MakeTransition<BossCastingSkillState, BossDefaultState>(state => !IsSkillInState<CastingState>(state));

        // Charging State
        // Casting State와 경우의 수가 같다. 
        MakeTransition<BossChargingSkillState, BossInSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<BossChargingSkillState, BossInSkillActionState>(EntityStateCommand.ToInSkillActionState);
        MakeTransition<BossChargingSkillState, BossDefaultState>(state => !IsSkillInState<ChargingState>(state));

        // PrecedingAction State
        // PrecedingAction이 끝났을 때, Message가 넘어오면 InSkillActionState로 전이, 아니면 PlayerDefaultState로 전이 
        MakeTransition<BossInSkillPrecedingActionState, BossInSkillActionState>(EntityStateCommand.ToInSkillActionState);
        MakeTransition<BossInSkillPrecedingActionState, BossDefaultState>(state => !IsSkillInState<InPrecedingActionState>(state));

        // Action State
        // IsStateEnded가 true라면 즉, Action이 끝났다면 PlayerDefaultState로 전이 
        MakeTransition<BossInSkillActionState, BossDefaultState>(state => (state as BossInSkillActionState).IsStateEnded);

        // Dead State
        // 1) DeadState → DefaultState / 조건 : IsDead가 false일 때 전이 
        MakeTransition<BossDeadState, BossDefaultState>(state => !Owner.IsDead);
        #endregion

        #region Any Transition
        // ※ Any Transition : StateMachine으로 ToState Command가 넘어오면 즉시 ToState로 전이 
        MakeAnyTransition<BossDefaultState>(EntityStateCommand.ToDefaultState);

        // Entity가 죽었으면 즉시 DeadState로 전이 (Command가 아닌 transitionCondition만 있는 버전)
        // → canTransitionToSelf가 디폴트 매개변수 false 이기 때문에 Dead에서 다시 Dead로 넘어갈 일은 없다.
        MakeAnyTransition<BossDeadState>(state => Owner.IsDead && !Owner.IsSelfDestructive);

        // CC State
        // → Stuning State
        MakeAnyTransition<BossStunningState>(EntityStateCommand.ToStunningState);
        #endregion
    }

    // 현재 실행 중인 Skill의 상태가 T가 맞다면 True를 아니라면 False를 반환
    private bool IsSkillInState<T>(State<BossEntity> state) where T : State<Skill>
        => (state as BossSkillState).RunningSkill.IsInState<T>();
}
