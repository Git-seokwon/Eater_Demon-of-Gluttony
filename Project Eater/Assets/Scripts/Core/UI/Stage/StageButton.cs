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
        var spawnPosition = StageManager.Instance.CurrentStage.PlayerSpawnPosition;
        GameManager.Instance.player.transform.position = spawnPosition;

        // ȭ�� Fade In/Out
        GameManager.Instance.StartDisplayStageNameText();
        // �������� On
        StageManager.Instance.CurrentRoom.gameObject.SetActive(true);
        // �������� ���� ó�� 

        transform.parent.parent.gameObject.SetActive(false);
    }

    private void CancelDungeon()
    {
        PlayerController.Instance.enabled = true;
        transform.parent.parent.gameObject.SetActive(false);
    }
}
