using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillState : State<Skill>
{
    // �� �߿��� �� 
    // ���������� ���� ��, Skill�� Entity�� ������ Code�� ���������� ȣ���ϴ� �� �ƴ϶� StateMachine�̶�� �Ű�ü�� �̿��ؼ� ����ϱ� 
    // ������ ���ü� ���� �� Class�� ������ �����ϴٴ� �� 
    // �� ���������� ȣ���ϴ� �ͺ��� ���� ���������� ���� ���� �Ͼ���� �𸣴� ������ ���������� �� ����ϴ�. 

    // Skill�� ������ Owner�� StateMachine���� ���� ��ȯ Command�� Skill�� ������ ������ �Լ� 
    // �� Skill�� Entity�� ����� �Ͽ� Sync�� �����. 
    // Ex) Skill�� Casting ���¶�� �� �Լ��� �̿��ؼ� Owner Entity���Ե� Casting ���·� ����� ����� �����ְ� ���ÿ� Skill ������ ������. 
    protected void TrySendCommandToOwner(Skill skill, EntityStateCommand command, AnimatorParameter animatorParameter)
    {
        if (Entity.Owner.IsPlayer)
        {
            var ownerStateMachine = (Entity.Owner as PlayerEntity).StateMachine;
            SendMessage(ownerStateMachine, skill, command, animatorParameter);    
        }
        else
        {
            var ownerStateMachine = (Entity.Owner as EnemyEntity).StateMachine;
            SendMessage(ownerStateMachine, skill, command, animatorParameter);
        }
    }

    protected void SendMessage<T>(MonoStateMachine<T> ownerStateMachine, Skill skill, EntityStateCommand command, 
        AnimatorParameter animatorParameter) where T : Entity
    {
        // animatorParameter�� ��ȿ�ϴٸ� 
        if (ownerStateMachine != null && animatorParameter.isValid)
        {
            // ���ڷ� ���� animatorParameter�� bool Type�̸� owner�� StateMachine���� ���ڷ� ���� command�� ���� 
            // �� StateMachine.ExecuteCommand : Command�� �޾Ƽ� �ش� Command�� ���� Transition�� �����ϴ� �Լ�
            //                                : ���������� ����Ǹ� ���� State ������ �ٲ�� �ȴ�. 
            // �� Transition�� Command�� �޾Ƶ鿴����, State�� UsingSkill Message�� Skill ������ �ǳٴ�. 
            if (animatorParameter.parameterType == AnimatorParameterType.Bool && ownerStateMachine.ExecuteCommand(command))
                // �� SendMessage : ���� �������� CurrentStateData�� Message�� ������ OnReceiveMessage�� �����ϴ� �Լ� 
                //                : OnReceiveMessage �Լ��� Message�� ���� State �ڽ��� �ؾ� �� ���� ��ϵǾ� �ִ�. 
                // �� EntityStateMessage.UsingSkill : ��ų ��� Message
                // �� (skill, animatorParameter) : extraData�� skill ������ �׿� �´� AnimatorParameter�� ����
                ownerStateMachine.SendMessage(EntityStateMessage.UsingSkill, (skill, animatorParameter));
            // ���ڷ� ���� animatorParameter�� Trigger Type�̶�� �ൿ�� ������ ���� ���� ���̹Ƿ� ToDefaultState Command�� ������ 
            // Transition�� �޾Ƶ鿴������ �������, State�� UsingSkill Message�� Skill ������ ������. 
            else if (animatorParameter.parameterType == AnimatorParameterType.Trigger)
            {
                ownerStateMachine.ExecuteCommand(EntityStateCommand.ToDefaultState);
                ownerStateMachine.SendMessage(EntityStateMessage.UsingSkill, (skill, animatorParameter));
            }
        }
    }
}
