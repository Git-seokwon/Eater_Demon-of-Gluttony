using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMovement : TutorialBase
{
    [SerializeField]
    private GameObject movementTooltip; // 기본 이동 조작 툴팁 UI
    [SerializeField]
    private float tutorialTime = 0f;

    private float elapsedTime = 0f;

    public override void Enter()
    {
        movementTooltip.SetActive(true);
        PlayerController.Instance.enabled = true;

        elapsedTime = 0f;
    }

    public override void Execute(TutorialController controller)
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= tutorialTime)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        PlayerController.Instance.enabled = false;
    }
}
