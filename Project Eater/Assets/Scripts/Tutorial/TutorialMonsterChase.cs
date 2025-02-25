using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMonsterChase : TutorialBase
{
    [SerializeField]
    private TutorialEnemyMovement[] targets;

    public override void Enter()
    {
        foreach (var target in targets)
        {
            target.isMoveStart = true;
            target.GetComponent<CoachellaAI>().SetTutorialEnemy(0, 0);
            target.MoveEnemy();
        }
    }

    public override void Execute(TutorialController controller)
    {
        controller.SetNextTutorial();
    }

    public override void Exit()
    {
        
    }
}
