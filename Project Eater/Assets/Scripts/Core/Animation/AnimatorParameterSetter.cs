using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Animation Node에 붙일 Script이기 때문에 StateMachineBehaviour를 상속 받는다. 
public class AnimatorParameterSetter : StateMachineBehaviour
{
    // AnimationParameter 값을 Animation이 시작할 때 바꿀지, 끝나고 바꿀지 나타내는 Enum
    private enum State { Enter, Update, Exit }

    [SerializeField]
    private State state;
    [SerializeField]
    private string parameterName;

    // 바꿀 값 
    // Ex) isOn이 true이면 Parameter 값이 true가 되고, false면 Parameter 값이 false가 된다. 
    [SerializeField]
    private bool isOn;

    // Parameter의 Hash 값을 저장할 변수 
    private int hash = int.MinValue;

    // Animation이 시작될 때 실행되는 OnStateEnter 함수 
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (hash == int.MinValue)
            hash = Animator.StringToHash(parameterName);

        if (state == State.Enter)
            animator.SetBool(hash, isOn);
    }

    // 매 프레임마다 호출되는 OnStateUpdate 함수
    // 이 함수에서 애니메이션이 종료되는 시점을 감지할 수 있음
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // normalizedTime이 1 이상이면 애니메이션이 종료된 것
        if (state == State.Update && stateInfo.normalizedTime >= 1.0f)
            animator.SetBool(hash, isOn);
    }

    // Animation이 끝났을 때, 실행되는 OnStateExit 함수 
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (state == State.Exit)
            animator.SetBool(hash, isOn);
    }
}
