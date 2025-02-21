using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDeActivateMovement : TutorialBase
{
    public override void Enter()
    {
        // �÷��̾��� �̵�, ������ �Ұ����ϵ��� ����
        PlayerController.Instance.enabled = false;
        PlayerController.Instance.SetPlayerMode(PlayerMode.Default);
    }

    public override void Execute(TutorialController controller)
    {
        
    }

    public override void Exit()
    {
        
    }
}
