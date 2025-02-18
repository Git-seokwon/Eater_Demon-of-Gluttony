using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1. �÷��̾ Ư�� �������� ȹ��
// 2. �÷��̾ Ư�� ��ġ���� �̵� 
public class TutorialTrigger : TutorialBase
{
    [SerializeField]
    private Transform triggerObject;

    public bool isTrigger { set; get; } = false;

    public override void Enter()
    {
        // �÷��̾� �̵� ����
        PlayerController.Instance.enabled = true;
        // Trigger ������Ʈ Ȱ��ȭ
        triggerObject.gameObject.SetActive(true);
    }

    public override void Execute(TutorialController controller)
    {
        // �Ÿ� ����
        if ((triggerObject.position - PlayerController.Instance.transform.position).sqrMagnitude < 0.1f)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        // �÷��̾� �̵� �Ұ���
        PlayerController.Instance.enabled = false;
        // Trigger ������Ʈ ��Ȱ��ȭ
        triggerObject.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.Equals(triggerObject))
        {
            isTrigger = true;

            collision.gameObject.SetActive(false);
        }
    }
}
