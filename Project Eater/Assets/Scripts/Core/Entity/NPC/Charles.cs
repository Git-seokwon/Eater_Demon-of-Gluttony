using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charles : NpcEntity
{
    protected override void Start()
    {
        base.Start();

        dialogInterActions.Add(Dialog_01);
    }

    private IEnumerator Dialog_01()
    {
        GameManager.Instance.CinemachineTarget.enabled = false;

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(1, DialogCharacter.CHARLES));

        string[] options = { "바알에 대해", "마인에 대해", "출구에 대해", "이곳에 대해" };
        int choice = 0;

        yield return StartCoroutine(DialogManager.Instance.ShowDialogChoices(options.Length, options, result =>
        {
            choice = result;
        }));

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(choice + 2, DialogCharacter.CHARLES));

        DialogManager.Instance.DeActivate();

        GameManager.Instance.CinemachineTarget.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        PlayerController.Instance.enabled = true;
    }
}
