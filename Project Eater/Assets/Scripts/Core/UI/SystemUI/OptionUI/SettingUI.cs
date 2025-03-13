using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : LobbyOptionUI
{
    [SerializeField]
    private GameObject SystemWindow;
    [SerializeField]
    private GameObject SettingWindow;

    [SerializeField]
    private Button ConfirmBtn;
    [SerializeField]
    private Button CancelBtn;

    protected override void OnClickConfirm()
    {
        SettingWindow.SetActive(false);
        SystemWindow.SetActive(true);
    }

    protected override void OnClickCancel()
    {
        SettingWindow.SetActive(false);
        SystemWindow.SetActive(true);
        // don't save setting changes

    }
}
