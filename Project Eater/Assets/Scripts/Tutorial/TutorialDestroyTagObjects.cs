using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 1. ���忡 �����ϴ� Ư�� ���� óġ�Ѵ�. 
// 2. ���忡 �����ϴ� Ư�� �������� ȹ���Ѵ�. 
public class TutorialDestroyTagObjects : TutorialBase
{
    [SerializeField]
    private GameObject[]        objectList;

    public override void Enter()
    {
    }

    public override void Execute(TutorialController controller)
    {
        if (objectList.All(obj => !obj.activeSelf)) // ��� ������Ʈ�� ��Ȱ��ȭ�Ǿ����� Ȯ��
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
    }
}
