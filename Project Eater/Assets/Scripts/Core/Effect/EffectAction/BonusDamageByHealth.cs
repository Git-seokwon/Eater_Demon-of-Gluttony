using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BonusDamageByHealth : EffectAction
{
    [SerializeField]
    private float bonusDamageFactor;

    public override void Start(Effect effect, Entity user, Entity target, int level, float scale)
    {
        var player = user as PlayerEntity;

        if (player != null) 
        {
            player.isRuthless = true;
            player.BonusDamagePercent = bonusDamageFactor;
        }
    }

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale) => true;

    public override void Release(Effect effect, Entity user, Entity target, int level, float scale)
    {
        var player = user as PlayerEntity;

        if (player != null)
        {
            player.isRuthless = false;
            player.BonusDamagePercent = 0f;
        }
    }

    protected override IReadOnlyDictionary<string, string> GetStringByKeyword(Effect effect)
    {
        var descriptionValuesByKeyword = new Dictionary<string, string>
        {
            { "bonusDamageFactor", (bonusDamageFactor * 100f).ToString() + "%" }
        };

        return descriptionValuesByKeyword;
    }

    public override object Clone()
    {
        return new BonusDamageByHealth()
        {
            bonusDamageFactor = bonusDamageFactor
        };
    }
}
