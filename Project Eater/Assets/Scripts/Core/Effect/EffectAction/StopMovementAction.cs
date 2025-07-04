using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StopMovementAction : EffectAction
{
    public override void Start(Effect effect, Entity user, Entity target, int level, float scale)
    {
        // target이 BossEntity인 경우 아무 것도 하지 않고 return
        if (target.TryGetComponent<BossEntity>(out _))
            return;

        // EnemyMovement가 존재하면 비활성화
        if (target.TryGetComponent<EntityMovement>(out var movement))
        {
            movement.Stop();
            movement.enabled = false;
            target.Animator.speed = 0f;
        }
    }

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale) => true;

    public override object Clone()
    {
        return new StopMovementAction()
        {
        };
    }
}
