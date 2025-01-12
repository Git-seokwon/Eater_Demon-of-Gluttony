using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcEntity : MonoBehaviour
{
    // ��ȭ ��ȣ�ۿ� 
    public delegate IEnumerator DialogInterAction();

    // ��ȣ�ۿ� Ű Icon
    [SerializeField]
    private GameObject interActionIcon;
    // ��ȣ�ۿ� Ű 
    [SerializeField]
    private KeyCode interActionKey;

    // NPC ȣ���� - ȣ������ ���� ��ȭ ��ȣ�ۿ��� �߻��Ѵ�. 
    protected int affinity = 0;
    public int Affinity
    {
        get => affinity;
        set
        {
            if (affinity == value) return;
            affinity = Mathf.Max(value, 0);
        }
    }
    // ȣ����(affinity) �� ��ȭ ��ȣ�ۿ� List<Action>
    // �� ȣ���� �� ��ȭ�� �� NPC�� ��ũ��Ʈ���� Action Ÿ�� �޼���� �ۼ��ϰ� Start �Լ����� dialogInterActions�� Add �Ѵ�,
    protected List<DialogInterAction> dialogInterActions;
    protected bool startInterAction = false;

    protected virtual void Start()
    {
        dialogInterActions = new List<DialogInterAction>();
    }

    private void Update()
    {
        if (startInterAction && Input.GetKeyDown(interActionKey) && !PlayerController.Instance.IsInterActive)
        {
            PlayerController.Instance.IsInterActive = true;
            startInterAction = false;
            StartCoroutine(dialogInterActions[affinity]());
        }
    }

    // �÷��̾�� ��ȣ�ۿ� Ű �ƾ��� Ȱ��ȭ
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            interActionIcon.SetActive(true);
            startInterAction = true;
        }
    }

    // �÷��̾�� ��ȣ�ۿ� Ű ��Ȱ��ȭ 
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            startInterAction = false;
            interActionIcon.SetActive(false);
        }
    }
}
