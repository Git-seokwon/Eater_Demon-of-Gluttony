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
        dialogInterActions.Add(Dialog_03);

        // 해방 스킬 강화 기능 개방 이후 게임을 시작하면 해방 스킬 강화 기능을 넣어주기 
        // → 런타임 중에 변경된 정보는 저장할 수 없기 때문에 게임 시작이 불러온다. 
        if (affinity == 2)
        {
            var interaction = GetComponent<Interaction>();
            interaction.AddInteractionPrefab(latentSkillUpgradeInteractionPrefab);
        }
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
        affinity = 2;

        // 한 분기의 대화 종료 시 호출 
        DialogManager.Instance.DeActivate();

        GameManager.Instance.CinemachineTarget.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        PlayerController.Instance.enabled = true;
    }

    private IEnumerator Dialog_03()
    {
        GameManager.Instance.CinemachineTarget.enabled = false;

        string[] options = { "해방 스킬이란 대체 뭐죠?", "이대로 괜찮은 걸까요..." };
        int choice = 0;

        yield return StartCoroutine(DialogManager.Instance.ShowDialogChoices(options.Length, options, result =>
        {
            choice = result;
        }));

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(choice + 6, DialogCharacter.BAAL));
        // 한 분기의 대화 종료 시 호출
        DialogManager.Instance.DeActivate();

        GameManager.Instance.CinemachineTarget.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        PlayerController.Instance.enabled = true;
    }
}
