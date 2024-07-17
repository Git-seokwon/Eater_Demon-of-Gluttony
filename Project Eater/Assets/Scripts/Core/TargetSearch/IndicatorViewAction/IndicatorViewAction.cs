using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class IndicatorViewAction : ICloneable
{
    // ※ requestObject : targetsearcher에게 Indicator를 보여달라고 요청한 Object
    public abstract void ShowIndicator(TargetSearcher targetSearcher, GameObject requestObject,
        float range, float angle, float fillAmount);

    public abstract void HideIndicator();

    public abstract void SetFillAmount(float amount);
    public abstract object Clone();
}
