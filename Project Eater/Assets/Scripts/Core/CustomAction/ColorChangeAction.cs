using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorChangeAction : CustomAction
{
    private enum MethodType { Start, Run, Release }

    [SerializeField]
    private MethodType methodType;
    [SerializeField]
    private Color color;

    public override void Start(object data)
    {
        if (methodType == MethodType.Start)
            (data as Skill).Owner.Sprite.color = color;
    }

    public override void Run(object data)
    {
        if (methodType == MethodType.Run)
            (data as Skill).Owner.Sprite.color = color;
    }

    public override void Release(object data)
    {
        if (methodType == MethodType.Release)
            (data as Skill).Owner.Sprite.color = color;
    }

    public override object Clone()
    {
        return new ColorChangeAction()
        {
            methodType = methodType,
            color = color
        };
    }
}
