using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillState : State<EnemyEntity>
{
    // ���� Entity�� ���� ���� Skill
    public Skill RunningSkill { get; private set; }

    // Entity�� �����ؾ� �� Animation�� Hash �� 
    protected int AnimatorParameterHash { get; private set; }

    public override void Enter()
    {
        // ��ų ��� ���� ��, ������ ���߱� 
        Entity.GetComponent<EnemyMovement>().Stop();
    }

    public override void Exit()
    {
        Entity.Animator?.SetBool(AnimatorParameterHash, false);

        if (RunningSkill.Movement == MovementInSkill.Stop)
        {
            if (Entity.TryGetComponent(out EnemyMovement enemyMovement))
            {
                enemyMovement.enabled = true;
            }
            else if (Entity.TryGetComponent(out BossMovement bossMovement))
            {
                bossMovement.enabled = true;
            }
        }

        RunningSkill = null;
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

        // Skill�� MovementInSkill Type�� Stop�̸� PlayerController ��Ȱ��ȭ 
        // �� Entity�� �������� ���ϰ� ������ �ִ´�. 
        if (RunningSkill.Movement == MovementInSkill.Stop)
        {
            if (Entity.TryGetComponent(out EnemyMovement enemyMovement))
            {
                enemyMovement.Stop();
                enemyMovement.enabled = false;
            }
            else if (Entity.TryGetComponent(out BossMovement bossMovement))
            {
                bossMovement.Stop();
                bossMovement.enabled = false;
            }
        }
            
        return true;
    }
}
