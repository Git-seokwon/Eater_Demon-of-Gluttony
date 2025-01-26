using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.SearchService;
using UnityEngine;

// ��ȣ�ۿ��� ������Ʈ�� �ٿ����� Monobehaviour ��ũ��Ʈ 
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private GameObject target; // ��ȣ�ۿ��� Target.
    [SerializeField] private GameObject interactionField; // ���� ��� UI ĵ����

    private PlayerInteractionUI pui;

    public delegate void PUIOnload(PlayerInteractionUI pui);

    public event PUIOnload PuiOnload;

    public PlayerInteractionUI PUI
    {
        get { return pui; }
        private set
        {
            pui = value;
            PuiOnload?.Invoke(pui);
        }
    }

    private void Awake()
    {
        GameObject obj = Instantiate(interactionField);
        RectTransform objscale = obj.GetComponent<RectTransform>();
        obj.transform.SetParent(transform, false);
        float pos = (gameObject.transform.localScale.x) / 2 + (objscale.rect.width) / 2;
        obj.transform.localPosition = new Vector3(pos, objscale.rect.height/2, 0);
        obj.SetActive(true);

        PUI = obj.GetComponent<PlayerInteractionUI>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(target))
        {
            //pui.AddAction(codeName, actions);
            pui.OpenUI();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(target))
        {
            //pui.DeleteAction(codeName);
            pui.CloseUI();
        }
    }
}
