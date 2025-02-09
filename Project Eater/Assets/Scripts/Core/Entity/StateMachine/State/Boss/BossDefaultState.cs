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

        // SkillState���� AnimatorParameter�� Trigger Type�̶�� Entity�� DefaultState�� ���̽�Ų ���� Message�� ������. 
        // �� Trigger Animation�� �ַ� ��뿡 Delay�� ���� ��� �ߵ��� Skill�� Ȱ���� �� �ִ�. 
        Entity.Animator?.SetTrigger(tupleData.Item2.Hash);

        return true;
    }
}
