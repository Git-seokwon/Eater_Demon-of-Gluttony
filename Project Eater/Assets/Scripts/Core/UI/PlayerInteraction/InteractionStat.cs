using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="InteractionStat", menuName ="PlayerInteraction")]
public class InteractionStat : InteractionPrefab
{
    DogamUI dogamui;
    public override void DoAction()
    {
        DogamUI.Instance.Open();
    }
}
