using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IncreaseStatByStackAction : EffectAction
{
    [SerializeField]
    private Stat[] bonusStats;
    [SerializeField]
    private float[] bonusValueStatFactor;

    private Effect ownerEffect;

    private float GetBonusStatValue(Entity user, Stat bonusValueStat, int index = 0)
    => user.Stats.GetValue(bonusValueStat) * bonusValueStatFactor[index] * ownerEffect.CurrentStack;

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
        int i = 0;

        foreach (var bonusStat in bonusStats)
        {
            float bonusValue = GetBonusStatValue(user, bonusStat, i++);
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
        var descriptionValuesByKeyword = new Dictionary<string, string>()
        {
            { "maxStack", (effect.MaxStack).ToString() }
        };

        for (int i = 0; i < bonusStats.Length; i++)
        {
            var statName = bonusStats[i].DisplayName;
            descriptionValuesByKeyword.Add("stat." + i, statName);
        }

        for (int i = 0; i < bonusValueStatFactor.Length; i++)
            descriptionValuesByKeyword.Add("statFactor." + i, bonusValueStatFactor[i].ToString());

        return descriptionValuesByKeyword;
    }

    public override object Clone()
    {
        return new IncreaseStatByStackAction()
        {
            bonusStats = (Stat[])bonusStats.Clone(),
            bonusValueStatFactor = (float[])bonusValueStatFactor.Clone(),
        };
    }
}
