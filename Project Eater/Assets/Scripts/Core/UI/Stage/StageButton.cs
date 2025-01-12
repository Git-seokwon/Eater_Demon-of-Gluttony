using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    [SerializeField]
    private Button enterButton;
    [SerializeField]
    private Button cancelButton;

    private void OnEnable()
    {
        enterButton.onClick.AddListener(EnterDungeon);
        cancelButton.onClick.AddListener(CancelDungeon);
    }

    private void OnDisable()
    {
        if (enterButton != null)
        {
            enterButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
        }
    }

    private void EnterDungeon()
    {
        var player = GameManager.Instance.player;

        // 플레이어 던전 위치 이동 
        var spawnPosition = StageManager.Instance.CurrentStage.PlayerSpawnPosition;
        player.transform.position = spawnPosition;

        // 화면 Fade In/Out
        GameManager.Instance.StartDisplayStageNameText();
        // 스테이지 On
        StageManager.Instance.CurrentRoom.gameObject.SetActive(true);
        // 스테이지 시작 처리  
        // Flow Field 생성 
        Invoke("SetFlowField", 2f);
        // 플레이어 Level 세팅 
        GameManager.Instance.InitializePlayer();
        // 플레이어 모드 변경
        PlayerController.Instance.SetPlayerMode(PlayerMode.Devil);
        // 현재 장착 중인 해방 스킬 활성화 
        player.SkillSystem.SetupLatentSkills(player.CurrentLatentSkill.Level);

        // 던전 입장 UI 끄기 
        transform.parent.parent.gameObject.SetActive(false);
        // 스테이지 입장 시, PlayerController.Instance.IsInterActive = true;가 유지 되면서 전투 중에는 옵션을 제외한
        // 나머지 기능성 UI(스텟 강화 등)에 접근할 수 없다. 
    }

    private void CancelDungeon()
    {
        PlayerController.Instance.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        transform.parent.parent.gameObject.SetActive(false);
    }

    private void SetFlowField() => GridController.Instance.EnterStage();
}
