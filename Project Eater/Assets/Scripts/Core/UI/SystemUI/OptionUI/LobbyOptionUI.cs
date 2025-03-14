using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyOptionUI : MonoBehaviour
{
    [SerializeField]
    private GameObject OptionWindow;
    [SerializeField]
    private GameObject GameStart;
    [SerializeField]
    private GameObject Option;
    [SerializeField]
    private GameObject Exit;

    [SerializeField]
    private Button OptionBtn;
    [SerializeField]
    private Button ConfirmBtn;
    [SerializeField]
    private Button CancelBtn;

    // 이벤트 만들기
    public Action ConfirmSettingAction;
    public Action CancelSettingAction;

    void Awake()
    {
        OptionBtn.onClick.AddListener(OnClickOption);
        ConfirmBtn.onClick.AddListener(OnClickConfirm);
        CancelBtn.onClick.AddListener(OnClickCancel);
    }

    public virtual void OnClickOption()
    {
        OptionWindow.SetActive(!OptionWindow.activeSelf);
        GameStart.SetActive(!GameStart.activeSelf);
        Option.SetActive(!Option.activeSelf);
        Exit.SetActive(!Exit.activeSelf);
    }

    protected virtual void OnClickConfirm()
    {
        OnClickOption();
        ConfirmSettingAction?.Invoke();
    }

    protected virtual void OnClickCancel()
    {
        OnClickOption();
        // don't save setting changes
        CancelSettingAction?.Invoke();
    }
}
