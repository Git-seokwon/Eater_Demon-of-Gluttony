using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlowAction : EffectAction
{
    [SerializeField]
    private Category removeTargetCategory;
    [SerializeField]
    private float slowPercentage;

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        target.SkillSystem.RemoveEffectAll(removeTargetCategory);

        var decrease = target.Stats.MoveSpeedStat.Value * slowPercentage;
        target.Stats.MoveSpeedStat.SetBonusValue(this, -decrease);

        return true;
    }

    public override void Release(Effect effect, Entity user, Entity target, int level, float scale)
        => target.Stats.MoveSpeedStat.RemoveBonusValue(this);

    protected override IReadOnlyDictionary<string, string> GetStringByKeyword(Effect effect)
    {
        var descriptionValueByKeyword = new Dictionary<string, string>
        {
            ["slowPercentage"] = (slowPercentage * 100f).ToString() + "%",
        };

        return descriptionValueByKeyword;
    }

    public override object Clone()
    {
        return new SlowAction()
        {
            removeTargetCategory = removeTargetCategory,
            slowPercentage = slowPercentage
        };
    }
}
