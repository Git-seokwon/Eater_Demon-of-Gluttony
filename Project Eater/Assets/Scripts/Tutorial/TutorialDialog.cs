using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDialog : TutorialBase
{
    [SerializeField]
    private int branch;

    private bool isCompleted = false; // ��ȭ �Ϸ� ����

    public override void Enter()
    {
        // �ڷ�ƾ ����
        StartCoroutine(WaitForDialog());
    }

    public override void Execute(TutorialController controller)
    {
        // ��ȭ�� �Ϸ�Ǿ����� ���� Ʃ�丮��� �̵�
        if (isCompleted)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
    }

    private IEnumerator WaitForDialog()
    {
        // ��ȭ�� ���� ������ ��ٸ�
        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(branch, DialogCharacter.TUTORIAL));

        isCompleted = true; // ��ȭ �Ϸ�
        // ���â �ݱ� 
        DialogManager.Instance.DeActivate();
    }
}
