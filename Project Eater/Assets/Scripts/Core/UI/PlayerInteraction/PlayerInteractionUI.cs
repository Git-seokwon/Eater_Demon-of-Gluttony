using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class PlayerInteractionUI : MonoBehaviour
{
    [SerializeField] private GameObject targetField;
    [SerializeField] private GameObject targetCheckField;
    [SerializeField] private Sprite checkSprite;
    [SerializeField] private PlayerMovement pm;

    private IReadOnlyList<InteractionPrefab> actionList;

    private VerticalLayoutGroup vlg;
    private int currentItem = 0;
    private GameObject targetCheckFieldInstance;

    private bool isInteractionAble = false;

    public bool IsInteractionAble => isInteractionAble;

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
        if (isInteractionAble)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                float input = Input.GetAxis("Mouse ScrollWheel");
                if (input < 0)
                    CurrentItem++;
                else
                    CurrentItem--;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                CloseUI(false);
                //Debug.Log(CurrentItem + "가 선택되었다.");
                PlayerLookAtTarget();
                actionList[CurrentItem]?.DoAction();
                CurrentItem = 0;
            }
        }
    }

    public void Init()
    {
        vlg = GetComponent<VerticalLayoutGroup>();
        targetCheckFieldInstance = Instantiate(targetCheckField);
        targetCheckFieldInstance.transform.SetParent(gameObject.transform.parent);
        targetCheckFieldInstance.SetActive(false);
    }

    public void OpenUI(IReadOnlyList<InteractionPrefab> actions, bool checkInteraction) 
    {
        isInteractionAble = checkInteraction;
        actionList = actions;

        if(actions.Count != 0)
        {
            gameObject.SetActive(true);
            targetCheckFieldInstance.SetActive(true);
            foreach(var action in actions)
            {
                //GameObject obj = Instantiate(targetField, vlg.transform);
                GameObject obj = PoolManager.Instance.ReuseGameObject(targetField, new Vector3(0, 0, 0), new Quaternion(0,0,0,0));
                obj.transform.SetParent(vlg.transform, false);
                obj.GetComponentInChildren<TextMeshProUGUI>(true).text = action.CodeName;
            }
            SetCheck(CurrentItem);
        }
        else
        {
            // Debug.Log("OpenUI - actions가 설정되지 않았습니다.");
            gameObject.SetActive(false);
        }
    }

    public void CloseUI(bool checkInteraction) 
    {
        isInteractionAble = checkInteraction;
        targetCheckFieldInstance.SetActive(false);

        if ((vlg.transform.childCount != 0) && gameObject.activeSelf)
        {
            //Debug.Log(vlg.transform.childCount + "개의 자식");

            while(vlg.transform.childCount != 0)
            {
                Transform cd = vlg.transform.GetChild(0);
                cd.SetParent(PoolManager.Instance.transform, false);
                cd.gameObject.SetActive(false);
            }
            /*
            foreach(Transform a in vlg.transform)
            {
                Debug.Log("보냇다!");
                a.SetParent(PoolManager.Instance.transform, false);
                a.gameObject.SetActive(false);
            }
            */

            gameObject.SetActive(false);
        }
    }

    private void SetCheck(int current) 
    {
        for(int i = 0; i< vlg.transform.childCount ; i++)
        {
            if(i == current)
            {
                Image tmg = vlg.transform.GetChild(i).Find("Image").GetComponent<UnityEngine.UI.Image>();
                tmg.sprite = checkSprite;
                tmg.color = Color.white;
                targetCheckFieldInstance.transform.SetParent(tmg.transform.parent, false);
                targetCheckFieldInstance.transform.localPosition = Vector3.zero;
            }
            else
            {
                Image tmg = vlg.transform.GetChild(i).Find("Image").GetComponent<UnityEngine.UI.Image>();
                tmg.sprite = null;
                tmg.color = new Color(0.1254f, 0.1254f, 0.0941f, 1);
            }
        }
    }

    private void PlayerLookAtTarget()
    {
        Transform self = gameObject.GetComponentInParent<Transform>();

        if(self.position.x - pm.transform.position.x < 0f)
            pm.LookAt(AimDirection.Left);
        else
            pm.LookAt(AimDirection.Right);
    }
}
