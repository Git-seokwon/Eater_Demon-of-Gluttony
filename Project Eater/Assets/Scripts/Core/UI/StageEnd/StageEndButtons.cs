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

    // 다시 시작 버튼
    private void ReStart()
    {
        ClearPlayerSkills();

        // 화면 Fade In/Out
        GameManager.Instance.StartDisplayStageNameText();

        // 플레이어 위치 이동
        var spawnPosition = StageManager.Instance.CurrentStage.PlayerSpawnPosition;
        GameManager.Instance.player.transform.position = spawnPosition;

        // Flow Field는 이전에 만들어 둔 거를 계속 사용한다
        // 플레이어 레벨 초기화
        GameManager.Instance.InitializePlayer();
        // Stage Manager 변수 초기화
        StageManager.Instance.ResetVariable(true);

        Time.timeScale = 1f;
        PlayerController.Instance.enabled = true;

        transform.parent.parent.gameObject.SetActive(false);
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
        // 캐릭터 로비로 이동
        GameManager.Instance.player.transform.position = StageManager.Instance.ReturnPosition.position;

        Time.timeScale = 1f;
        PlayerController.Instance.SetPlayerMode(PlayerMode.Default);
        PlayerController.Instance.enabled = true;

        transform.parent.parent.gameObject.SetActive(false);
    }

    private void ClearPlayerSkills()
        => GameManager.Instance.player.SkillSystem.ReSetPlayerSkills();
}
