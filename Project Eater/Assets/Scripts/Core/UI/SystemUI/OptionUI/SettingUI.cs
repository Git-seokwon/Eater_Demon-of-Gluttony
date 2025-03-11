using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField]
    private GameObject SystemWindow;
    [SerializeField]
    private GameObject SettingWindow;

    [SerializeField]
    private GameObject SoundSettingScreen;
    [SerializeField]
    private GameObject GraphicSettingScreen;
    [SerializeField]
    private GameObject GameplaySettingScreen;

    [SerializeField]
    private Button SoundBtn;
    [SerializeField]
    private Button GraphicBtn;
    [SerializeField]
    private Button GameplayBtn;
    [SerializeField]
    private Button ConfirmBtn;
    [SerializeField]
    private Button CancelBtn;

    void Awake()
    {
        SoundBtn.onClick.AddListener(OnClickSoundSetting);
        GraphicBtn.onClick.AddListener(OnClickGraphicSetting);
        GameplayBtn.onClick.AddListener(OnClickGameplaySetting);
        ConfirmBtn.onClick.AddListener(OnClickConfirm);
        CancelBtn.onClick.AddListener(OnClickCancel);
    }

    private void OnClickSoundSetting()
    {
        GraphicSettingScreen.SetActive(false);
        GameplaySettingScreen.SetActive(false);
        SoundSettingScreen.SetActive(true);
    }

    private void OnClickGraphicSetting()
    {
        GameplaySettingScreen.SetActive(false);
        SoundSettingScreen.SetActive(false);
        GraphicSettingScreen.SetActive(true);
    }

    private void OnClickGameplaySetting()
    {
        GraphicSettingScreen.SetActive(false);
        SoundSettingScreen.SetActive(false);
        GameplaySettingScreen.SetActive(true);
    }

    private void OnClickConfirm()
    {
        SettingWindow.SetActive(false);
        SystemWindow.SetActive(true);
    }

    private void OnClickCancel()
    {
        SettingWindow.SetActive(false);
        SystemWindow.SetActive(true);
        // don't save setting changes
        CancelSettingChanges();
    }
    private void CancelSettingChanges()
    {

    }
}
