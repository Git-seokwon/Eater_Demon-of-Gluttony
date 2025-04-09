// EntityStateMachine���� �� Command
public enum EntityStateCommand
{
    ToDefaultState,                 // DefaultState�� ����

                                    // �� Skill�� ����ϴ� ���·� ����
    ToCastingSkillState,            // ĳ���� 
    ToChargingSkillState,           // ��¡ 
    ToInSkillPrecedingActionState,  // ���� ����
    ToInSkillActionState,           // ��ų �׼�

                                    // �� Crowd Control(CC) : ���� �̻� �ɸ� ���·� ����
    ToStunningState,                // ����

    ToDeadState,                    // ���
}

// EntityStateMachine���� �� Message
public enum EntityStateMessage { UsingSkill } // Skill ��뿡 ���� ó���� ���ֱ� ���� Message