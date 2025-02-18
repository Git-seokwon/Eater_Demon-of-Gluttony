using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDialog : TutorialBase
{
    [SerializeField]
    private int branch;

    private bool isCompleted = false; // 대화 완료 여부

    public override void Enter()
    {
        // 코루틴 실행
        StartCoroutine(WaitForDialog());
    }

    public override void Execute(TutorialController controller)
    {
        // 대화가 완료되었으면 다음 튜토리얼로 이동
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
        // 대화가 끝날 때까지 기다림
        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(branch, DialogCharacter.TUTORIAL));

        isCompleted = true; // 대화 완료
        // 대사창 닫기 
        DialogManager.Instance.DeActivate();
    }
}
