using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/QCondition/QAffinity", fileName = "QCondition_")]
public class QAffinityCondition : QCondition
{
    public override bool IsPass(Quest quest)
    {
        if(GameManager.Instance.sigma.affinity == 4)
        {
            return true;
        }
        return false;
    }
}
