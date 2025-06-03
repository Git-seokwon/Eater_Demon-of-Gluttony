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
        SettingBtn.onClick.AddListener(OnClickSetting);
        LobbyBtn.onClick.AddListener(OnClickBackLobby);
        ResumeBtn.onClick.AddListener(OnClickResume);
        ExitBtn.onClick.AddListener(OnClickExitGame);
    }

    private void Update()
    {
        if (GameManager.Instance.CinemachineTarget.enabled == true && !GameManager.Instance.IsEntering 
            && Input.GetKeyDown(KeyCode.Escape))
        {
            OnSystemOpen();
        }
    }

    private void OnSystemOpen()
    {
        SystemWindow.SetActive(true);

        ChangePlayerSetting(false);

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
        CursorManager.Instance.ChangeCursor(0);
    }

    private void OnClickBackLobby()
    {
        ChangePlayerSetting(true);

        SystemWindow.SetActive(false);
        StageManager.Instance.OnDefeatStage();
        CursorManager.Instance.ChangeCursor(0);
    }

    private void OnClickResume()
    {
        SystemWindow.SetActive(false);
        ChangePlayerSetting(true);
        CursorManager.Instance.ChangeCursor(0);
    }

    private void OnClickExitGame()
    {
        Application.Quit();
    }

    private void ChangePlayerSetting(bool open)
    {
        Time.timeScale = GameManager.Instance.player.Animator.speed =
            GameManager.Instance.player.EffectAnimation.EffectAnimator.speed = (open) ? 1f : 0f;

        PlayerController.Instance.enabled = open;
        GameManager.Instance.CinemachineTarget.enabled = open;
        CursorManager.Instance.ChangeCursor(0);
    }
}
