using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEntranceTrigger : MonoBehaviour
{
    [HideInInspector] public bool[] isTriggers = new bool[2];
    [HideInInspector] public int eventIndex;

    [SerializeField] private CinemachineTarget cinemachineTarget;
    [SerializeField] private Transform focusTarget; // 던전 입구
    [SerializeField] private float focusDuration = 2f;
    [SerializeField] private float focusWeight = 3f;
    [SerializeField] private float focusRadius = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag && !isTriggers[eventIndex])
        {
            ShowEvent();
        }
    }

    private void ShowEvent()
    {
        isTriggers[eventIndex] = true;

        switch (eventIndex)
        {
            case 0:
                ShowEntraceEvent();
                break;

            case 1:
                StartCoroutine(ShowLatentSkillEvent());
                break;

            default:
                break;
        }
    }

    private void ShowEntraceEvent()
    {
        GameManager.Instance.player.PlayerMovement.Stop();
        PlayerController.Instance.enabled = false;
        PlayerController.Instance.IsInterActive = true;

        cinemachineTarget.StartFocusSequence(focusTarget, focusDuration, focusWeight, focusRadius);
    }

    private IEnumerator ShowLatentSkillEvent()
    {
        GameManager.Instance.player.PlayerMovement.Stop();
        GameManager.Instance.CinemachineTarget.enabled = false;
        PlayerController.Instance.enabled = false;
        PlayerController.Instance.IsInterActive = true;

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(2, DialogCharacter.EVENTS));
        DialogManager.Instance.DeActivate();

        GameManager.Instance.CinemachineTarget.enabled = true;
        PlayerController.Instance.enabled = true;
        PlayerController.Instance.IsInterActive = false;
    }
}
