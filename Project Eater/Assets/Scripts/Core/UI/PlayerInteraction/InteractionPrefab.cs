using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionPrefab : ScriptableObject
{
    [SerializeField] protected string codeName;
    [SerializeField] protected bool condition; // check if meet the register condition
    public string CodeName => codeName;
    public abstract void DoAction();
    public abstract void ConditionCheck();
}
