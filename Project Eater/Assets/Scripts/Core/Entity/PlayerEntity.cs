using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerEntity : Entity
{
    // Entity의 Hunger DeadLine : 해당 허기도를 넘기면 Entity는 사망처리가 된다.
    [SerializeField]
    protected float playerDeadLineHunger = 100f;

    public PlayerMovement Movement;

    protected override void Awake()
    {
        base.Awake();

        controlType = EntityControlType.Player;

        Movement = GetComponent<PlayerMovement>();
        Movement?.Setup(this);
    }

    private void Start()
    {
        SetDeadLineHunger(playerDeadLineHunger);
    }

    public override void IncreaseHunger(Entity instigator, object causer, float damage)
    {
        base.IncreaseHunger(instigator, causer, damage);

        // 사망 처리 
        if (Mathf.Approximately(Stats.HungerStat.DefaultValue, playerDeadLineHunger))
            OnDead();
    }

    private void OnDead()
    {
        if (Movement)
            Movement.enabled = false;

        // onDead 이벤트 호출 함수 
        CallOnDead(this);
    }
}
