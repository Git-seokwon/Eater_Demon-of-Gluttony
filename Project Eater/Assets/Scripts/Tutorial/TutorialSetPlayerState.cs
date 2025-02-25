using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSetPlayerState : TutorialBase
{
    [SerializeField]
    private bool isActive = false;
    [SerializeField]
    private PlayerMode playerMode;

    public override void Enter()
    {
        // 플레이어의 이동, 공격이 불가능하도록 설정
        PlayerController.Instance.enabled = isActive;
        PlayerController.Instance.SetPlayerMode(playerMode);
    }

    public override void Execute(TutorialController controller)
    {
        controller.SetNextTutorial();
    }

    public override void Exit()
    {
        
    }
}
