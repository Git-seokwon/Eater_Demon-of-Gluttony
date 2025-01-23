using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

// 상호작용할 오브젝트에 붙여놓을 Monobehaviour 스크립트 
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private GameObject target; // 상호작용할 Target.
    [SerializeField] private GameObject interactionField; // 옆에 띄울 UI 캔버스
    [SerializeField] private Dictionary<string, string> actions; // 상호작용 목록

    private PlayerInteractionUI pui;

    private void Awake()
    {
        GameObject obj = Instantiate(interactionField);
        RectTransform objscale = obj.GetComponent<RectTransform>();
        obj.transform.SetParent(transform, false);
        float pos = (gameObject.transform.localScale.x) / 2 + (objscale.rect.width) / 2;
        obj.transform.localPosition = new Vector3(pos, objscale.rect.height/2, 0);
        obj.SetActive(true);

        pui = obj.GetComponent<PlayerInteractionUI>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(target))
        {
            //pui.AddAction(codeName, actions);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(target))
        {
            //pui.DeleteAction(codeName);
        }
    }
}
