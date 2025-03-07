using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] Transform arrow;
    [SerializeField] Transform menu;

    private List<Transform> menus = new();

    private int currentMenu = 0;

    public int CurrentMenu 
    {
        get { return currentMenu; }
        set
        {
            currentMenu = Mathf.Clamp(value, 0, menu.transform.childCount);
        }
    }

    private void Awake()
    {
        foreach (Transform x in menu)
            menus.Add(x);

        arrow.gameObject.SetActive(false);
    }

    void Update()
    {
        if(RectTransformUtility.RectangleContainsScreenPoint(menu.GetComponent<RectTransform>(), Input.mousePosition))
        {
            arrow.gameObject.SetActive(true);

            foreach(RectTransform m in menus)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(m, Input.mousePosition))
                {
                    VisualizeSelect(m);
                }
            }
        }
        else
        {
            arrow.gameObject.SetActive(false);
        }
            
        
    }

    private void VisualizeSelect(Transform target)
    {
        arrow.transform.position = target.position;
    }
}
