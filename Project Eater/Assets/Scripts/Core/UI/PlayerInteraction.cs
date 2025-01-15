using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��ȣ�ۿ��� ������Ʈ�� �ٿ����� Monobehaviour ��ũ��Ʈ 
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private GameObject target; // ��ȣ�ۿ��� Target.

    [SerializeField] private string name;
    [SerializeField] private string action;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(target))
        {
            //Debug.Log("TriggerEnter");
            PlayerInteractionUI pui = collision.gameObject.GetComponentInChildren<PlayerInteractionUI>(true);
            pui.AddAction(name, action);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(target))
        {
            //Debug.Log("TriggerExit");
            PlayerInteractionUI pui = collision.gameObject.GetComponentInChildren<PlayerInteractionUI>(true);
            pui.DeleteAction(name);
        }
    }
}
