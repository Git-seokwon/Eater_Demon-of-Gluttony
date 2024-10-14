using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Animation Node�� ���� Script�̱� ������ StateMachineBehaviour�� ��� �޴´�. 
public class AnimatorParameterSetter : StateMachineBehaviour
{
    // AnimationParameter ���� Animation�� ������ �� �ٲ���, ������ �ٲ��� ��Ÿ���� Enum
    private enum State { Enter, Update, Exit }

    [SerializeField]
    private State state;
    [SerializeField]
    private string parameterName;

    // �ٲ� �� 
    // Ex) isOn�� true�̸� Parameter ���� true�� �ǰ�, false�� Parameter ���� false�� �ȴ�. 
    [SerializeField]
    private bool isOn;

    // Parameter�� Hash ���� ������ ���� 
    private int hash = int.MinValue;

    // Animation�� ���۵� �� ����Ǵ� OnStateEnter �Լ� 
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (hash == int.MinValue)
            hash = Animator.StringToHash(parameterName);

        if (state == State.Enter)
            animator.SetBool(hash, isOn);
    }

    // �� �����Ӹ��� ȣ��Ǵ� OnStateUpdate �Լ�
    // �� �Լ����� �ִϸ��̼��� ����Ǵ� ������ ������ �� ����
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // normalizedTime�� 1 �̻��̸� �ִϸ��̼��� ����� ��
        if (state == State.Update && stateInfo.normalizedTime >= 1.0f)
            animator.SetBool(hash, isOn);
    }

    // Animation�� ������ ��, ����Ǵ� OnStateExit �Լ� 
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (state == State.Exit)
            animator.SetBool(hash, isOn);
    }
}
