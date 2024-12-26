using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageEndButtons : MonoBehaviour
{
    [SerializeField]
    private Button restartButton;
    [SerializeField]
    private Button returnButton;

    private void OnEnable()
    {
        restartButton.onClick.AddListener(ReStart);
        returnButton.onClick.AddListener(Return);
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveAllListeners();
        returnButton.onClick.RemoveAllListeners();
    }

    // �ٽ� ���� ��ư
    private void ReStart()
    {
        ClearPlayerSkills();

        // ȭ�� Fade In/Out
        GameManager.Instance.StartDisplayStageNameText();

        // �÷��̾� ��ġ �̵�
        var spawnPosition = StageManager.Instance.CurrentStage.PlayerSpawnPosition;
        GameManager.Instance.player.transform.position = spawnPosition;

        // Flow Field�� ������ ����� �� �Ÿ� ��� ����Ѵ�
        // �÷��̾� ���� �ʱ�ȭ
        GameManager.Instance.InitializePlayer();
        // Stage Manager ���� �ʱ�ȭ
        StageManager.Instance.ResetVariable(true);

        Time.timeScale = 1f;
        PlayerController.Instance.enabled = true;

        transform.parent.parent.gameObject.SetActive(false);
    }

    // ���ư��� ��ư
    private void Return()
    {
        // �÷��̾� ��ų �ڷᱸ�� ���� Clear
        ClearPlayerSkills();

        // ���ư��� ����
        GameManager.Instance.StartDisplayStageExitText();
        // ��ȭ ����
        GameManager.Instance.BaalFlesh = StageManager.Instance.GetBaalFlesh;

        // �������� ����
        StageManager.Instance.CurrentRoom.gameObject.SetActive(false);
        // FlowField �ʱ�ȭ
        GridController.Instance.ExitStage();
        // Level �� Level Up ���� ���� �ʱ�ȭ
        GameManager.Instance.FinalizePlayer();
        // Stage Manager ���� �ʱ�ȭ
        StageManager.Instance.ResetVariable();
        // ĳ���� �κ�� �̵�
        GameManager.Instance.player.transform.position = StageManager.Instance.ReturnPosition.position;

        Time.timeScale = 1f;
        PlayerController.Instance.SetPlayerMode(PlayerMode.Default);
        PlayerController.Instance.enabled = true;

        transform.parent.parent.gameObject.SetActive(false);
    }

    private void ClearPlayerSkills()
        => GameManager.Instance.player.SkillSystem.ReSetPlayerSkills();
}
