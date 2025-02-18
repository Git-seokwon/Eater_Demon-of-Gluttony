using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    [SerializeField] private GameObject target; // Almost Player
    [SerializeField] private List<InteractionPrefab> interactions = new(); // Need to Fill
    [SerializeField] private GameObject interactionField; // Onload.
    private PlayerInteractionUI pui;
    private bool checkInteraction = false;
    

    public GameObject Target => target;
    public IReadOnlyList<InteractionPrefab> Interactions => interactions;
    public PlayerInteractionUI PUI => PUI;
    
    private void Awake()
    {
        /*
        RectTransform objscale = obj.GetComponent<RectTransform>();
        obj.transform.SetParent(transform, false);
        float pos = (gameObject.transform.localScale.x) / 2 + (objscale.rect.width) / 2;
        obj.transform.localPosition = new Vector3(pos, objscale.rect.height / 2, 0);
        */
        pui = interactionField.GetComponent<PlayerInteractionUI>();
        pui.Init();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(target))
        {
            checkInteraction = true;
            pui.OpenUI(interactions, checkInteraction);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(target))
        {
            checkInteraction = false;
            pui.CloseUI(checkInteraction);
        }
    }
}
