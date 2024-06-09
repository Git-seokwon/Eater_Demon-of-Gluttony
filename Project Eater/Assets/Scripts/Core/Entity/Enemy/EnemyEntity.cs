using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class EnemyEntity : Entity
{
    public EnemyMovement Movement;

    protected override void Awake()
    {
        base.Awake();

        controlType = EntityControlType.AI;

        Movement = GetComponent<EnemyMovement>();
        Movement?.Setup(this);
    }

    public override void TakeDamage(Entity instigator, object causer, float damage)
    {
        base.TakeDamage(instigator, causer, damage);

        if (Mathf.Approximately(Stats.HungerStat.DefaultValue, 0f))
            OnDead();
    }

    private void OnDead()
    {
        if (Movement)
            Movement.enabled = false;

        CallOnDead(this);
    }
}
