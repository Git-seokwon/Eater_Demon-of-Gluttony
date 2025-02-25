using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWait : TutorialBase
{
    [SerializeField] private float waitTime = 1.0f; // ����� �ð� (��)

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
        // �ʿ��ϸ� �߰� ���� ���� ���� ����
    }

    private IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(waitTime);
        isCompleted = true;
    }
}
