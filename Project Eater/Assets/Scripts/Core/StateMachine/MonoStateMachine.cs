using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ※ MonoStateMachine : GameObject에 Component로 넣을 수 있는 StateMachine
// → StateMachine 변수를 만들고 그 변수의 함수들을 Wrapping하는 방식으로 구현
// → Entity Class의 StateMachine은 이 MonoStateMachine으로 만들 것
public abstract class MonoStateMachine<EntityType> : MonoBehaviour
{
	#region Event
	public delegate void StateChangedHandler(StateMachine<EntityType> stateMachine,
											 State<EntityType> newState,
											 State<EntityType> prevState,
											 int layer);
	#endregion

	public event StateChangedHandler onStateChanged;

	private readonly StateMachine<EntityType> stateMachine = new();

	public EntityType Owner => stateMachine.Owner;

    private void Update()
    {
		if (Owner != null)
			stateMachine.Update();
    }

    private void FixedUpdate()
    {
        if (Owner != null)
            stateMachine.FixedUpdate();
    }

    public void Setup(EntityType owner)
	{
		stateMachine.Setup(owner);
        // → 여기서 stateMachine은 기본 stateMachine이기 때문에 
        //    추가된 State와 Transition이 없어서 텅 비어있는 상태이다.

        AddStates();
		MakeTransitions();

		stateMachine.SetupLayers();

        // stateMachine.onStateChanged 이벤트에 MonoStateMachine의 onStateChanged를 등록 
        // ※ 람다식 사용
        // 1) 매개변수 : (_, newState, prevState, layer)
        // 2) Method Body : onStateChanged?.Invoke(stateMachine, newState, prevState, layer);
        stateMachine.onStateChanged += (_, newState, prevState, layer)
			=> onStateChanged?.Invoke(stateMachine, newState, prevState, layer);
    }

    #region StateMachine 함수 Wrapping
    public void AddState<T>(int layer = 0)
        where T : State<EntityType>
        => stateMachine.AddState<T>(layer);

    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand,
        Func<State<EntityType>, bool> transitionCondition,
        int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => stateMachine.MakeTransition<FromStateType, ToStateType>(transitionCommand, transitionCondition, layer);

    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand,
        Func<State<EntityType>, bool> transitionCondition,
        int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => stateMachine.MakeTransition<FromStateType, ToStateType>(transitionCommand, transitionCondition, layer);

    public void MakeTransition<FromStateType, ToStateType>(
        Func<State<EntityType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => stateMachine.MakeTransition<FromStateType, ToStateType>(int.MinValue, transitionCondition, layer);

    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => stateMachine.MakeTransition<FromStateType, ToStateType>(transitionCommand, null, layer);

    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => stateMachine.MakeTransition<FromStateType, ToStateType>(transitionCommand, null, layer);

    public void MakeAnyTransition<ToStateType>(int transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<EntityType>
        => stateMachine.MakeAnyTransition<ToStateType>(transitionCommand, transitionCondition, layer, canTransitionToSelf);

    public void MakeAnyTransition<ToStateType>(Enum transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<EntityType>
        => stateMachine.MakeAnyTransition<ToStateType>(transitionCommand, transitionCondition, layer, canTransitionToSelf);

    public void MakeAnyTransition<ToStateType>(Func<State<EntityType>, bool> transitionCondition,
        int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<EntityType>
        => stateMachine.MakeAnyTransition<ToStateType>(int.MinValue, transitionCondition, layer, canTransitionToSelf);

    public void MakeAnyTransition<ToStateType>(Enum transitionCommand, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<EntityType>
        => stateMachine.MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitionToSelf);

    public void MakeAnyTransition<ToStateType>(int transitionCommand, int layer = 0, bool canTransitionToSelf = false)
    where ToStateType : State<EntityType>
        => stateMachine.MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitionToSelf);

    public bool ExecuteCommand(int transitionCommand, int layer)
        => stateMachine.ExecuteCommand(transitionCommand, layer);

    public bool ExecuteCommand(Enum transitionCommand, int layer)
        => stateMachine.ExecuteCommand(transitionCommand, layer);

    public bool ExecuteCommand(int transitionCommand)
        => stateMachine.ExecuteCommand(transitionCommand);

    public bool ExecuteCommand(Enum transitionCommand)
        => stateMachine.ExecuteCommand(transitionCommand);

    public bool SendMessage(int message, int layer, object extraData = null)
        => stateMachine.SendMessage(message, layer, extraData);

    public bool SendMessage(Enum message, int layer, object extraData = null)
        => stateMachine.SendMessage(message, layer, extraData);

    public bool SendMessage(int message, object extraData = null)
        => stateMachine.SendMessage(message, extraData);

    public bool SendMessage(Enum message, object extraData = null)
        => stateMachine.SendMessage(message, extraData);

    public bool IsInState<T>() where T : State<EntityType>
        => stateMachine.IsInState<T>();

    public bool IsInState<T>(int layer = 0) where T : State<EntityType>
        => stateMachine.IsInState<T>(layer);

    public State<EntityType> GetCurrentState(int layer = 0) => stateMachine.GetCurrentState(layer);

    public Type GetCurrentStateType(int layer = 0) => stateMachine.GetCurrentStateType(layer);
    #endregion

    #region Abstract Method
    // MonoStateMachine을 상속 받는 Class에서 Override해서 AddStates와 MakeTransitions 함수를 쓰면 
    // StateMachine에 State와 Transition이 추가되는 형식
    protected abstract void AddStates();
	protected abstract void MakeTransitions();
	#endregion
}
