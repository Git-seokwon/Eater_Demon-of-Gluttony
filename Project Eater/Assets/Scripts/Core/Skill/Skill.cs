using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : IdentifiedObject
{
    // 스킬 소유 Entity
    public Entity Owner { get; private set; }

    // 현재 Level에 대응되는 SkillData
    private SkillData currentData;

    public TargetSearcher TargetSearcher => currentData.targetSearcher;
    public TargetSelectionResult TargetSelectionResult => TargetSearcher.SelectionResult;
}
