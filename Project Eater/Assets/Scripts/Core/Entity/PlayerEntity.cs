using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerEntity : Entity
{
    // Entity�� Hunger DeadLine : �ش� ��⵵�� �ѱ�� Entity�� ���ó���� �ȴ�.
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

        // ��� ó�� 
        if (Mathf.Approximately(Stats.HungerStat.DefaultValue, playerDeadLineHunger))
            OnDead();
    }

    private void OnDead()
    {
        if (Movement)
            Movement.enabled = false;

        // onDead �̺�Ʈ ȣ�� �Լ� 
        CallOnDead(this);
    }
}
