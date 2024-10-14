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

        // ���� Index
        // �� �ߺ��� Entity�� �����ϱ� ����
        int prevIndex = -1;

        for (int i = 0; i < targetCount; i++)
        {
            int currentIndex = Random.Range(0, skill.Targets.Count);

            // Target Index �����ϱ� 
            if (currentIndex != prevIndex)
                targetIndex.Add(currentIndex);
            // �ߺ� ������ �ٽ� �̱� 
            else
                i--;

            prevIndex = currentIndex;
        }

        // �����ص� Target Index�� ��ȸ�ϸ鼭 ��ų �����ϱ� 
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
