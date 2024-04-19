using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class EnemyEntity : Entity
{
    [SerializeField]
    protected float enemyDeadLineHunger = 100f;

    public EnemyMovement Movement;

    protected override void Awake()
    {
        base.Awake();

        controlType = EntityControlType.AI;

        Movement = GetComponent<EnemyMovement>();
        Movement?.Setup(this);
    }

    private void Start()
    {
        SetDeadLineHunger(enemyDeadLineHunger);
    }

    public override void IncreaseHunger(Entity instigator, object causer, float damage)
    {
        base.IncreaseHunger(instigator, causer, damage);

        if (Mathf.Approximately(Stats.HungerStat.DefaultValue, enemyDeadLineHunger))
            OnDead();
    }

    private void OnDead()
    {
        if (Movement)
            Movement.enabled = false;

        CallOnDead(this);
    }
}
