using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSFX : TutorialBase
{
    [SerializeField]
    SoundEffectSO sfx;

    public override void Enter()
    {
        if (sfx != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(sfx);

            Debug.Log("½ÇÇà");
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
