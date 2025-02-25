using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TutorialLoseSkill : TutorialBase
{
    [SerializeField]
    private Skill skill;

    public override void Enter()
    {
        GameManager.Instance.player.SkillSystem.ReSetPlayerSkills();
    }

    public override void Execute(TutorialController controller)
    {
        controller.SetNextTutorial();
    }

    public override void Exit()
    {

    }
}
