using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStunningState : EnemyCCState
{
    public override string Description => "기절";

    public override void Enter()
    {
        Entity.GetComponent<EnemyMovement>().Stop();

        // CC기를 맞으면 모든 Aactive Skill 발동을 취소 
        Entity.SkillSystem.CancelAllActiveSkill();

        Entity.GetComponent<EnemyMovement>().enabled = false;
    }

    public override void Exit() => Entity.GetComponent<EnemyMovement>().enabled = true;
}
