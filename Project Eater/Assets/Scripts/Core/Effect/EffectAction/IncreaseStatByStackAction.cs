using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IncreaseStatByStackAction : EffectAction
{
    [SerializeField]
    private Stat[] bonusStats;
    [SerializeField]
    private float bonusValueStatFactor;

    private Effect ownerEffect;

    private float GetBonusStatValue(Entity user, Stat bonusValueStat)
    => user.Stats.GetValue(bonusValueStat) * bonusValueStatFactor * ownerEffect.CurrentStack;

    public override void Start(Effect effect, Entity user, Entity target, int level, float scale)
    {
        ownerEffect = effect;
        user.onDealBasicDamage += OnDealBasicDamage;
    }

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        ownerEffect.CurrentStack -= 1;
        return true;
    }

    public override void Release(Effect effect, Entity user, Entity target, int level, float scale)
    {
        user.onDealBasicDamage -= OnDealBasicDamage;
        foreach (var bonusStat in bonusStats)
            user.Stats.RemoveBonusValue(bonusStat, this);
    }

    public override void OnEffectStackChanged(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        foreach (var bonusStat in bonusStats)
        {
            float bonusValue = GetBonusStatValue(user, bonusStat);
            user.Stats.SetBonusValue(bonusStat, this, bonusValue);
        }
    }

    public void OnDealBasicDamage(object causer, Entity target, float damage)
    {
        if (causer is Effect)
        {
            ownerEffect.CurrentStack += 1;
            ownerEffect.CurrentApplyCycle = 0f;
        }
    }

    protected override IReadOnlyDictionary<string, string> GetStringByKeyword(Effect effect)
    {
        // 스텟은 직접 쓰기 
        var descriptionValuesByKeyword = new Dictionary<string, string>
        {
            { "bonusDamageStatFactor", (bonusValueStatFactor * 100f).ToString() + "%" },
        };

        return descriptionValuesByKeyword;
    }

    public override object Clone()
    {
        return new IncreaseStatByStackAction()
        {
            bonusStats = bonusStats,
            bonusValueStatFactor = bonusValueStatFactor,
        };
    }
}
