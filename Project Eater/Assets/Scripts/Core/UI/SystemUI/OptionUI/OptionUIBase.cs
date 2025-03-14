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

    // �̺�Ʈ �����
    public Action ConfirmSettingAction;
    public Action CancelSettingAction;

    void Awake()
    {
        ConfirmBtn.onClick.AddListener(OnClickConfirm);
        CancelBtn.onClick.AddListener(OnClickCancel);
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
}
