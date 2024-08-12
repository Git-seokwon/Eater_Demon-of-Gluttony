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

        // SkillState에서 AnimatorParameter가 Trigger Type이라면 Entity를 DefaultState로 전이시킨 다음 Message를 보낸다. 
        Entity.Animator?.SetTrigger(tupleData.Item2.Hash);

        return true;
    }
}
