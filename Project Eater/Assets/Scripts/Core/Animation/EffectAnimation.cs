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

    private void Update()
    {
        if (effectAnimator.runtimeAnimatorController == null)
            return;

        transform.localScale = (movement.IsFlipX) ? new Vector2(1, 1) : new Vector2(-1, 1);
    }

    private void StartEffect(EffectAnimationNumber effect)
    {
        currentEffect = this.effect[Convert.ToInt32(effect)];
        effectAnimator.runtimeAnimatorController = currentEffect;
    }

    public void EndEffect()
    {
        effectAnimator.runtimeAnimatorController = null;
        transform.localScale = new Vector2(1, 1);
        currentEffect = null;
    }
}
