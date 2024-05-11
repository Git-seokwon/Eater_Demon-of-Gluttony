using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class EntityMovement : MonoBehaviour
{
    // �̵��ӵ� Stat
    [SerializeField]
    private Stat moveSpeedStat;

    // Entity�� Rigidbody2D�� ������
    protected new Rigidbody2D rigidbody;

    // Entity�� Animator�� ������
    protected Animator animator;

    // Entity�� SpriteRenderer�� ������
    protected SpriteRenderer sprite;

    // �� moveSpeedStat�� �纻
    private Stat entityMoveSpeedStat;

    public Entity Owner {  get; private set; }

    // �̵��ӵ� Property
    public float MoveSpeed => entityMoveSpeedStat.Value;


    // ���� �Ÿ� 
    [SerializeField] protected float chaseDistance;

    public virtual void Setup(Entity owner)
    {
        Owner = owner;

        rigidbody = Owner.rigidbody;
        animator = Owner.Animator;
        sprite = Owner.Sprite;

        entityMoveSpeedStat = moveSpeedStat ? Owner.Stats.GetStat(moveSpeedStat) : null;
        // �� Eater������ agent�� ������� �ʰ� entityMoveSpeedStat.Value ���� �ٷ� ����ϱ� ������ 
        //    OnMoveSpeedChanged �޼���� ������ �ʴ´�. 
    }

    private void OnDisable() => Stop();

    public virtual void Stop()
    {
        if (rigidbody)
            rigidbody.velocity = Vector2.zero;
    }
}
