using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SuperArmorAction : EffectAction
{
    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        if (user is PlayerEntity player && !player.isGrit)
            player.isGrit = true;

        return true;
    }

    public override object Clone() => new SuperArmorAction();
}
