using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectAnimationNumber
{
    Dash,
    Basic_Attack,
    Shard_Shot,
    Sniper_Shot,
    Sniper_Charge
}

public class EffectAnimation : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController[] effect;

    private Transform effectTransform;
    private PlayerMovement movement;
    private Animator effectAnimator;
    private SpriteRenderer spriteRenderer;
    private RuntimeAnimatorController currentEffect;

    public Animator EffectAnimator => effectAnimator;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.name == "Effect")
            {
                effectTransform = child;
                break;
            }
        }

        movement = GetComponent<PlayerMovement>();

        effectAnimator = effectTransform?.GetComponent<Animator>();
        spriteRenderer = effectTransform?.GetComponent<SpriteRenderer>();
    }

    private void StartEffect(EffectAnimationNumber effect)
    {
        currentEffect = this.effect[Convert.ToInt32(effect)];
        effectAnimator.runtimeAnimatorController = currentEffect;
    }

    public void EndEffect()
    {
        effectAnimator.runtimeAnimatorController = null;
        currentEffect = null;
    }
}
