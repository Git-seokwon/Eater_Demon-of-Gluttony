using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class IsFullnessReadyCondition : SkillCondition
{
    [SerializeField]
    private float percentage;
    [SerializeField]
    private Stat stat;

    public override bool IsPass(Skill skill)
    {
        var maxValue = stat.MaxValue;
        var value = stat.Value;
        var currentPercentage = value / maxValue;

        // Mathf.Epsilon�� �̿��� �ణ�� ������ ����ϴ� ���
        return currentPercentage + Mathf.Epsilon >= percentage;
    }

    public override object Clone()
    {
        return new IsFullnessReadyCondition()
        {
            percentage = percentage,
            stat = stat
        };
    }
}
