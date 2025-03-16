using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : OptionUIBase
{
    [SerializeField]
    private GameObject SystemWindow;

    public override void OnClickOption()
    {
        OptionWindow.SetActive(!OptionWindow.activeSelf);
        SystemWindow.SetActive(!SystemWindow.activeSelf);
    }

    protected override void OnClickConfirm()
    {
        OnClickOption();
        ConfirmSettingAction?.Invoke();
    }

    protected override void OnClickCancel()
    {
        OnClickOption();
        CancelSettingAction?.Invoke();
    }
}
