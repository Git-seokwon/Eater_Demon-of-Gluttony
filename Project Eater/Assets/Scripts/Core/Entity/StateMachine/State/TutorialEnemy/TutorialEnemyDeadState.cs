using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemyDeadState : State<TutorialEnemyEntity>
{
    private TutorialEnemyMovement enemyMovement;

    protected override void Setup() => enemyMovement = Entity.EnemyMovement;

    public override void Enter()
    {
        if (enemyMovement != null)
            enemyMovement.enabled = false;
    }

    public override void Exit()
    {
        if (enemyMovement)
            enemyMovement.enabled = true;
    }
}
