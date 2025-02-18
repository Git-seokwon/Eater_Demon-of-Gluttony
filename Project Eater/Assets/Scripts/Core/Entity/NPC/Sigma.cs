using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sigma : NpcEntity
{
    // �ʹ� 3���� ��ȭ�� ��� �Ϸ��ߴ��� �Ǵ��ϴ� bool �迭
    private bool[] isChoiceDialogueComplete = new bool[3];

    protected override void Start()
    {
        base.Start();

        dialogInterActions.Add(Dialog_01);
        dialogInterActions.Add(Dialog_02);
        dialogInterActions.Add(Dialog_03);
        dialogInterActions.Add(Dialog_04);
    }

    private IEnumerator Dialog_01()
    {
        GameManager.Instance.CinemachineTarget.enabled = false;

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(1, DialogCharacter.SIGMA));

        string[] options = { "����� ��������? ��ŵ� ����ü�ΰ���...?", "����� ����ΰ���?",
                             "����� �� �׷� ����� ���� �ִ� �ǰ���?" };
        int choice = 0;

        yield return StartCoroutine(DialogManager.Instance.ShowDialogChoices(options.Length, options, result =>
        {
            choice = result;
        }));

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(choice + 2, DialogCharacter.BAAL));

        // ��� ��ȭ �������� �Ϸ������� ���� ��� �б�� �̵� 
        isChoiceDialogueComplete[choice] = true;
        if (CheckChoiceDialogueComplete())
            affinity = 1;

        DialogManager.Instance.DeActivate();

        GameManager.Instance.CinemachineTarget.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        PlayerController.Instance.enabled = true;
    }

    private IEnumerator Dialog_02()
    {
        GameManager.Instance.CinemachineTarget.enabled = false;

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(5, DialogCharacter.SIGMA));

        affinity = 2;

        DialogManager.Instance.DeActivate();

        GameManager.Instance.CinemachineTarget.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        PlayerController.Instance.enabled = true;
    }

    private IEnumerator Dialog_03()
    {
        GameManager.Instance.CinemachineTarget.enabled = false;

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(6, DialogCharacter.SIGMA));

        DialogManager.Instance.DeActivate();

        GameManager.Instance.CinemachineTarget.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        PlayerController.Instance.enabled = true;
    }

    private IEnumerator Dialog_04()
    {
        GameManager.Instance.CinemachineTarget.enabled = false;

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(7, DialogCharacter.SIGMA));

        // TODO : ���� �� ����Ʈ ��� �ع�


        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(8, DialogCharacter.SIGMA));

        affinity = 4;

        DialogManager.Instance.DeActivate();

        GameManager.Instance.CinemachineTarget.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        PlayerController.Instance.enabled = true;
    }

    private bool CheckChoiceDialogueComplete()
    {
        bool isComplete = true;
        for (int i = 0; i < isChoiceDialogueComplete.Length; i++)
        {
            if (isChoiceDialogueComplete[i] == false)
                isComplete = false;
        }

        return isComplete;
    }
}
