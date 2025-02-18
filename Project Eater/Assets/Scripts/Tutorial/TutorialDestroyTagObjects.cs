using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1. ���忡 �����ϴ� Ư�� ���� óġ�Ѵ�. 
// 2. ���忡 �����ϴ� Ư�� �������� ȹ���Ѵ�. 
public class TutorialDestroyTagObjects : TutorialBase
{
    [SerializeField]
    private GameObject[]        objectList;

    public override void Enter()
    {
        // �÷��̾��� �̵�, ������ �����ϵ��� ����
        PlayerController.Instance.enabled = true;
        PlayerController.Instance.SetPlayerMode(PlayerMode.Devil);

        // �ı��ؾ��� ������Ʈ���� Ȱ��ȭ
        for (int i = 0; i < objectList.Length; ++i)
        {
            objectList[i].SetActive(true);
        }
    }

    public override void Execute(TutorialController controller)
    {
        if (objectList.Length == 0)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        PlayerController.Instance.enabled = false;
        PlayerController.Instance.SetPlayerMode(PlayerMode.Default);
    }
}
