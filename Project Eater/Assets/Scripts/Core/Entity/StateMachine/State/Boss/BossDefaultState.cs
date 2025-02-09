using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDefaultState : State<BossEntity>
{
    public override bool OnReceiveMessage(int message, object data)
    {
        if ((EntityStateMessage)message != EntityStateMessage.UsingSkill)
            return false;

        var tupleData = ((Skill skill, AnimatorParameter animatorParameter))data;

        // SkillState에서 AnimatorParameter가 Trigger Type이라면 Entity를 DefaultState로 전이시킨 다음 Message를 보낸다. 
        // → Trigger Animation은 주로 사용에 Delay가 없는 즉시 발동용 Skill에 활용할 수 있다. 
        Entity.Animator?.SetTrigger(tupleData.Item2.Hash);

        return true;
    }
}
