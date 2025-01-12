using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Baal : NpcEntity
{
    // Test�� �Լ� 
    [SerializeField]
    private TextMeshProUGUI textCountDown;

    protected override void Start()
    {
        base.Start();

        dialogInterActions.Add(Dialog_01);
    }

    // ��ȭ ���� �������� PlayerController.Instance.IsInterActive�� false�� �����ϱ�
    private IEnumerator Dialog_01()
    {
        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(1, DialogCharacter.BAAL));

        // ��� �б� ���̿� ���ϴ� �ൿ�� �߰��� �� �ִ�. 
        // ĳ���͸� �����̰ų� �������� ȹ���ϴ� ����... ����� 5 ~ 1 ī��Ʈ �ٿ� ����
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
        // �� �б��� ��ȭ ���� �� ȣ��
        DialogManager.Instance.DeActivate();

        textCountDown.gameObject.SetActive(true);
        textCountDown.text = "The End";

        yield return new WaitForSeconds(2);

        PlayerController.Instance.IsInterActive = false;
        UnityEditor.EditorApplication.ExitPlaymode();
    }
}
