using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Quest/QTask/Target/QString", fileName ="QTarget_")]

public class QStringTarget : QTaskTarget
{
    [SerializeField] private string value;

    public override object Value => value;

    public override bool IsEqual(object target)
    {
        string targetAsString = target as string;
        if (targetAsString == null)
            return false;
        return value == targetAsString;
    }
}