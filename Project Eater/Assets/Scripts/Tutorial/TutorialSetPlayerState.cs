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
        // �÷��̾��� �̵�, ������ �Ұ����ϵ��� ����
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
