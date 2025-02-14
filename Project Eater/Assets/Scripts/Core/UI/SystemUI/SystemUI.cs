using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemUI : MonoBehaviour
{
    [SerializeField]
    private GameObject SystemWindow;
    [SerializeField]
    private GameObject SettingWindow;

    [SerializeField]
    private Button SystemOpenBtn;
    [SerializeField]
    private Button SettingBtn;
    [SerializeField]
    private Button LobyBtn;
    [SerializeField]
    private Button ResumeBtn;
    [SerializeField]
    private Button ExitBtn;

    void Awake()
    {
        SystemOpenBtn.onClick.AddListener(OnSystemOpen);
        SettingBtn.onClick.AddListener(OnClickSetting);
        LobyBtn.onClick.AddListener(OnClickBackLoby);
        ResumeBtn.onClick.AddListener(OnClickResume);
        ExitBtn.onClick.AddListener(OnClickExitGame);
    }

    private void OnSystemOpen()
    {
        SystemWindow.SetActive(true);
    }

    private void OnClickSetting()
    {
        SystemWindow.SetActive(false);
        SettingWindow.SetActive(true);
    }

    private void OnClickBackLoby()
    {
        SystemWindow.SetActive(false);
        // back to loby
    }

    private void OnClickResume()
    {
        SystemWindow.SetActive(false);
    }

    private void OnClickExitGame()
    {
        SystemWindow.SetActive(false);
        // exit game
    }
}
