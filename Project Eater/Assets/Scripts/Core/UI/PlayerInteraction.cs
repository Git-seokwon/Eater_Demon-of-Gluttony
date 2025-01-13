using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    GameObject interactionUI; // 상호작용전용 UI
    PlayerInteractionUI interactionUIProperty;

    Dictionary<string, string> actions = new Dictionary<string, string>();

    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            interactionUI.SetActive(true);
            interactionUIProperty = other.gameObject.GetComponent<PlayerInteractionUI>();

            // UI 활성화 및 UI컴포넌트 추가처리
        }
    }
}
