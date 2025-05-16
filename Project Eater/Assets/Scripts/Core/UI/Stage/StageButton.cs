using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    [SerializeField]
    private Button enterButton;
    [SerializeField]
    private Button cancelButton;
    [SerializeField]
    private GameObject battleUI;

    private void OnEnable()
    {
        PlayerController.Instance.enabled = false;
        GameManager.Instance.CinemachineTarget.enabled = false;

        enterButton.onClick.AddListener(EnterDungeon);
        cancelButton.onClick.AddListener(CancelDungeon);
    }

    private void OnDisable()
    {
        PlayerController.Instance.enabled = true;
        GameManager.Instance.CinemachineTarget.enabled = true;

        if (enterButton != null)
        {
            enterButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
        }
    }

    private void EnterDungeon()
    {
        // 스테이지 입장 효과음 재생
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.stageIn);

        var player = GameManager.Instance.player;

        // 플레이어 스킬 획득 리스트 갱신 
        var skills
            = player.SkillSystem.SkillSlot.Where(pair => pair.Key.Item1 == 0 &&
                                                 pair.Value.IsInherent || pair.Value.IsDevoured)
                                          .Select(pair => pair.Value).ToList();
        foreach (var skill in skills)
            player.SkillSystem.AddAcquirableSkills(skill);

        // 플레이어 던전 위치 이동 
        var spawnPosition = StageManager.Instance.CurrentStage.PlayerSpawnPosition;
        player.transform.position = spawnPosition;

        // 만약 잔여 고기가 있다면 비활성화 해주기 
        StageManager.Instance.ClearFieldItems();
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

        // Battle UI 켜기 
        battleUI.gameObject.SetActive(true);
        // PlayerHUD 연동
        if (PlayerHUD.Instance != null)
            PlayerHUD.Instance.Show();
        // 해방 스킬 아이콘 띄우기 
        battleUI.GetComponentInChildren<LatentSkillSlot>().Skill = player.SkillSystem.LatentSkill;

        // 시네머신 바운더리 설정 
        StageManager.Instance.SetCameraBounds();

        // 웨이브 시작
        StageManager.Instance.StartWave();
        // 전투 bgm 재생
        MusicManager.Instance.PlayMusic(GameResources.Instance.battleMusic);
    }

    private void CancelDungeon()
    {
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.equipSkill);

        PlayerController.Instance.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        transform.parent.parent.gameObject.SetActive(false);
    }

    private void SetFlowField() => GridController.Instance.EnterStage();
}
