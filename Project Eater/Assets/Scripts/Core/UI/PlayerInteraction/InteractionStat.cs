using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="InteractionStat", menuName ="PlayerInteraction/Dogam")]
public class InteractionStat : InteractionPrefab
{
    private DogamUI dogamui;
    public override void DoAction()
    {
        dogamui = GameObject.Find("UI").GetComponentInChildren<DogamUI>(true);
        dogamui.Open();
    }
}
