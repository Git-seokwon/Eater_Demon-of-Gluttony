using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StopMovementAction : EffectAction
{
    public override void Start(Effect effect, Entity user, Entity target, int level, float scale)
    {
        (target as EnemyEntity).EnemyMovement.Stop();
        (target as EnemyEntity).EnemyMovement.enabled = false;
    }

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale) => true;

    public override object Clone()
    {
        return new StopMovementAction()
        {
        };
    }
}
