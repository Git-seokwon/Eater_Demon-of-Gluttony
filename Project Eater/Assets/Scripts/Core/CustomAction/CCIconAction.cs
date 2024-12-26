using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CCType
{
    Weak,
    Stun,
    Slow
}

[System.Serializable]
public class CCIconAction : CustomAction
{
    [SerializeField]
    private CCType type;

    private Entity entity;

    public override void Start(object data)
    {
        if (data is Effect)
            Show(data as Effect);
    }

    public override void Release(object data) => entity?.GetComponent<FloatingIcon>().SetDeActiveCCSprite((int)type);

    private void Show(Effect effect)
    {
        entity = effect.Target;
        var ccIcon = entity?.GetComponent<FloatingIcon>();
        if (ccIcon != null)
            ccIcon.SetActiveCCSprite(((int)type));
    }

    public override object Clone()
    {
        return new CCIconAction()
        {
            type = type
        };
    }
}
