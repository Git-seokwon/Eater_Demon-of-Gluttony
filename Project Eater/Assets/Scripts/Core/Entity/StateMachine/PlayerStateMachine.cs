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

        // Skill이 Casting 중일 때, Player의 상태 
        AddState<CastingSkillState>();
        // Skill이 Charging 중일 때 Player의 상태
        AddState<ChargingSkillState>();
        // Skill이 Preceding Action 중일 때 Player의 상태 
        // ※ PrecedingAction : Skill을 발동하기 전에 하는 사전 Action 
        // Ex) 이즈리얼 비전이동 : 순간이동 후 가장 가까이에 있는 적에게 화살을 발사
        //     여기서, 순간이동이 PrecedingAction이고, 가장 가까운 적을 공격하는 것이 SkillAction 이다.
        AddState<InSkillPrecedingActionState>();
        // Skill이 발동 중일 때 Player의 상태 
        AddState<InSkillActionState>();
        // Player가 Stun CC기를 맞았을 때의 상태 
        AddState<StunningState>();
        // Player가 투지 스킬을 발동했을 때의 상태 
        AddState<PlayerSuperArmorState>();
    }

    protected override void MakeTransitions()
    {
        #region Default State
        // 1) DefaultState → RollingState / 조건 : Entity의 Movement가 있고(null이 아니고), IsRolling이 true라면 전이
        // ※ ?? : Movement가 null일 경우 false를 반환
        MakeTransition<PlayerDefaultState, DashState>(state => Owner.PlayerMovement?.IsDashing ?? false);
        // 2) DefaultState → Skill관련 State / 조건 : 해당하는 SkillState로 전이하라는 명령을 받으면 해당 명령에 맞는 SkillState로 전이 
        MakeTransition<PlayerDefaultState, CastingSkillState>(EntityStateCommand.ToCastingSkillState);
        MakeTransition<PlayerDefaultState, ChargingSkillState>(EntityStateCommand.ToChargingSkillState);
        MakeTransition<PlayerDefaultState, InSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<PlayerDefaultState, InSkillActionState>(EntityStateCommand.ToInSkillActionState);

        // Dash State
        // 1) DashState → DefaultState / 조건 : Dash가 실행 중이 아닐 때 전이
        MakeTransition<DashState, PlayerDefaultState>(state => !Owner.PlayerMovement.IsDashing);

        // Skill State
            // Casting State
        // 1) Casting이 끝나서 PrecedingAction State로 전이되는 경우 / 조건 : Message
        MakeTransition<CastingSkillState, InSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        // 2) Casting이 끝나고 PrecedingAction이 없어서 바로 SkillAction State로 넘어가는 경우 / 조건 : Message
        MakeTransition<CastingSkillState, InSkillActionState>(EntityStateCommand.ToInSkillActionState);
        // 3) Casting을 중간에 끊어서 다시 DefaultState로 넘어가는 경우 / 조건 : 현재 실행 중인 Skill이 Casting 상태가 아니라면 상태 전환
        // ※ 주의점 : CastingSkillState(Entity의 State), CastingState(Skill의 State)는 다른 것 
        // → Skill이 Casting 상태가 아니니깐 Skill을 사용하고 있는 대상도 Casting 상태에서 Default 상태로 돌아가라 
        // ※ transitionCondition : state => IsSkillInState<CastingState>(state)
        MakeTransition<CastingSkillState, PlayerDefaultState>(state => !IsSkillInState<CastingState>(state));

            // Charging State
        // Casting State와 경우의 수가 같다. 
        MakeTransition<ChargingSkillState, InSkillPrecedingActionState>(EntityStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<ChargingSkillState, InSkillActionState>(EntityStateCommand.ToInSkillActionState);
        MakeTransition<ChargingSkillState, PlayerDefaultState>(state => !IsSkillInState<ChargingState>(state));

            // PrecedingAction State
        // PrecedingAction이 끝났을 때, Message가 넘어오면 InSkillActionState로 전이, 아니면 PlayerDefaultState로 전이 
        MakeTransition<InSkillPrecedingActionState, InSkillActionState>(EntityStateCommand.ToInSkillActionState);
        MakeTransition<InSkillPrecedingActionState, PlayerDefaultState>(state => !IsSkillInState<InPrecedingActionState>(state));

            // Action State
        // IsStateEnded가 true라면 즉, Action이 끝났다면 PlayerDefaultState로 전이 
        MakeTransition<InSkillActionState, PlayerDefaultState>(state => (state as InSkillActionState).IsStateEnded);

        // Dead State
        // 1) DeadState → DefaultState / 조건 : IsDead가 false일 때 전이 
        MakeTransition<PlayerDeadState, PlayerDefaultState>(state => !Owner.IsDead);

        // SuperArmor State
        MakeTransition<PlayerSuperArmorState, PlayerDefaultState>(state => (state as PlayerSuperArmorState).isTimeOver);
        #endregion

        #region Any Transition
        // ※ Any Transition : StateMachine으로 ToState Command가 넘어오면 즉시 ToState로 전이 
        MakeAnyTransition<PlayerDefaultState>(EntityStateCommand.ToDefaultState);

        // Entity가 죽었으면 즉시 DeadState로 전이 (Command가 아닌 transitionCondition만 있는 버전)
        // → canTransitionToSelf가 디폴트 매개변수 false 이기 때문에 Dead에서 다시 Dead로 넘어갈 일은 없다.
        MakeAnyTransition<PlayerDeadState>(state => Owner.IsDead);

        // CC State
            // Stuning State
        MakeAnyTransition<StunningState>(EntityStateCommand.ToStunningState);

        // 투지 스킬에 의해 SuperArmor 상태로 전이 
        MakeAnyTransition<PlayerSuperArmorState>(EntityStateCommand.ToSuperArmorState);
        #endregion
    }

    // 현재 실행 중인 Skill의 상태가 T가 맞다면 True를 아니라면 False를 반환
    private bool IsSkillInState<T>(State<PlayerEntity> state) where T : State<Skill>
        => (state as PlayerSkillState).RunningSkill.IsInState<T>();
}
