using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGetLatentSkill : TutorialBase
{
    public override void Enter()
    {
        var player = GameManager.Instance.player;
        player.SetUpLatentSkill();
        player.AcquireLatentSkill(0);
        player.ChangeLatentSkill(0);

        player.SkillSystem.SetupLatentSkills(player.CurrentLatentSkill.Level);
    }

    public override void Execute(TutorialController controller)
    {
        controller.SetNextTutorial();
    }

    public override void Exit()
    {
        SaveManager.Instance.SaveLatentSkill();
    }
}
