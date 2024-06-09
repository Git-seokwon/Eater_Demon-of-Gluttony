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

        // 사망 처리 
        if (Mathf.Approximately(Stats.HungerStat.DefaultValue, 0f))
            OnDead();
    }

    private void OnDead()
    {
        if (Movement)
            Movement.enabled = false;

        // onDead 이벤트 호출 함수 
        CallOnDead(this);
    }

    // IsInState 함수 Wrapping
    // → 외부에서 StateMachine Property를 거치지 않고 Entity를 통해 바로 현재 State를
    //    판별할 수 있도록 했다.
    public bool IsInState<T> () where T : State<PlayerEntity>
        => StateMachine.IsInState<T>();

    public bool IsInState<T>(int layer) where T : State<PlayerEntity>
    => StateMachine.IsInState<T>(layer);
}
