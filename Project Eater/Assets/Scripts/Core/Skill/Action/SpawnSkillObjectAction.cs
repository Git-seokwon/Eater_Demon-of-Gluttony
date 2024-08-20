using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnSkillObjectAction : SkillAction
{
    [SerializeField]
    private GameObject skillObjectPrefab;

    [Header("Target Searcher")]
    [SerializeField]
    private TargetSearcher targetSearcherForSkillObejct;

    // [Header("Data")]


    public override void Apply(Skill skill)
    {
        throw new System.NotImplementedException();
    }

    public override object Clone()
    {
        throw new System.NotImplementedException();
    }

}
