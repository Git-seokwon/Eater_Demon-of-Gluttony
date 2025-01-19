using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEnter : MonoBehaviour, IInteractive
{
    [SerializeField]
    private GameObject showUI;
    [SerializeField]
    private GameObject showInteractiveKey; 
    [field : SerializeField]
    public KeyCode keyCode { get; set; }
    private bool isPlayerInTrigger = false;

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(keyCode) && !PlayerController.Instance.IsInterActive)
        {
            showInteractiveKey.SetActive(false);
            InterActive();
        }
    }

    public void InterActive()
    {
        // �������� ���� UI ���� 
        showUI.SetActive(true);
        // �÷��̾� �Է� ���� 
        GameManager.Instance.player.rigidbody.velocity = Vector3.zero;
        PlayerController.Instance.enabled = false;
        PlayerController.Instance.IsInterActive = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            showInteractiveKey.SetActive(true);
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            showInteractiveKey.SetActive(false);
            isPlayerInTrigger = false;
            PlayerController.Instance.IsInterActive = false;
        }
    }
}
