using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// UI에 붙여놓을 스크립트
public class PlayerInteractionUI : MonoBehaviour
{
    [SerializeField] private GameObject targetField;

    public delegate void ActionsChangeHandler(string name);
    private event ActionsChangeHandler onActionsChange;
    private VerticalLayoutGroup vlg;
    private Dictionary<string, string> actions = new();

    private int currentItem = 0;

    public int CurrentItem
    {
        get { return currentItem; }
        private set 
        {
            if (actions.Count != 0)
            {
                currentItem = Mathf.Clamp(value, 0, actions.Count - 1);
                SetCheck(CurrentItem);
            }
            else
                currentItem = 0;
        }
    }

    private void Awake()
    {
        Init();
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
    }

    private void Init()
    {
        vlg = GetComponent<VerticalLayoutGroup>();
        onActionsChange += OnActionsChange;
    }

    public void AddAction(string name, string action)
    {
        if(!actions.Any(x => (x.Key == name)))
        {
            //Debug.Log(name + " registered");
            actions.Add(name, action);
            onActionsChange?.Invoke(name);
        }
    }

    public void DeleteAction(string name)
    {
        if(actions.Any(x => (x.Key == name)))
        {
            //Debug.Log(name + " Deleted");
            actions.Remove(name);
            onActionsChange?.Invoke(name);
        }
    }

    private void OnActionsChange(string name)
    {
        if(actions.Count == 0)
        {
            Destroy(vlg.transform.Find(name).gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
            if(!actions.Any(x => x.Key == name))
            {
                Destroy(vlg.transform.Find(name).gameObject);
            }
            else
            {
                GameObject obj = Instantiate(targetField) as GameObject;
                obj.name = name;
                obj.GetComponentInChildren<TextMeshProUGUI>().text = name + " " + actions[name];
                obj.transform.SetParent(vlg.transform, false);
            }
            gameObject.SetActive(true);
        }

        CurrentItem = 0;
    }

    private void SetCheck(int current)
    {
        for(int i = 0; i< vlg.transform.childCount ; i++)
        {
            if(i == current)
                vlg.transform.GetChild(i).Find("Image").GetComponent<Image>().color = Color.red;
            else
                vlg.transform.GetChild(i).Find("Image").GetComponent<Image>().color = Color.white;
        }
    }

}
