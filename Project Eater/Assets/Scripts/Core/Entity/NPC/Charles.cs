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

        string[] options = { "제 몸에 무엇을 집어넣으신 거죠?", "이 안에 다른 사람들도 있나요?",
                             "저를 계속 가둬둘 셈인가요?", "여긴 뭘 하는 곳이죠...?" };
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
