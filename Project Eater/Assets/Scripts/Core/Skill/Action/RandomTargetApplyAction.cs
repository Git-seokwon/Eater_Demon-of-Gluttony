using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomTargetApplyAction : SkillAction
{
    [SerializeField]
    private int targetCount;

    private List<int> targetIndex = new List<int>();

    public override void Apply(Skill skill)
    {
        if (skill.Targets.Count <= 0)
            return;

        // 이전 Index
        // → 중복된 Entity를 방지하기 위함
        int prevIndex = -1;

        for (int i = 0; i < targetCount; i++)
        {
            int currentIndex = Random.Range(0, skill.Targets.Count);

            // Target Index 저장하기 
            if (currentIndex != prevIndex)
                targetIndex.Add(currentIndex);
            // 중복 됐으면 다시 뽑기 
            else
                i--;

            prevIndex = currentIndex;
        }

        // 저장해둔 Target Index를 순회하면서 스킬 적용하기 
        foreach (var target in targetIndex)
            skill.Targets[target].SkillSystem.Apply(skill);
    }

    public override void Release(Skill skill)
    {
        targetIndex.Clear();
    }

    protected override IReadOnlyDictionary<string, string> GetStringByKeyword()
    {
        var descriptionValueByKeyword = new Dictionary<string, string>()
        {
            ["targetCount"] = targetCount.ToString(),
        };

        return descriptionValueByKeyword;
    }

    public override object Clone()
    {
        return new RandomTargetApplyAction()
        {
            targetCount = targetCount
        };
    }
}
