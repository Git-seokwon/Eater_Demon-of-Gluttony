using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTest : MonoBehaviour
{
    [SerializeField] private List<InteractionPrefab> interPre;

    private PlayerInteraction pI;
    private PlayerInteractionUI pui;
    
    private void Awake()
    {
        pI = GetComponent<PlayerInteraction>();
        pI.PuiOnload += GetPUI;
    }

    void GetPUI(PlayerInteractionUI value)
    {
        pui = value;

        foreach(var a in interPre)
        {
            pui.AddAction(a);
        }
    }
}
