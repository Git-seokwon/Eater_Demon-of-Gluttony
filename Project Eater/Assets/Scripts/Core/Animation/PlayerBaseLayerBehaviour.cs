using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseLayerBehaviour : StateMachineBehaviour
{
    private readonly static int kMoveSpeed = Animator.StringToHash("MoveSpeed");

    private Entity entity;
    private PlayerMovement movment;

    // ※ OnStateEnter : Layer 가진 어떤 Node든 Node가 시작될 때 실행되는 함수
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    // ※ AnimatorStateInfo : 현재 Animation Node 정보 
    // ※ layerIndex : 현재 Layer의 Index
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (entity != null)
            return;

        entity = animator.GetComponent<Entity>();
        movment = animator.GetComponent<PlayerMovement>();
    }

    // ※ OnStateUpdate : MonoBehaviour의 Update와 같은 역할의 함수 
    //                  : Node가 실행되는 동안 계속 실행되는 함수 
    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (movment)
            // MoveDirection.magnitude와 kMoveSpeed를 bind
            animator.SetFloat(kMoveSpeed, PlayerController.Instance.MoveDirection.magnitude);
    }
}
