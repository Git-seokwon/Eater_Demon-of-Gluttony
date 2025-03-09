using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGetSkill : TutorialBase
{
    [SerializeField]
    private Skill skill;

    public override void Enter()
    {
        var player = GameManager.Instance.player;
        var clone = player.SkillSystem.Register(skill);
        GameManager.Instance.player.SkillSystem.Equip(clone, 1);
    }

    public override void Execute(TutorialController controller)
    {
        controller.SetNextTutorial();
    }

    public override void Exit()
    {
        SaveManager.Instance.SaveShardShot();
        SaveManager.Instance.SaveCoachellaDNA();
    }
}
