using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSetPlayerStateWithTime : TutorialBase
{
    [SerializeField]
    private bool isActive = false;
    [SerializeField]
    private PlayerMode playerMode;

    [Space(10)]
    [SerializeField]
    private bool isActiveAfter = false;
    [SerializeField]
    private PlayerMode afterPlayerMode;

    [SerializeField]
    private float time;

    private float elapsedTime = 0f; // 경과 시간

    public override void Enter()
    {
        // 플레이어의 이동, 공격 설정
        PlayerController.Instance.enabled = isActive;
        PlayerController.Instance.SetPlayerMode(playerMode);
    }

    public override void Execute(TutorialController controller)
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= time)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        // 플레이어의 이동, 공격 설정
        PlayerController.Instance.enabled = isActiveAfter;
        // 플레이어 정지 
        GameManager.Instance.player.PlayerMovement.Stop();
        PlayerController.Instance.SetPlayerMode(afterPlayerMode);
    }
}
