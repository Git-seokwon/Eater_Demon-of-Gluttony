using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Baal : NpcEntity
{
    // Test용 함수 
    [SerializeField]
    private TextMeshProUGUI textCountDown;

    protected override void Start()
    {
        base.Start();

        dialogInterActions.Add(Dialog_01);
    }

    private IEnumerator Dialog_01()
    {
        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(1, DialogCharacter.BAAL));

        // 대사 분기 사이에 원하는 행동을 추가할 수 있다. 
        // 캐릭터를 움직이거나 아이템을 획득하는 등의... 현재는 5 ~ 1 카운트 다운 실행
        textCountDown.gameObject.SetActive(true);
        int count = 5;
        while (count > 0)
        {
            textCountDown.text = count.ToString();
            count--;

            yield return new WaitForSeconds(1);
        }
        textCountDown.gameObject.SetActive(false);

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(2, DialogCharacter.BAAL));

        textCountDown.gameObject.SetActive(true);
        textCountDown.text = "The End";

        yield return new WaitForSeconds(2);

        UnityEditor.EditorApplication.ExitPlaymode();
    }
}
