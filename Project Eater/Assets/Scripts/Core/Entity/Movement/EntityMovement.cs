using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class EntityMovement : MonoBehaviour
{
    // 이동속도 Stat
    [SerializeField]
    private Stat moveSpeedStat;

    // Entity의 Rigidbody2D를 가져옴
    protected new Rigidbody2D rigidbody;

    // Entity의 Animator를 가져옴
    protected Animator animator;

    // Entity의 SpriteRenderer를 가져옴
    protected SpriteRenderer sprite;

    // 위 moveSpeedStat의 사본
    private Stat entityMoveSpeedStat;

    public Entity Owner {  get; private set; }

    // 이동속도 Property
    public float MoveSpeed => entityMoveSpeedStat.Value;


    // 추적 거리 
    [SerializeField] protected float chaseDistance;

    public virtual void Setup(Entity owner)
    {
        Owner = owner;

        rigidbody = Owner.rigidbody;
        animator = Owner.Animator;
        sprite = Owner.Sprite;

        entityMoveSpeedStat = moveSpeedStat ? Owner.Stats.GetStat(moveSpeedStat) : null;
        // → Eater에서는 agent를 사용하지 않고 entityMoveSpeedStat.Value 값을 바로 사용하기 때문에 
        //    OnMoveSpeedChanged 메서드는 만들지 않는다. 
    }

    private void OnDisable() => Stop();

    public virtual void Stop()
    {
        if (rigidbody)
            rigidbody.velocity = Vector2.zero;
    }
}
