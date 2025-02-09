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

    // 돌아가기 버튼
    private void Return()
    {
        // 플레이어 스킬 자료구조 전부 Clear
        ClearPlayerSkills();

        // 돌아가기 연출
        GameManager.Instance.StartDisplayStageExitText();
        // 재화 지급
        GameManager.Instance.BaalFlesh = StageManager.Instance.GetBaalFlesh;

        // 스테이지 끄기
        StageManager.Instance.CurrentRoom.gameObject.SetActive(false);
        // FlowField 초기화
        GridController.Instance.ExitStage();
        // Level 및 Level Up 관련 변수 초기화
        GameManager.Instance.FinalizePlayer();
        // Stage Manager 변수 초기화
        StageManager.Instance.ResetVariable();
        GameManager.Instance.player.gameObject.SetActive(true);
        // 캐릭터 로비로 이동
        GameManager.Instance.player.transform.position = StageManager.Instance.ReturnPosition.position;
        // 캐릭터 활성화 
        GameManager.Instance.player.gameObject.SetActive(true);

        Time.timeScale = 1f;
        GameManager.Instance.CinemachineTarget.enabled = true;
        PlayerController.Instance.SetPlayerMode(PlayerMode.Default);
        PlayerController.Instance.enabled = true;

        transform.parent.parent.gameObject.SetActive(false);
        // 스테이지 종료 시 다시 기능성 UI에 접근 가능
        PlayerController.Instance.IsInterActive = false;
    }

    private void ClearPlayerSkills()
        => GameManager.Instance.player.SkillSystem.ReSetPlayerSkills();
}
