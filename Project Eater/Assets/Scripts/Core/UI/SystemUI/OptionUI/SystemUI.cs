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
    private Button LobbyBtn;
    [SerializeField]
    private Button ResumeBtn;
    [SerializeField]
    private Button ExitBtn;

    void Awake()
    {
        SystemOpenBtn.onClick.AddListener(OnSystemOpen);
        SettingBtn.onClick.AddListener(OnClickSetting);
        LobbyBtn.onClick.AddListener(OnClickBackLobby);
        ResumeBtn.onClick.AddListener(OnClickResume);
        ExitBtn.onClick.AddListener(OnClickExitGame);
    }

    private void OnSystemOpen()
    {
        SystemWindow.SetActive(true);

        Time.timeScale = 0f;
        PlayerController.Instance.enabled = false;
        GameManager.Instance.CinemachineTarget.enabled = false;

        // 전투 중이면 LobbyBtn 버튼 활성화 
        if (StageManager.Instance.isCombat)
            LobbyBtn.interactable = true;
        else
            LobbyBtn.interactable = false;
    }

    private void OnClickSetting()
    {
        SystemWindow.SetActive(false);
        SettingWindow.SetActive(true);
    }

    private void OnClickBackLobby()
    {
        Time.timeScale = 1f;
        PlayerController.Instance.enabled = true;
        GameManager.Instance.CinemachineTarget.enabled = true;

        SystemWindow.SetActive(false);
        StageManager.Instance.OnDefeatStage();
    }

    private void OnClickResume()
    {
        SystemWindow.SetActive(false);
        Time.timeScale = 1f;
        PlayerController.Instance.enabled = true;
        GameManager.Instance.CinemachineTarget.enabled = true;
    }

    private void OnClickExitGame()
    {
        Application.Quit();
    }
}
