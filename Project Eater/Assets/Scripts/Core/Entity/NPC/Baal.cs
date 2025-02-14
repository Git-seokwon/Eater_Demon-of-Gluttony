using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Baal : NpcEntity
{
    protected override void Start()
    {
        base.Start();

        dialogInterActions.Add(Dialog_01);
        dialogInterActions.Add(Dialog_02);
    }

    // ��ȭ ���� �������� PlayerController.Instance.IsInterActive�� false�� �����ϱ�
    private IEnumerator Dialog_01()
    {
        GameManager.Instance.CinemachineTarget.enabled = false;

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(1, DialogCharacter.BAAL));

        string[] options = { "�˼������� ����� ������?", "������ ���θ��� �ִٰ� �ߴµ�..���ο� ���� �˷��ֽ� �� �ֳ���?" };
        int choice = 0;

        yield return StartCoroutine(DialogManager.Instance.ShowDialogChoices(options.Length, options, result =>
        {
            choice = result;
        }));

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(choice + 2, DialogCharacter.BAAL));
        // �� �б��� ��ȭ ���� �� ȣ��
        DialogManager.Instance.DeActivate();

        GameManager.Instance.CinemachineTarget.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        PlayerController.Instance.enabled = true;
    }

    private IEnumerator Dialog_02()
    {
        GameManager.Instance.CinemachineTarget.enabled = false;

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(4, DialogCharacter.BAAL));

        // TODO : �ع� ��ų ��� �ع� 


        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(5, DialogCharacter.BAAL));

        // ���� ��� �б�� ���� 
        affinity = 0;

        GameManager.Instance.CinemachineTarget.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        PlayerController.Instance.enabled = true;
    }
}
