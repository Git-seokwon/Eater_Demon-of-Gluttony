using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeakAction : EffectAction
{
    [SerializeField]
    private Category removeTargetCategory;
    [SerializeField]
    private float weakPercentage;

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        if (target is PlayerEntity player && player.StateMachine.IsInState<PlayerSuperArmorState>())
            return true;

        target.SkillSystem.RemoveEffectAll(removeTargetCategory);

        // 방어력 감소량 구하기 
        var decrease = target.Stats.DefenceStat.Value * weakPercentage;
        // 방어력 감소 
        target.Stats.DefenceStat.SetBonusValue(this, -decrease);

        return true;
    }

    public override void Release(Effect effect, Entity user, Entity target, int level, float scale)
        => target.Stats.RemoveBonusValue(target.Stats.DefenceStat, this);

    protected override IReadOnlyDictionary<string, string> GetStringByKeyword(Effect effect)
    {
        var descriptionValueByKeyword = new Dictionary<string, string>
        {
            ["weakPercentage"] = (weakPercentage * 100f).ToString() + "%",
        };

        return descriptionValueByKeyword;
    }

    public override object Clone()
    {
        return new WeakAction()
        {
            removeTargetCategory = removeTargetCategory,
            weakPercentage = weakPercentage,
        };
    }
}
