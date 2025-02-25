using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSpriteFlip : TutorialBase
{
    [SerializeField] private SpriteRenderer targetSprite;

    public override void Enter()
    {
        targetSprite.flipX = true;
    }

    public override void Execute(TutorialController controller)
    {
        controller.SetNextTutorial();
    }

    public override void Exit()
    {

    }
}
