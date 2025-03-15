using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] Transform arrow;
    [SerializeField] Transform menu;

    [SerializeField] LobbyOptionUI optionUI;

    [HideInInspector]
    public bool isClearTutorial;

    private bool isMouseMoving;
    private Vector3 currentMousePos = new Vector3(0, 0, 0);
    private List<Transform> menus = new();

    private int currentMenu = 0;
    private bool isOptionPoped = false;

    // 이거 option 창 여는 버튼
    [SerializeField] Button optionBtn;

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
        optionBtn.onClick.AddListener(() => optionUI.OnClickOption());
    }

    private void Start()
    {
        MusicManager.Instance.PlayMusic(GameResources.Instance.LobbyMenuMusic);
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

        if (!menus[0].gameObject.activeSelf)
            return;

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


        if (Input.GetKeyDown(KeyCode.F) && isOptionPoped)
        {
            switch (CurrentMenu)
            {
                case 0:
                    OnGameStartBtnClicked();
                    break;
                case 1:
                    //여기에 옵션창 켜는거 한줄 추가하면 끝
                    optionUI.OnClickOption();
                    break;
                case 2:
                    Application.Quit();
                    break;
            }
        }
    }

    public void OnGameStartBtnClicked()
    {
        if (!isClearTutorial)
        {
            SoundEffectManager.Instance.PlayLobbyEnterSound();
            // 씬 전환
            LoadingSceneUI.LoadScene("TutorialScene");
        }
        else
        {
            SoundEffectManager.Instance.PlayLobbyEnterSound();
            // 씬 전환
            LoadingSceneUI.LoadScene("MainScene");
        }
    }

    private void VisualizeSelect(Transform target)
    {
        arrow.transform.position = target.position;
    }
}
