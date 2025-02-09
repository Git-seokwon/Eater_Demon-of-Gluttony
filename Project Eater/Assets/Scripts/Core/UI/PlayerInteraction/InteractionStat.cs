using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionStat", menuName = "PlayerInteraction/Stat")]
public class InteractionStat : InteractionPrefab
{
    public override void DoAction()
    {
        GameObject.Find("Stat_Upgrade").gameObject.SetActive(true);
    }

}
