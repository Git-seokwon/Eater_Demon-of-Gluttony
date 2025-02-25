using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBattleUIActive : TutorialBase
{
    [SerializeField]
    private PlayerHUD playerHUD;

    public override void Enter()
    {
        playerHUD.transform.parent.parent.gameObject.SetActive(true);
        playerHUD.Show();
    }

    public override void Execute(TutorialController controller)
    {
        controller.SetNextTutorial();
    }

    public override void Exit()
    {
    }
}
