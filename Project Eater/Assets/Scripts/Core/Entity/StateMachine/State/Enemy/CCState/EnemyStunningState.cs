using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunningState : EnemyCCState
{
    public override string Description => "����";

    public override void Enter()
    {
        Entity.GetComponent<EnemyMovement>().Stop();

        // CC�⸦ ������ ��� Aactive Skill �ߵ��� ��� 
        Entity.SkillSystem.CancelAllActiveSkill();

        Entity.GetComponent<EnemyMovement>().enabled = false;
    }

    public override void Exit() => Entity.GetComponent<EnemyMovement>().enabled = true;
}
