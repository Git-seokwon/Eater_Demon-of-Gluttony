using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    GameObject interactionUI; // ��ȣ�ۿ����� UI
    PlayerInteractionUI interactionUIProperty;

    Dictionary<string, string> actions = new Dictionary<string, string>();

    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            interactionUI.SetActive(true);
            interactionUIProperty = other.gameObject.GetComponent<PlayerInteractionUI>();

            // UI Ȱ��ȭ �� UI������Ʈ �߰�ó��
        }
    }
}
