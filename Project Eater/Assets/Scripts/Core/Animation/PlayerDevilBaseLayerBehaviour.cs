using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ※ StateMachineBehaviour : 상태 머신 동작(State Machine Behaviour) 특별 스크립트 클래스
// → 일반 Unity 스크립트(MonoBehaviours)를 개별 게임 오브젝트에 연결하는 것과 유사한 방법으로
//    StateMachineBehaviour 스크립트를 상태 머신에 개별 상태로 연결
// → 이렇게 하면 상태 머신이 특정 상태로 전환되거나, 상태를 종료하거나, 해당 상태로 유지될 때
//    실행할 코드를 작성할 수 있다 (Animator)
// → 상태를 테스트하거나 상태 변경을 감지하기 위해 로직을 직접 작성할 필요가 없다
public class PlayerDevilBaseLayerBehaviour : StateMachineBehaviour
{
    // readonly static으로 Animation 파라미터의 Hash를 가져온다. 
    // → Hash를 안 쓰고 문자열 그대로 Parameter Setting을 하는 경우가 있는데 
    //    Performance 적으로 좋지 않기 때문에 Unity에서도 Hash를 쓸걸 권장하고 있다.
    // → Animator의 moveSpeed 파라미터 Hash로 가져오기 
    private readonly static int kMoveSpeed = Animator.StringToHash("MoveSpeed");
    private readonly static int kDash = Animator.StringToHash("Dash");
    private readonly static int kDashDown = Animator.StringToHash("DashDown");
    private readonly static int kDashUp = Animator.StringToHash("DashUp");

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
        {
            // MoveDirection.magnitude와 kMoveSpeed를 bind
            animator.SetFloat(kMoveSpeed, PlayerController.Instance.MoveDirection.magnitude);
            animator.SetBool(kDash, movment.IsDashing);
            animator.SetBool(kDashDown, movment.IsDashingDown);
            animator.SetBool(kDashUp, movment.IsDashingUp);
        }
    }
}
