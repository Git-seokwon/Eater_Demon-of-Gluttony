using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerEntity : Entity
{
    public PlayerMovement Movement;

    public MonoStateMachine<PlayerEntity> StateMachine {  get; private set; }

    protected override void Awake()
    {
        base.Awake();

        controlType = EntityControlType.Player;

        Movement = GetComponent<PlayerMovement>();
        Movement?.Setup(this);

        StateMachine = GetComponent<MonoStateMachine<PlayerEntity>>();
        StateMachine?.Setup(this);
    }

    public override void TakeDamage(Entity instigator, object causer, float damage)
    {
        base.TakeDamage(instigator, causer, damage);

        // ��� ó�� 
        if (Mathf.Approximately(Stats.HungerStat.DefaultValue, 0f))
            OnDead();
    }

    private void OnDead()
    {
        if (Movement)
            Movement.enabled = false;

        // onDead �̺�Ʈ ȣ�� �Լ� 
        CallOnDead(this);
    }

    // IsInState �Լ� Wrapping
    // �� �ܺο��� StateMachine Property�� ��ġ�� �ʰ� Entity�� ���� �ٷ� ���� State��
    //    �Ǻ��� �� �ֵ��� �ߴ�.
    public bool IsInState<T> () where T : State<PlayerEntity>
        => StateMachine.IsInState<T>();

    public bool IsInState<T>(int layer) where T : State<PlayerEntity>
    => StateMachine.IsInState<T>(layer);
}
