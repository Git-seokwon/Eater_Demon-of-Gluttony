using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� StateTransition : State�� ������ �ִ� Transition�� ����
public class StateTransition<EntityType>
{
    // Transition Command�� ������ ��Ÿ���� ���
    // �� Command : Animator�� Trigger�� ���� ���� (���� ������ ����)
    //            : Command�� Messageó�� int���̰� �����ڰ� �����ִ� �� 
    // �� int.MinValue : Trigger�� ���ٴ� �� (null)
    // �� nullable�� ���� �ʴ� ���� : ���� ��� ��� nullable�� �� ���������� �ƴϿ��� �̷��� ó���� ��
    public const int kNullCommand = int.MinValue;

    // �� Transition�� ���� ���� �Լ�, ���ڴ� ���� State, ������� ���� ���� ����(bool)
    // �� Func : Func �븮�ڴ� Action �븮�ڿ� �޸� �Ķ���� ���� �ְ�, ��ȯ���� �ִ�. (Action �븮�ڴ� ��ȯ���� ����)
    // �� ���� State : ��� State���� ���� State�� Transition�� ������ ��, Transition�� ������ State�� ���⼭�� fromState��� �θ���.
    // �� ��ȯ bool : transitionCondition�� ������� true��� ���� ������ �����ߴٴ� �ǹ��̴�. 
    private Func<State<EntityType>, bool> transitionCondition;

    // �� CanTransitionToSelf : �ڱ� �ڽ��� State�ε� ���̰� ���������� ���� ����
    // �� Animator Any State�� Can Transition To Self �ɼ�
    // ex) ���� A �����ε� �ٽ� A ���·� �� �� �ִ����� ����
    // �� �Ϲ� Transition������ ���� ���� ���� ( �⺻ ���� true�� ���� )
    // �� � ���µ��� ���Ǹ� ������ �ٷ� ���̽�Ű�� Any Transition�� ���� �� ��� ( �⺻ ���� false�� ���� ) 
    // (Any Transition���� �� �ɼ� ���� true�� �Ǹ� ����ؼ� �ش� State Enter�� ���� ����)
    public bool CanTransitionToSelf {  get; private set; }

    // ��� State
    public State<EntityType> FromState { get; private set; }
    // ���� State (������ State)
    public State<EntityType> ToState { get; private set; }

    // ���� ��ɾ�
    // �� ��ɾ kNullCommand�̸� command�� ���ٴ� ���̴�. 
    public int TransitionCommand { get; private set; }

    // �� ���� ���� ����(Condition ���� ���� ����)
    // 1) transitionCondition�� null�̸� ������ ���� ������ �ٷ� ����
    // 2) transitionCondition�� ������� true�̸� ���̸� ����
    public bool IsTransferable => transitionCondition == null || transitionCondition.Invoke(FromState);

    // ������
    public StateTransition(State<EntityType> fromState, State<EntityType> toState,
                           int transitionCommand,
                           Func<State<EntityType>, bool> transitionCondition,
                           bool canTransitionToSelf)
    {
        // transitionCommand == kNullCommand && transitionCondition == null�� ��� ���� 
        Debug.Assert(transitionCommand != kNullCommand || transitionCondition != null,
            "StateTransition - TransitionCommand�� TransitionCondition�� �� �� null�� �� �� �����ϴ�.");

        this.FromState = fromState;
        this.ToState = toState;
        this.TransitionCommand = transitionCommand;
        this.transitionCondition = transitionCondition;
        this.CanTransitionToSelf = canTransitionToSelf;
    }
}
