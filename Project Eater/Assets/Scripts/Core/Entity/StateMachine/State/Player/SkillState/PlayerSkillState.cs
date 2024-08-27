using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillState : State<PlayerEntity>
{
    // ���� Entity�� ���� ���� Skill
    public Skill RunningSkill { get; private set; }
    
    // Entity�� �����ؾ� �� Animation�� Hash �� 
    protected int AnimatorParameterHash { get; private set; }

    public override void Enter()
    {
        // ��ų ��� ���� ��, ������ ���߱� 
        Entity.GetComponent<PlayerMovement>().Stop();

        // Player & Skill�� MovementInSkill Type�� Stop�̸� PlayerController ��Ȱ��ȭ 
        // �� Player�� �������� ���ϰ� ������ �ִ´�. 
        var playerContorller = Entity.GetComponent<PlayerController>();
        if (playerContorller && RunningSkill.Movement == MovementInSkill.Stop)
            playerContorller.enabled = false;
    }

    public override void Exit()
    {
        Entity.Animator?.SetBool(AnimatorParameterHash, false);

        RunningSkill = null;

        var playerContorller = Entity.GetComponent<PlayerController>();
        if (playerContorller && RunningSkill.Movement == MovementInSkill.Stop)
            playerContorller.enabled = true;
    }

    public override bool OnReceiveMessage(int message, object data)
    {
        // Skill���� Entity�� �޼����� �Ѱ��ش�. 
        // �� �̶�, EntityStateMessage Type�� UsingSkill�̿��� �Ѵ�. 
        if ((EntityStateMessage)message != EntityStateMessage.UsingSkill)
            return false;

        // �� data : Message�� ���� Skill�� Entity�� �����ؾ� �ϴ� AnimatorParameter ������ ��� Tuple
        var tupleData = ((Skill, AnimatorParameter))data;

        // �� Tuple �ʵ��� �⺻ �̸� : Item1, Item2, Item3 ... 
        RunningSkill = tupleData.Item1;
        AnimatorParameterHash = tupleData.Item2.Hash;

        // RunningSkill null üũ 
        Debug.Assert(RunningSkill != null,
           $"CastingSkillState({message})::OnReceiveMessage - �߸��� data�� ���޵Ǿ����ϴ�.");

        // Entity�� Parameter�� ���缭 Animation�� ���� 
        Entity.Animator?.SetBool(AnimatorParameterHash, true);

        return true;
    }
}
