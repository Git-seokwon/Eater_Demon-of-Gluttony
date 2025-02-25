using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWait : TutorialBase
{
    [SerializeField] private float waitTime = 1.0f; // 대기할 시간 (초)

    private bool isCompleted = false;

    public override void Enter()
    {
        isCompleted = false;
        StartCoroutine(WaitCoroutine());
    }

    public override void Execute(TutorialController controller)
    {
        if (isCompleted)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        // 필요하면 추가 정리 로직 구현 가능
    }

    private IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(waitTime);
        isCompleted = true;
    }
}
