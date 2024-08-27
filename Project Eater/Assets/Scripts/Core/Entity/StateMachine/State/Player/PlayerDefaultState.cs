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
        // → Trigger로 하면 PlayerController가 활성화 되기 때문에 애니메이션 도중 움직이게 될 수 있다. 
        // → 그래서 Trigger Animation은 주로 사용에 Delay가 없는 즉시 발동용 Skill에 활용할 수 있다. 
        Entity.Animator?.SetTrigger(tupleData.Item2.Hash);

        return true;
    }
}
