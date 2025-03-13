using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BloodFXAction : CustomAction
{
    public override void Start(object data)
    {
        if (data is Effect effect)
            effect.Target.PlayBleedingEffect();
    }

    public override void Release(object data)
    {
        if (data is Effect effect)
            effect.Target.StopBleedingEffect();
    }

    public override object Clone() => new BloodFXAction();
}
