using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 상호작용할 오브젝트에 붙여놓을 Monobehaviour 스크립트 
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private GameObject target; // 상호작용할 Target.

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
