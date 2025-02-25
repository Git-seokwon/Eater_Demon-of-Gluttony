using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGetSkill : TutorialBase
{
    [SerializeField]
    private Skill skill;

    public override void Enter()
    {
        var clone = GameManager.Instance.player.SkillSystem.Register(skill);
        GameManager.Instance.player.SkillSystem.Equip(clone, 1);
    }

    public override void Execute(TutorialController controller)
    {
        controller.SetNextTutorial();
    }

    public override void Exit()
    {
    }
}
