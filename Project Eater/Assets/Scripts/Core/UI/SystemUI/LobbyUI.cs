using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] Transform arrow;
    [SerializeField] Transform menu;

    [HideInInspector]
    public bool isClearTutorial;

    private bool isMouseMoving;
    private Vector3 currentMousePos = new Vector3(0, 0, 0);
    private List<Transform> menus = new();

    private int currentMenu = 0;

    public int CurrentMenu 
    {
        get { return currentMenu; }
        set
        {
            currentMenu = Mathf.Clamp(value, 0, menu.transform.childCount-1);
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
        if (currentMousePos != Input.mousePosition)
        {
            isMouseMoving = true;
            currentMousePos = Input.mousePosition;
        }
        else
        {
            isMouseMoving = false;
        }

        if(isMouseMoving)
        {
            arrow.gameObject.SetActive(false);
            if (RectTransformUtility.RectangleContainsScreenPoint(menu.GetComponent<RectTransform>(), Input.mousePosition))
            {
                arrow.gameObject.SetActive(true);

                foreach (RectTransform m in menus)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(m, Input.mousePosition))
                    {
                        VisualizeSelect(m);
                        CurrentMenu = menus.IndexOf(m);
                    }
                }
            }
            else
            {
                arrow.gameObject.SetActive(false);
            }
        }
        else
        {
            KeyCode temp = KeyCode.None;
            if (Input.anyKey)
            {
                arrow.gameObject.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
                CurrentMenu += 1;

            if (Input.GetKeyDown(KeyCode.UpArrow))
                CurrentMenu -= 1;

            VisualizeSelect(menus[CurrentMenu]);
        }


        if(Input.GetKeyDown(KeyCode.F) || (Input.GetMouseButtonDown(0) && RectTransformUtility.RectangleContainsScreenPoint(menu.GetComponent<RectTransform>(), Input.mousePosition)))
        {
            switch (CurrentMenu)
            {
                case 0:
                    if (!isClearTutorial)
                        LoadingSceneUI.LoadScene("TutorialScene");
                    else
                        LoadingSceneUI.LoadScene("MainScene");
                    break;
                case 1:
                    break;
                case 2:
                    Application.Quit();
                    break;
            }
        }
    }

    private void VisualizeSelect(Transform target)
    {
        arrow.transform.position = target.position;
    }
}
