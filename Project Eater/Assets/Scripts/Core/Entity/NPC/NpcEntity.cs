using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcEntity : MonoBehaviour
{
    // 대화 상호작용 
    public delegate IEnumerator DialogInterAction();

    // 상호작용 키 Icon
    [SerializeField]
    private GameObject interActionIcon;
    // 상호작용 키 
    [SerializeField]
    private KeyCode interActionKey;

    // NPC 호감도 - 호감도에 따른 대화 상호작용이 발생한다. 
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
    // 호감도(affinity) 별 대화 상호작용 List<Action>
    // → 호감도 별 대화는 각 NPC별 스크립트에서 Action 타입 메서드로 작성하고 Start 함수에서 dialogInterActions에 Add 한다,
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

    // 플레이어면 상호작용 키 아아콘 활성화
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            interActionIcon.SetActive(true);
            startInterAction = true;
        }
    }

    // 플레이어면 상호작용 키 비활성화 
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            startInterAction = false;
            interActionIcon.SetActive(false);
        }
    }
}
