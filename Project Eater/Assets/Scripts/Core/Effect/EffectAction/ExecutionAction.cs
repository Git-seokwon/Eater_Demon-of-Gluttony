using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExecutionAction : EffectAction
{
    [SerializeField]
    private float executionPercentage;
    // �ش� Impact�� ������Ʈ ��ü���� ������Բ� �ڵ带 �ۼ��ؾ� �Ѵ�. 
    [SerializeField]
    private GameObject executionImpact;

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        // ��� ü�� ���� �������� 
        var targetFullness = target.Stats.FullnessStat.Value;
        var targetMaxFullness = target.Stats.FullnessStat.MaxValue;

        var fullnessPercentage = targetFullness / targetMaxFullness;
        if (fullnessPercentage < executionPercentage)
        {
            PoolManager.Instance.ReuseGameObject(executionImpact, target.transform.position, Quaternion.identity);
            target.TakeDamage(user, effect, targetMaxFullness, true, false);
        }

        return true;
    }

    protected override IReadOnlyDictionary<string, string> GetStringByKeyword(Effect effect)
    {
        var descriptionValueByKeyword = new Dictionary<string, string>
        {
            ["executionPercentage"] = (executionPercentage * 100f).ToString() + "%",
        };

        return descriptionValueByKeyword;
    }

    public override object Clone()
    {
        return new ExecutionAction()
        {
            executionPercentage = executionPercentage,
            executionImpact = executionImpact
        };
    }

}
