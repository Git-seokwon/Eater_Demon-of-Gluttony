using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillState : State<EnemyEntity>
{
    // 현재 Entity가 실행 중인 Skill
    public Skill RunningSkill { get; private set; }

    // Entity가 실행해야 할 Animation의 Hash 값 
    protected int AnimatorParameterHash { get; private set; }

    public override void Enter()
    {
        // 스킬 사용 직전 시, 움직임 멈추기 
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
        // Skill에서 Entity로 메세지를 넘겨준다. 
        // → 이때, EntityStateMessage Type은 UsingSkill이여야 한다. 
        if ((EntityStateMessage)message != EntityStateMessage.UsingSkill)
            return false;

        // ※ data : Message를 보낸 Skill과 Entity가 설정해야 하는 AnimatorParameter 정보가 담긴 Tuple
        var tupleData = ((Skill, AnimatorParameter))data;

        // ※ Tuple 필드의 기본 이름 : Item1, Item2, Item3 ... 
        RunningSkill = tupleData.Item1;
        AnimatorParameterHash = tupleData.Item2.Hash;

        // RunningSkill null 체크 
        Debug.Assert(RunningSkill != null,
           $"CastingSkillState({message})::OnReceiveMessage - 잘못된 data가 전달되었습니다.");

        // Entity가 Parameter에 맞춰서 Animation을 실행 
        Entity.Animator?.SetBool(AnimatorParameterHash, true);

        // Skill의 MovementInSkill Type이 Stop이면 PlayerController 비활성화 
        // → Entity는 움직이지 못하고 가만히 있는다. 
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
