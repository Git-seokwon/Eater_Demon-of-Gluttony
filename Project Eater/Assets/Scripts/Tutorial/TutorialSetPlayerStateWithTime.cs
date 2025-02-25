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

    private float elapsedTime = 0f; // ��� �ð�

    public override void Enter()
    {
        // �÷��̾��� �̵�, ���� ����
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
        // �÷��̾��� �̵�, ���� ����
        PlayerController.Instance.enabled = isActiveAfter;
        // �÷��̾� ���� 
        GameManager.Instance.player.PlayerMovement.Stop();
        PlayerController.Instance.SetPlayerMode(afterPlayerMode);
    }
}
