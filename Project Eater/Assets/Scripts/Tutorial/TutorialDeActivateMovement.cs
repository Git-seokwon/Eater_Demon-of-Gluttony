using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDeActivateMovement : TutorialBase
{
    public override void Enter()
    {
        // 플레이어의 이동, 공격이 불가능하도록 설정
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
