using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReaperBaseLayerBehaviour : StateMachineBehaviour
{
    private readonly static int kDead = Animator.StringToHash("IsDead");

    private Entity entity;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (entity != null)
            return;

        entity = animator.GetComponent<Entity>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(kDead, entity.IsDead);
    }
}
