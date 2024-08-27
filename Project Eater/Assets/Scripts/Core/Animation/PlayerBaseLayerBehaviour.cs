using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseLayerBehaviour : StateMachineBehaviour
{
    private readonly static int kMoveSpeed = Animator.StringToHash("MoveSpeed");

    private Entity entity;
    private PlayerMovement movment;

    // �� OnStateEnter : Layer ���� � Node�� Node�� ���۵� �� ����Ǵ� �Լ�
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    // �� AnimatorStateInfo : ���� Animation Node ���� 
    // �� layerIndex : ���� Layer�� Index
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (entity != null)
            return;

        entity = animator.GetComponent<Entity>();
        movment = animator.GetComponent<PlayerMovement>();
    }

    // �� OnStateUpdate : MonoBehaviour�� Update�� ���� ������ �Լ� 
    //                  : Node�� ����Ǵ� ���� ��� ����Ǵ� �Լ� 
    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (movment)
            // MoveDirection.magnitude�� kMoveSpeed�� bind
            animator.SetFloat(kMoveSpeed, PlayerController.Instance.MoveDirection.magnitude);
    }
}
