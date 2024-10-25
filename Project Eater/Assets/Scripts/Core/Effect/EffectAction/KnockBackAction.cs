using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KnockBackAction : EffectAction
{
    [SerializeField]
    private int knockBackPower; 

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        var knockBackDirection = (target.rigidbody.position - user.rigidbody.position).normalized;
        (target as EnemyEntity).ApplyKnockback(knockBackDirection, knockBackPower, 0.5f);

        return true;
    }

    protected override IReadOnlyDictionary<string, string> GetStringByKeyword(Effect effect)
    {
        var descriptionValueByKeyword = new Dictionary<string, string>()
        {
            ["knockBackPower"] = knockBackPower.ToString()
        };

        return descriptionValueByKeyword;
    }

    public override object Clone()
    {
        return new KnockBackAction()
        {
            knockBackPower = knockBackPower
        };
    }
}
