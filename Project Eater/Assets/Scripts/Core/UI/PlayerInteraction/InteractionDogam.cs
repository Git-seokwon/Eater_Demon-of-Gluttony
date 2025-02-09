using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="InteractionDogam", menuName ="PlayerInteraction/Dogam")]
public class InteractionDogam : InteractionPrefab
{
    private DogamUI dogamui;
    public override void DoAction()
    {
        dogamui = GameObject.Find("UI").GetComponentInChildren<DogamUI>(true);
        dogamui.Open();
    }
}
