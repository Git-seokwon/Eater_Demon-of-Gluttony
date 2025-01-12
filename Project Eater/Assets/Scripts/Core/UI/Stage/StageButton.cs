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

        // �÷��̾� ���� ��ġ �̵� 
        var spawnPosition = StageManager.Instance.CurrentStage.PlayerSpawnPosition;
        player.transform.position = spawnPosition;

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
        // �������� ���� ��, PlayerController.Instance.IsInterActive = true;�� ���� �Ǹ鼭 ���� �߿��� �ɼ��� ������
        // ������ ��ɼ� UI(���� ��ȭ ��)�� ������ �� ����. 
    }

    private void CancelDungeon()
    {
        PlayerController.Instance.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        transform.parent.parent.gameObject.SetActive(false);
    }

    private void SetFlowField() => GridController.Instance.EnterStage();
}
