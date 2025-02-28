using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcEntity : MonoBehaviour
{
    // ��ȭ ��ȣ�ۿ� 
    public delegate IEnumerator DialogInterAction();

    // NPC ȣ���� - ȣ������ ���� ��ȭ ��ȣ�ۿ��� �߻��Ѵ�. 
    [HideInInspector]
    public int affinity = 0;
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

    protected virtual void Start()
    {
        dialogInterActions = new List<DialogInterAction>();
    }

    public void StartDialog() => StartCoroutine(dialogInterActions[affinity]());
}
