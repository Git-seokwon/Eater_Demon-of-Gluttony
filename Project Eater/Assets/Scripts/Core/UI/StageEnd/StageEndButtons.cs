using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageEndButtons : MonoBehaviour
{
    [SerializeField]
    private Button returnButton;

    private void OnEnable()
    {
        returnButton.onClick.AddListener(Return);
    }

    private void OnDisable()
    {
        returnButton.onClick.RemoveAllListeners();
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
        GameManager.Instance.player.gameObject.SetActive(true);
        // ĳ���� �κ�� �̵�
        GameManager.Instance.player.transform.position = StageManager.Instance.ReturnPosition.position;
        // ĳ���� Ȱ��ȭ 
        GameManager.Instance.player.gameObject.SetActive(true);

        Time.timeScale = 1f;
        GameManager.Instance.CinemachineTarget.enabled = true;
        PlayerController.Instance.SetPlayerMode(PlayerMode.Default);
        PlayerController.Instance.enabled = true;

        transform.parent.parent.gameObject.SetActive(false);
        // �������� ���� �� �ٽ� ��ɼ� UI�� ���� ����
        PlayerController.Instance.IsInterActive = false;
    }

    private void ClearPlayerSkills()
        => GameManager.Instance.player.SkillSystem.ReSetPlayerSkills();
}
