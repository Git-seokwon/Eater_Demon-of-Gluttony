using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Baal : NpcEntity
{
    [SerializeField]
    private InteractionPrefab latentSkillUpgradeInteractionPrefab;

    protected override void Start()
    {
        base.Start();

        dialogInterActions.Add(Dialog_01);
        dialogInterActions.Add(Dialog_02);
    }

    // 대화 종료 시점마다 PlayerController.Instance.IsInterActive를 false로 설정하기
    private IEnumerator Dialog_01()
    {
        GameManager.Instance.CinemachineTarget.enabled = false;

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(1, DialogCharacter.BAAL));

        string[] options = { "죄송하지만 당신은 누구죠?", "마인이 가로막고 있다고..." };
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

    private IEnumerator Dialog_02()
    {
        GameManager.Instance.CinemachineTarget.enabled = false;

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(4, DialogCharacter.BAAL));

        // 해방 스킬 기능 해방 
        GetComponent<Interaction>().AddInteractionPrefab(latentSkillUpgradeInteractionPrefab);

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(5, DialogCharacter.BAAL));

        // 다음 대사 분기로 변경 
        affinity = 0;

        // 한 분기의 대화 종료 시 호출 
        DialogManager.Instance.DeActivate();

        GameManager.Instance.CinemachineTarget.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        PlayerController.Instance.enabled = true;
    }
}
