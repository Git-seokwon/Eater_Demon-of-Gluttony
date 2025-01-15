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

        if (target is EnemyEntity enemy)
        {
            enemy.ApplyKnockback(knockBackDirection, knockBackPower, 0.5f);
        }
        else if (target is BossEntity boss)
        {
            // BossEntity에 대한 특수 Knockback 처리
            // 예: Knockback Power를 절반으로 줄이고 지속시간을 증가
            boss.ApplyKnockback(knockBackDirection, knockBackPower, 0.5f);
        }

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
