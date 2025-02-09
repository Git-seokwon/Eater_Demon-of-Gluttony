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

    private List<InteractionPrefab> actionList = new();

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

    private void Awake()
    {
        Init();
        gameObject.SetActive(false);
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

        if((gameObject.activeSelf == true) && Input.GetKey(KeyCode.F))
        {
            // Debug.Log(CurrentItem + "가 선택되었다.");
            actionList[CurrentItem].DoAction();
        }
    }

    private void Init()
    {
        vlg = GetComponent<VerticalLayoutGroup>();
    }

    public void OpenUI() 
    {
        if(actionList.Count != 0)
        {
            gameObject.SetActive(true);
            foreach(var action in actionList)
            {
                GameObject obj = Instantiate(targetField, vlg.transform);
                obj.GetComponentInChildren<TextMeshProUGUI>(true).text = action.CodeName;
            }
            SetCheck(CurrentItem);
        }
        else
            gameObject.SetActive(false);
    }

    public void CloseUI() 
    {
        if (actionList.Count != 0)
        {
            foreach(Transform a in vlg.transform)
            {
                Destroy(a.gameObject);
            }
            gameObject.SetActive(false);
        }
    }

    public void AddAction(InteractionPrefab obj) => actionList.Add(obj); 
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
