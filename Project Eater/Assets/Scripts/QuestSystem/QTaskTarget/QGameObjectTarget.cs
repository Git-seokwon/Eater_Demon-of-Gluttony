using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

[CreateAssetMenu(menuName ="Quest/QTask/Target/QGameObject", fileName = "QTarget_")]
public class QGameObjectTarget : QTaskTarget
{
    [SerializeField] private GameObject value;

    public override object Value => value;

    public override bool IsEqual(object target)
    {
        var targetAsGameObject = target as GameObject;
        if(targetAsGameObject == null)
            return false;
        return targetAsGameObject.name.Contains(value.name);
    }
}
