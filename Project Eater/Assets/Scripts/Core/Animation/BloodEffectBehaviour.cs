using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffectBehaviour : StateMachineBehaviour
{
    // Effect 재생이 끝나면 비활성화 
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.SetActive(false);
    }
}
