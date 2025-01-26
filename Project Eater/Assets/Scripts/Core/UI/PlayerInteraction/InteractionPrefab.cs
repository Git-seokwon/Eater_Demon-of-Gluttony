using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionPrefab : ScriptableObject
{
    [SerializeField] string codeName;
    public string CodeName => codeName;
    public abstract void DoAction();
}
