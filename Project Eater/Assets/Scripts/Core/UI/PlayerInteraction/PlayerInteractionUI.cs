using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class PlayerInteractionUI : MonoBehaviour
{
    [SerializeField] private GameObject targetField;

    private IReadOnlyList<InteractionPrefab> actionList;

    private VerticalLayoutGroup vlg;
    private int currentItem = 0;

    public int CurrentItem
    {
        get { return currentItem; }
        private set
        {
            if (actionList.Count != 0)
            {
                currentItem = Mathf.Clamp(value, 0, actionList.Count - 1);
                SetCheck(CurrentItem);
            }
            else
                currentItem = 0;
        }
    }

    private void Update()
    {
        if ((gameObject.activeSelf == true) && (Input.GetAxis("Mouse ScrollWheel") != 0))
        {
            float input = Input.GetAxis("Mouse ScrollWheel");
            if (input < 0)
                CurrentItem++;
            else
                CurrentItem--;
        }

        // 콜라이더도 조건에 포함시키기..
        if((gameObject.activeSelf == true) && Input.GetKey(KeyCode.C))
        {
            // Debug.Log(CurrentItem + "가 선택되었다.");
            actionList[CurrentItem]?.DoAction();
        }
    }

    public void Init()
    {
        vlg = GetComponent<VerticalLayoutGroup>();
    }

    public void OpenUI(IReadOnlyList<InteractionPrefab> actions) 
    {
        actionList = actions;

        if(actions.Count != 0)
        {
            gameObject.SetActive(true);
            foreach(var action in actions)
            {
                GameObject obj = Instantiate(targetField, vlg.transform);
                obj.GetComponentInChildren<TextMeshProUGUI>(true).text = action.CodeName;
            }
            SetCheck(CurrentItem);
        }
        else
        {
            Debug.Log("OpenUI - actions가 설정되지 않았습니다.");
            gameObject.SetActive(false);
        }
            
    }

    public void CloseUI() 
    {
        if ((vlg.transform.childCount != 0) && gameObject.activeSelf)
        {
            foreach(Transform a in vlg.transform)
            {
                Destroy(a.gameObject);
            }
            gameObject.SetActive(false);
        }
    }

    private void SetCheck(int current) 
    {
        for(int i = 0; i< vlg.transform.childCount ; i++)
        {
            if(i == current)
                vlg.transform.GetChild(i).Find("Image").GetComponent<UnityEngine.UI.Image>().color = Color.red;
            else
                vlg.transform.GetChild(i).Find("Image").GetComponent<UnityEngine.UI.Image>().color = Color.white;
        }
    }
}
