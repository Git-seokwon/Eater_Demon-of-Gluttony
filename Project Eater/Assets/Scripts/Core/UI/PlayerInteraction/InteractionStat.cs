using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="InteractionStat", menuName ="PlayerInteraction/Stat")]
public class InteractionStat : InteractionPrefab
{
    DogamUI dogamui;
    public override void DoAction()
    {
        DogamUI.Instance.Open();
    }
}
