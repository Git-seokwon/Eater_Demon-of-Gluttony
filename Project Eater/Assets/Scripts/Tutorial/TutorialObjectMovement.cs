using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObjectMovement : TutorialBase
{
    [SerializeField]
    private Transform targetObject; // 이동할 대상(GameObject)
    [SerializeField]
    private Vector3 endPosition; // 목표 위치

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
