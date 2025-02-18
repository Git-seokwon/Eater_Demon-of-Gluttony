using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObjectMovement : TutorialBase
{
    [SerializeField]
    private Transform targetObject; // �̵��� ���(GameObject)
    [SerializeField]
    private Vector3 endPosition; // ��ǥ ��ġ
    [SerializeField]
    private float moveTime = 0.5f; // �̵� �ð�

    private bool isCompleted = false; // �̵� �Ϸ� ����

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
        Vector3 start = targetObject.position; // ���� ��ġ

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / moveTime;

            // �ε巴�� �̵�
            targetObject.position = Vector3.Lerp(start, endPosition, percent);

            yield return null;
        }

        isCompleted = true;
    }
}
