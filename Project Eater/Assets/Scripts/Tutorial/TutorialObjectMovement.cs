using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObjectMovement : TutorialBase
{
    [SerializeField]
    private Transform targetObject; // �̵��� ���(GameObject)
    [SerializeField]
    private Vector3 endPosition; // ��ǥ ��ġ

    public override void Enter()
    {
        targetObject.transform.position = endPosition;
    }

    public override void Execute(TutorialController controller)
    {
        controller.SetNextTutorial();
    }

    public override void Exit()
    {
    }
}
