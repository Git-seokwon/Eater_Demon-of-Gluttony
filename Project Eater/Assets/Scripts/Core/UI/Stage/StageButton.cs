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
        // �������� ���� ȿ���� ���
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.stageIn);

        var player = GameManager.Instance.player;

        // �÷��̾� ��ų ȹ�� ����Ʈ ���� 
        var skills
            = player.SkillSystem.SkillSlot.Where(pair => pair.Key.Item1 == 0 &&
                                                 pair.Value.IsInherent || pair.Value.IsDevoured)
                                          .Select(pair => pair.Value).ToList();
        foreach (var skill in skills)
            player.SkillSystem.AddAcquirableSkills(skill);

        // �÷��̾� ���� ��ġ �̵� 
        var spawnPosition = StageManager.Instance.CurrentStage.PlayerSpawnPosition;
        player.transform.position = spawnPosition;

        // ���� �ܿ� ��Ⱑ �ִٸ� ��Ȱ��ȭ ���ֱ� 
        StageManager.Instance.ClearFieldItems();
        // ȭ�� Fade In/Out
        GameManager.Instance.StartDisplayStageNameText();
        // �������� On
        StageManager.Instance.CurrentRoom.gameObject.SetActive(true);
        // �������� ���� ó��
        // Flow Field ���� 
        Invoke("SetFlowField", 2f);
        // �÷��̾� Level ���� 
        GameManager.Instance.InitializePlayer();
        // �÷��̾� ��� ����
        PlayerController.Instance.SetPlayerMode(PlayerMode.Devil);
        // ���� ���� ���� �ع� ��ų Ȱ��ȭ 
        player.SkillSystem.SetupLatentSkills(player.CurrentLatentSkill.Level);

        // ���� ���� UI ���� 
        transform.parent.parent.gameObject.SetActive(false);

        // Battle UI �ѱ� 
        battleUI.gameObject.SetActive(true);
        // PlayerHUD ����
        if (PlayerHUD.Instance != null)
            PlayerHUD.Instance.Show();
        // �ع� ��ų ������ ���� 
        battleUI.GetComponentInChildren<LatentSkillSlot>().Skill = player.SkillSystem.LatentSkill;

        // �ó׸ӽ� �ٿ���� ���� 
        StageManager.Instance.SetCameraBounds();

        // ���̺� ����
        StageManager.Instance.StartWave();
        // ���� bgm ���
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
