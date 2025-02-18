using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sigma : NpcEntity
{
    // 초반 3개의 대화를 모두 완료했는지 판단하는 bool 배열
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

        string[] options = { "당신은 누구시죠? 당신도 실험체인가요...?", "여기는 어디인가요?",
                             "당신은 왜 그런 사람을 돕고 있는 건가요?" };
        int choice = 0;

        yield return StartCoroutine(DialogManager.Instance.ShowDialogChoices(options.Length, options, result =>
        {
            choice = result;
        }));

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(choice + 2, DialogCharacter.BAAL));

        // 모든 대화 선택지를 완료했으면 다음 대사 분기로 이동 
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

        // TODO : 도감 및 퀘스트 기능 해방


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
