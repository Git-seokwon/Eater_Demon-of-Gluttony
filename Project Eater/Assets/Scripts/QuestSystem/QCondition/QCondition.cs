using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QCondition : ScriptableObject
{
    [SerializeField] private string description;

    public abstract bool IsPass(Quest quest);
}