using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectAnimationNumber
{
    Dash,

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

        spriteRenderer.flipX = movement.IsFlipX;
    }

    private void StartEffect(EffectAnimationNumber effect)
    {
        currentEffect = this.effect[Convert.ToInt32(effect)];
        effectAnimator.runtimeAnimatorController = currentEffect;
    }

    private void EndEffect()
    {
        effectAnimator.runtimeAnimatorController = null;
        currentEffect = null;
    }
}
