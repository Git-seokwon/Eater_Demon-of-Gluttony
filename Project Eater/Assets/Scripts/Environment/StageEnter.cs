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

    public void InterActive()
    {
        // �������� ���� UI ���� 
        showUI.SetActive(true);
        // �÷��̾� �Է� ���� 
        PlayerController.Instance.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            showInteractiveKey.SetActive(true);
            if (Input.GetKeyUp(keyCode))
            {
                showInteractiveKey.SetActive(false);
                InterActive();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            showInteractiveKey.SetActive(false);
        }
    }
}
