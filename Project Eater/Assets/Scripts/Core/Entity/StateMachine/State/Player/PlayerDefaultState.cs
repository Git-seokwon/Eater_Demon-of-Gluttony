using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefaultState : State<PlayerEntity>  
{
    public override bool OnReceiveMessage(int message, object data)
    {
        if ((EntityStateMessage)message != EntityStateMessage.UsingSkill)
            return false;

        var tupleData = ((Skill skill, AnimatorParameter animatorParameter))data;

        // SkillState���� AnimatorParameter�� Trigger Type�̶�� Entity�� DefaultState�� ���̽�Ų ���� Message�� ������. 
        Entity.Animator?.SetTrigger(tupleData.Item2.Hash);

        return true;
    }
}
