using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class OptionUIBase : MonoBehaviour
{
    [SerializeField]
    protected GameObject OptionWindow;

    [SerializeField]
    private Button ConfirmBtn;
    [SerializeField]
    private Button CancelBtn;
    [SerializeField]
    private Button InitializeBtn;
    [SerializeField]
    private Scrollbar SettingScrollbar;

    // 이벤트 만들기
    public Action ConfirmSettingAction;
    public Action CancelSettingAction;
    public Action InitializeSettingAction;

    protected virtual void Awake()
    {
        ConfirmBtn.onClick.AddListener(OnClickConfirm);
        CancelBtn.onClick.AddListener(OnClickCancel);
        InitializeBtn.onClick.AddListener(OnClickInitialize);
    }

    public virtual void OnClickOption()
    {
        OptionWindow.SetActive(!OptionWindow.activeSelf);
    }

    protected virtual void OnClickConfirm()
    {
        OnClickOption();
        ConfirmSettingAction?.Invoke();
    }

    protected virtual void OnClickCancel()
    {
        OnClickOption();
        CancelSettingAction?.Invoke();
    }

    protected virtual void OnClickInitialize()
    {
        InitializeSettingAction?.Invoke();
        SettingScrollbar.value = 1;
    }
}
