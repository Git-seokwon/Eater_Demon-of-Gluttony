using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffectBehaviour : StateMachineBehaviour
{
    // Effect ����� ������ ��Ȱ��ȭ 
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.SetActive(false);
    }
}
