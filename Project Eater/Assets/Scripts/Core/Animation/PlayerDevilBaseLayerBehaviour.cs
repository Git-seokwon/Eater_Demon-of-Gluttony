using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� StateMachineBehaviour : ���� �ӽ� ����(State Machine Behaviour) Ư�� ��ũ��Ʈ Ŭ����
// �� �Ϲ� Unity ��ũ��Ʈ(MonoBehaviours)�� ���� ���� ������Ʈ�� �����ϴ� �Ͱ� ������ �������
//    StateMachineBehaviour ��ũ��Ʈ�� ���� �ӽſ� ���� ���·� ����
// �� �̷��� �ϸ� ���� �ӽ��� Ư�� ���·� ��ȯ�ǰų�, ���¸� �����ϰų�, �ش� ���·� ������ ��
//    ������ �ڵ带 �ۼ��� �� �ִ� (Animator)
// �� ���¸� �׽�Ʈ�ϰų� ���� ������ �����ϱ� ���� ������ ���� �ۼ��� �ʿ䰡 ����
public class PlayerDevilBaseLayerBehaviour : StateMachineBehaviour
{
    // readonly static���� Animation �Ķ������ Hash�� �����´�. 
    // �� Hash�� �� ���� ���ڿ� �״�� Parameter Setting�� �ϴ� ��찡 �ִµ� 
    //    Performance ������ ���� �ʱ� ������ Unity������ Hash�� ���� �����ϰ� �ִ�.
    // �� Animator�� moveSpeed �Ķ���� Hash�� �������� 
    private readonly static int kMoveSpeed = Animator.StringToHash("MoveSpeed");
    private readonly static int kDash = Animator.StringToHash("Dash");
    private readonly static int kDashDown = Animator.StringToHash("DashDown");
    private readonly static int kDashUp = Animator.StringToHash("DashUp");

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
        {
            // MoveDirection.magnitude�� kMoveSpeed�� bind
            animator.SetFloat(kMoveSpeed, PlayerController.Instance.MoveDirection.magnitude);
            animator.SetBool(kDash, movment.IsDashing);
            animator.SetBool(kDashDown, movment.IsDashingDown);
            animator.SetBool(kDashUp, movment.IsDashingUp);
        }
    }
}
