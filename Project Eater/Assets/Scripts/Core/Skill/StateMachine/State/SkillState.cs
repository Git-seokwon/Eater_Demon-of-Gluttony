using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillState : State<Skill>
{
    // ★ 중요점 ★ 
    // 구조적으로 봤을 때, Skill과 Entity가 서로의 Code를 직접적으로 호출하는 게 아니라 StateMachine이라는 매개체를 이용해서 통신하기 
    // 때문에 관련성 높은 두 Class의 결합이 느슨하다는 점 
    // → 직접적으로 호출하는 것보다 서로 내부적으로 무슨 일이 일어나는지 모르는 지금이 구조적으로 다 깔끔하다. 

    // Skill을 소유한 Owner의 StateMachine에게 상태 전환 Command와 Skill의 정보를 보내는 함수 
    // → Skill과 Entity가 통신을 하여 Sync를 맞춘다. 
    // Ex) Skill이 Casting 상태라면 이 함수를 이용해서 Owner Entity에게도 Casting 상태로 들어가라고 명령을 보내주고 동시에 Skill 정보도 보낸다. 
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
        // animatorParameter가 유효하다면 
        if (ownerStateMachine != null && animatorParameter.isValid)
        {
            // 인자로 받은 animatorParameter가 bool Type이면 owner의 StateMachine으로 인자로 받은 command를 보냄 
            // ※ StateMachine.ExecuteCommand : Command를 받아서 해당 Command를 가진 Transition을 실행하는 함수
            //                                : 성공적으로 실행되면 현재 State 정보가 바뀌게 된다. 
            // → Transition이 Command를 받아들였으면, State로 UsingSkill Message와 Skill 정보를 건넨다. 
            if (animatorParameter.parameterType == AnimatorParameterType.Bool && ownerStateMachine.ExecuteCommand(command))
                // ※ SendMessage : 현재 실행중인 CurrentStateData에 Message를 보내서 OnReceiveMessage를 실행하는 함수 
                //                : OnReceiveMessage 함수는 Message에 따라서 State 자신이 해야 할 일이 기록되어 있다. 
                // ※ EntityStateMessage.UsingSkill : 스킬 사용 Message
                // ※ (skill, animatorParameter) : extraData로 skill 정보와 그에 맞는 AnimatorParameter를 가짐
                ownerStateMachine.SendMessage(EntityStateMessage.UsingSkill, (skill, animatorParameter));
            // 인자로 받은 animatorParameter가 Trigger Type이라면 행동에 제약을 주지 않을 것이므로 ToDefaultState Command를 보내고 
            // Transition이 받아들였는지와 상관없이, State로 UsingSkill Message와 Skill 정보를 보낸다. 
            else if (animatorParameter.parameterType == AnimatorParameterType.Trigger)
            {
                ownerStateMachine.ExecuteCommand(EntityStateCommand.ToDefaultState);
                ownerStateMachine.SendMessage(EntityStateMessage.UsingSkill, (skill, animatorParameter));
            }
        }
    }
}
