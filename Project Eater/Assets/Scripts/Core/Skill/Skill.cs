using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : IdentifiedObject
{
    // ��ų ���� Entity
    public Entity Owner { get; private set; }

    // ���� Level�� �����Ǵ� SkillData
    private SkillData currentData;

    public TargetSearcher TargetSearcher => currentData.targetSearcher;
    public TargetSelectionResult TargetSelectionResult => TargetSearcher.SelectionResult;
}
