using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ※ StateTransition : State가 가지고 있는 Transition의 정보
public class StateTransition<EntityType>
{
    // Transition Command가 없음을 나타내는 상수
    // ※ Command : Animator의 Trigger와 같은 역할 (전이 조건의 일종)
    //            : Command는 Message처럼 int형이고 개발자가 정해주는 것 
    // ※ int.MinValue : Trigger가 없다는 뜻 (null)
    // → nullable을 쓰지 않는 이유 : 굳이 비용 들게 nullable을 쓸 정도까지는 아니여서 이렇게 처리를 함
    public const int kNullCommand = int.MinValue;

    // ※ Transition을 위한 조건 함수, 인자는 현재 State, 결과값은 전이 가능 여부(bool)
    // ※ Func : Func 대리자는 Action 대리자와 달리 파라미터 값이 있고, 반환값이 있다. (Action 대리자는 반환값이 없음)
    // → 인자 State : 출발 State에서 도착 State로 Transition을 연결할 때, Transition을 시작한 State로 여기서는 fromState라고 부른다.
    // → 반환 bool : transitionCondition의 결과값이 true라면 전이 조건을 만족했다는 의미이다. 
    private Func<State<EntityType>, bool> transitionCondition;

    // ※ CanTransitionToSelf : 자기 자신의 State로도 전이가 가능하지에 대한 여부
    // → Animator Any State의 Can Transition To Self 옵션
    // ex) 현재 A 상태인데 다시 A 상태로 갈 수 있는지의 유무
    // → 일반 Transition에서는 쓰지 않을 예정 ( 기본 값은 true로 설정 )
    // → 어떤 상태든지 조건만 맞으면 바로 전이시키는 Any Transition을 만들 때 사용 ( 기본 값은 false로 설정 ) 
    // (Any Transition에서 이 옵션 값이 true가 되면 계속해서 해당 State Enter만 들어가기 때문)
    public bool CanTransitionToSelf {  get; private set; }

    // 출발 State
    public State<EntityType> FromState { get; private set; }
    // 도착 State (전이할 State)
    public State<EntityType> ToState { get; private set; }

    // 전이 명령어
    // → 명령어가 kNullCommand이면 command가 없다는 뜻이다. 
    public int TransitionCommand { get; private set; }

    // ※ 전이 가능 여부(Condition 조건 만족 여부)
    // 1) transitionCondition이 null이면 조건이 없기 때문에 바로 전이
    // 2) transitionCondition의 결과값이 true이면 전이를 만족
    public bool IsTransferable => transitionCondition == null || transitionCondition.Invoke(FromState);

    // 생성자
    public StateTransition(State<EntityType> fromState, State<EntityType> toState,
                           int transitionCommand,
                           Func<State<EntityType>, bool> transitionCondition,
                           bool canTransitionToSelf)
    {
        // transitionCommand == kNullCommand && transitionCondition == null인 경우 오류 
        Debug.Assert(transitionCommand != kNullCommand || transitionCondition != null,
            "StateTransition - TransitionCommand와 TransitionCondition은 둘 다 null이 될 수 없습니다.");

        this.FromState = fromState;
        this.ToState = toState;
        this.TransitionCommand = transitionCommand;
        this.transitionCondition = transitionCondition;
        this.CanTransitionToSelf = canTransitionToSelf;
    }
}
