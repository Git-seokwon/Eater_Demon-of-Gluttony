using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IncreaseStatByMeatAction : EffectAction
{
    [SerializeField]
    private Stat[] bonusStats;
    [SerializeField]
    private float[] defaultValue;
    [SerializeField]
    private float[] bonusValueStatFactor;

    [SerializeField]
    private bool isBonusType = true;

    private float totalValue;

    private float GetBonusValue(Entity user, Stat bonusValueStat, int index = 0)
    => bonusValueStatFactor[index] * (user as PlayerEntity).MeatStack;

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        for (int i = 0; i < bonusStats.Length; i++)
        {
            totalValue = user.Stats.GetValue(bonusStats[i]) * (defaultValue[i] + GetBonusValue(user, bonusStats[i], i));

            if (isBonusType)
                target.Stats.SetBonusValue(bonusStats[i], this, totalValue);
            else
                target.Stats.IncreaseDefaultValue(bonusStats[i], totalValue);
        }

        (user as PlayerEntity).MeatStack = 0;

        return true;
    }

    public override void Release(Effect effect, Entity user, Entity target, int level, float scale)
    {
        for (int i = 0; i < bonusStats.Length; i++)
        {
            if (isBonusType)
                target.Stats.RemoveBonusValue(bonusStats[i], this);
            else
                target.Stats.IncreaseDefaultValue(bonusStats[i], -totalValue);
        }
    }

    protected override IReadOnlyDictionary<string, string> GetStringByKeyword(Effect effect)
    {
        var descriptionValuesByKeyword = new Dictionary<string, string>();

        for (int i = 0; i < bonusStats.Length; i++)
        {
            var statName = bonusStats[i].DisplayName;
            descriptionValuesByKeyword.Add("stat." + i, statName);
        }

        for (int i = 0; i < defaultValue.Length; i++)
            descriptionValuesByKeyword.Add("defaultValue." + i, defaultValue[i].ToString());

        for (int i = 0; i < bonusValueStatFactor.Length; i++)
            descriptionValuesByKeyword.Add("statFactor." + i, bonusValueStatFactor[i].ToString());

        if (effect.Owner != null)
        {
            for (int i = 0; i < bonusStats.Length; i++)
            {
                descriptionValuesByKeyword.Add("totalValue." + i,
                    (defaultValue[i] + GetBonusValue(effect.User, bonusStats[i], i)).ToString());   
            }
        }

        return descriptionValuesByKeyword;
    }

    public override object Clone()
    {
        return new IncreaseStatByMeatAction()
        {
            // bonusStats, defaultValue, bonusValueStatFactor ±íÀº º¹»ç 
            bonusStats = (Stat[])bonusStats.Clone(),
            defaultValue = (float[])defaultValue.Clone(),
            bonusValueStatFactor = (float[])bonusValueStatFactor.Clone(),
            isBonusType = isBonusType,
        };
    }
}
