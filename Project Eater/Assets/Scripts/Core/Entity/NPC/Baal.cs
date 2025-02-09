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
    }

    // 대화 종료 시점마다 PlayerController.Instance.IsInterActive를 false로 설정하기
    private IEnumerator Dialog_01()
    {
        GameManager.Instance.CinemachineTarget.enabled = false;

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(1, DialogCharacter.BAAL));

        string[] options = { "바알에 대해", "마인에 대해" };
        int choice = 0;

        yield return StartCoroutine(DialogManager.Instance.ShowDialogChoices(options.Length, options, result =>
        {
            choice = result;
        }));

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(choice + 2, DialogCharacter.BAAL));
        // 한 분기의 대화 종료 시 호출
        DialogManager.Instance.DeActivate();

        GameManager.Instance.CinemachineTarget.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        PlayerController.Instance.enabled = true;
    }
}
