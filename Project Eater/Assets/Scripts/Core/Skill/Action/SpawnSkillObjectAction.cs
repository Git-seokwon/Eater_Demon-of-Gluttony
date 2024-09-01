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

    [Header("Data")]
    [SerializeField]
    private float duration;
    [SerializeField]
    private int applyCount;
    [SerializeField]
    private Vector2 objectScale = Vector2.one;

    // Skill의 TargetPositions에 SkillObject를 Spawn하고 Setup 한다. 
    public override void Apply(Skill skill)
    {
        foreach (var targetPosition in skill.TargetPositions)
        {
            var poolObject = PoolManager.Instance.ReuseGameObject(skillObjectPrefab, targetPosition, Quaternion.identity);
            var skillObject = poolObject.GetComponent<SkillObject>();
            if (skillObject)
                skillObject.SetUp(skill, targetSearcherForSkillObejct, duration, applyCount, objectScale);
        }
    }

    public override object Clone()
    {
        return new SpawnSkillObjectAction()
        {
            applyCount = applyCount,
            duration = duration,
            objectScale = objectScale,
            skillObjectPrefab = skillObjectPrefab,
            targetSearcherForSkillObejct = targetSearcherForSkillObejct
        };
    }

    protected override IReadOnlyDictionary<string, string> GetStringByKeyword()
    {
        var applyCycle = skillObjectPrefab.GetComponent<SkillObject>()?.CalculateApplyCycle(duration, applyCount).ToString("0.##");

        var dictionary = new Dictionary<string, string>()
        {
            { "duration", duration.ToString("0.##") },
            { "applyCount", applyCount.ToString("0.##") },
            { "applyCountPerSec", (applyCount / duration).ToString() },
            { "applyCycle", applyCycle }
        };

        return dictionary;
    }

    public override string BuildDescription(string description)
    {
        description = base.BuildDescription(description);
        description = targetSearcherForSkillObejct.BuildDescription(description, "skillAction");

        return description;
    }
}
