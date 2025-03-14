using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : OptionUIBase
{
    [SerializeField]
    private GameObject SystemWindow;
    [SerializeField]
    private GameObject SettingWindow;

    protected override void OnClickConfirm()
    {
        SettingWindow.SetActive(false);
        SystemWindow.SetActive(true);
        ConfirmSettingAction?.Invoke();
    }

    protected override void OnClickCancel()
    {
        SettingWindow.SetActive(false);
        SystemWindow.SetActive(true);
        CancelSettingAction?.Invoke();
    }
}
