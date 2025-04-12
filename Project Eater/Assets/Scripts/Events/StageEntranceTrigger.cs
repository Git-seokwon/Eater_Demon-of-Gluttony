using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEntranceTrigger : MonoBehaviour
{
    [HideInInspector] public bool isTrigger;

    [SerializeField] private CinemachineTarget cinemachineTarget;
    [SerializeField] private Transform focusTarget; // 던전 입구
    [SerializeField] private float focusDuration = 2f;
    [SerializeField] private float focusWeight = 3f;
    [SerializeField] private float focusRadius = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag && !isTrigger)
        {
            Show();
        }
    }

    private void Show()
    {
        isTrigger = true;

        GameManager.Instance.player.PlayerMovement.Stop();
        PlayerController.Instance.enabled = false;
        PlayerController.Instance.IsInterActive = true;

        cinemachineTarget.StartFocusSequence(focusTarget, focusDuration, focusWeight, focusRadius);
    }
}
