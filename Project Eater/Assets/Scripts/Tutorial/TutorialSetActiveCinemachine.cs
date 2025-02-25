using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSetActiveCinemachine : TutorialBase
{
    [SerializeField]
    private bool isActive = false;

    public override void Enter()
    {
        GameManager.Instance.CinemachineTarget.enabled = isActive;        
    }

    public override void Execute(TutorialController controller)
    {
        controller.SetNextTutorial();
    }

    public override void Exit()
    {

    }
}
