using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObjectMovement : TutorialBase
{
    [SerializeField]
    private Transform targetObject; // 이동할 대상(GameObject)
    [SerializeField]
    private Vector3 endPosition; // 목표 위치
    [SerializeField]
    private float moveTime = 0.5f; // 이동 시간

    private bool isCompleted = false; // 이동 완료 여부

    public override void Enter()
    {
        isCompleted = false;
        StartCoroutine(Movement());
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
    }

    private IEnumerator Movement()
    {
        float current = 0;
        float percent = 0;
        Vector3 start = targetObject.position; // 시작 위치

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / moveTime;

            // 부드럽게 이동
            targetObject.position = Vector3.Lerp(start, endPosition, percent);

            yield return null;
        }

        isCompleted = true;
    }
}
