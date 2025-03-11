using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyOptionUI : MonoBehaviour
{
    [SerializeField]
    private Button OptionOpenBtn;
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
        OptionOpenBtn.onClick.AddListener(OnOpenOptionWindow);
        SoundBtn.onClick.AddListener(OnClickSoundSetting);
        GraphicBtn.onClick.AddListener(OnClickGraphicSetting);
        GameplayBtn.onClick.AddListener(OnClickGameplaySetting);
        ConfirmBtn.onClick.AddListener(OnClickConfirm);
        CancelBtn.onClick.AddListener(OnClickCancel);
    }

    private void OnOpenOptionWindow()
    {
        SettingWindow.SetActive(true);
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
    }

    private void OnClickCancel()
    {
        SettingWindow.SetActive(false);
        // don't save setting changes
        CancelSettingChanges();
    }
    private void CancelSettingChanges()
    {

    }
}
