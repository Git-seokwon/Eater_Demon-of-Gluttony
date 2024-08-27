// EntityStateMachine에서 쓸 Command
public enum EntityStateCommand
{
    ToDefaultState,                 // DefaultState로 전이

                                    // ※ Skill을 사용하는 상태로 전이
    ToCastingSkillState,            // 캐스팅 
    ToChargingSkillState,           // 차징 
    ToInSkillPrecedingActionState,  // 사전 동작
    ToInSkillActionState,           // 스킬 액션

                                    // ※ Crowd Control(CC) : 상태 이상에 걸린 상태로 전이
    ToStunningState,                // 스턴
}

// EntityStateMachine에서 쓸 Message
public enum EntityStateMessage { UsingSkill } // Skill 사용에 대한 처리를 해주기 위한 Message