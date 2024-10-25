using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// �� StateMachine ���
/* 1.  State�� ���̵Ǿ����� �˸� (Event)
   2.  Layer �� StateData���� ����
   3.  Layer �� AnyTransition���� ����
   4.  Layer �� ���� �������� StateData�� ����
   5.  Layer ����
   6.  StateMachine Setup ���� (State, Transition, Layer)
   7.  ���� State ���� ���
   8.  ���̸� �õ��ϴ� ��� (���� State ���� �� ���)
   9.  Update ��� (StateMahcine ���̸� ��� �õ��ϸ鼭 ���� ������ �����ϸ� ���� or ���� State�� Update�� ����)
   10. State�� �����Ͽ� StateMachine�� �ִ� ��� 
   11. Transition�� �����Ͽ� State�� ��������ֱ�(romStateData.Transitions.Add(newTransition))
   12. Any Transition�� �����Ͽ� StateMachine�� ����ϱ�(anyTransitionsByLayer[layer].Add(newTransition))
   13. Command�� �޾Ƽ� �ش� Command�� ���� Transition�� �����ϴ� ���
   14. ���� �������� CurrentStateData�� Message�� ������ ��� ()
   15. ���� �������� CurrentState�� Ȯ���Ͽ�, ���� State�� T Type�� State���� Ȯ���ϴ� ���
   16. ���� �������� State�� Type�� �������� ���
*/
public class StateMachine<EntityType>
{
    #region Event
    // State�� ���̵Ǿ����� �˸��� Event
    public delegate void StateChangedHandler(StateMachine<EntityType> stateMachine, // ���� State
                                             State<EntityType> newState,
                                             State<EntityType> prevState,
                                             int layer); // State�� ���� Layer ��ȣ 
    #endregion

    #region Variable
    // StateData Class : State�� State�� ���� �ΰ� ������ ��� �ִ�.
    private class StateData
    {
        // State�� ����Ǵ� Layer
        public int Layer { get; private set; }

        // State�� Layer�� ��ϵ� ����
        public int Priority { get; private set; }

        // ���� State ����
        public State<EntityType> State { get; private set; }

        // ���� State�� from�� Transitions List
        public List<StateTransition<EntityType>> Transitions { get; private set; } = new();

        // ������
        public StateData(int layer, int priority, State<EntityType> state)
            => (Layer, Priority, State) = (layer, priority, state); // �ش� ���� ����ϱ�!
    }

    // Layer�� ������ �ִ� StateDatas(=Layer Dictionary)
    // �� int Key : StateData�� ���� Layer ��ȣ 
    // Value Dictionary�� Key�� Value�� StateData�� ���� State�� Type
    // �� ��, State�� Type�� ���� �ش� StateData(State)�� ���� ã�ƿ� �� ����
    private readonly Dictionary<int, Dictionary<Type, StateData>> stateDatasByLayer = new();

    // Layer�� Any Tansitions(���Ǹ� �����ϸ� �������� ToState�� ���̵Ǵ� Transition)
    // �� �ش� Layer�� ���� �ִ� ��� State�� Any Tansition���� ������ �� �ִ�. ��, �ش� Layer�� State�� ��� �������� ������ �ִ�
    //    Transitions�̴�. (���� ���ǵ� ����)
    private readonly Dictionary<int, List<StateTransition<EntityType>>> anyTransitionsByLayer = new();

    // Layer�� ���� �������� StateData(=���� �������� State)
    private readonly Dictionary<int, StateData> currentStateDatasByLayer = new();

    // StateMachine�� �����ϴ� Layer��, Layer�� �ߺ����� �ʾƾ� �ϰ�, �ڵ� ������ ���ؼ� SortedSet�̶�� �ڷᱸ���� �����
    private readonly SortedSet<int> layers = new();

    // StateMachine�� ������
    public EntityType Owner { get; private set; }

    // event ����
    public event StateChangedHandler onStateChanged;
    #endregion

    #region Setup
    public void Setup(EntityType owner)
    {
        Debug.Assert(owner != null, $"StateMachine<{typeof(EntityType).Name}>::Setup - owner�� null�� �� �� �����ϴ�.");

        this.Owner = owner;

        AddStates();
        MakeTransitions();
        SetupLayers();
    }

    // Layer�� ����� Layer���� Current State�� �������ִ� ���ִ� �Լ�
    public void SetupLayers()
    {
        // �� (int, var) : Ʃ�� ���� ��Ī 
        // �� Dictionary�� �� ��Ҹ� �ݺ��ϸ鼭 �� ��Ҹ� Ʃ�� ���·� �����Ѵ�. 
        // �� Ʃ���� (int, var) ���¸� ������, ù ��° ���(Key)�� layer ������ �� ��° ���(Value)�� stateDatasByType ������ �Ҵ�ȴ�. 
        // �� var stateDatasByType : Dictionary<Type, StateData>
        // ����, stateDatasByLayer Dictionary�� �� ��Ҹ� �ݺ��ϸ鼭, �� ����� Ű�� layer ������ ��(Dictionary<Type, StateData>)�� 
        // stateDatasByType ������ �Ҵ��Ѵ�. 
        foreach ((int layer, var stateDatasByType) in stateDatasByLayer)
        {
            // State�� �����ų Layer(Key ����)�� �������
            currentStateDatasByLayer[layer] = null;

            // �ش� Layer���� �켱 ������ ���� ���� StateData�� ã�ƿ�
            // �� stateDatasByType.Values : Dictionary<Type, StateData>�� ����� ��� ������ �����´�. 
            // �� Values.First(x => x.Priority == 0) : ����(���ٽ�)�� �´� ù ��° ��Ҹ� �����ϴ� �޼���
            var firstStateData = stateDatasByType.Values.First(x => x.Priority == 0);

            // ã�ƿ� StateData�� State�� ���� Layer�� Current State�� ��������
            ChangeState(firstStateData);
        }
    }

    // ���ڷ� ���� StateData�� currentStateDatasByLayer�� ����(����)�ϴ� �Լ� 
    private void ChangeState(StateData newStateData)
    {
        // ���ڷ� ���� StateData�� Layer�� �̿��ؼ� Layer���� ���� �������� CurrentStateData�� �����´�.
        var prevState = currentStateDatasByLayer[newStateData.Layer];

        // prevState�� null�� �ƴ϶�� Exit �Լ��� ���� State ����ó�� 
        prevState?.State.Exit();

        // ���� �������� CurrentStateData�� ���ڷ� ���� newStateData�� ��ü����
        currentStateDatasByLayer[newStateData.Layer] = newStateData;
        newStateData.State.Enter();

        // State�� ���̵Ǿ����� �˸�
        onStateChanged?.Invoke(this, newStateData.State, prevState.State, newStateData.Layer);
    }

    // �� �����ε� ChangeState
    // �� layer�� ������ newState�� Type�� �̿��� StateData�� ã�ƿͼ� ���� �������� CurrentStateData�� �����ϴ� �Լ�
    private void ChangeState(State<EntityType> newState, int layer)
    {
        // Layer�� ����� StateDatas�� newState�� ���� StateData�� ã�ƿ�
        var newStateData = stateDatasByLayer[layer][newState.GetType()];
        ChangeState(newStateData);
    }
    #endregion

    #region Transition
    // Transition�� ������ Ȯ���Ͽ� ���̸� �õ��ϴ� �Լ�
    // �� transtions : ���� �õ��� �� transition
    // �� layer : transition�� ���� Layer
    private bool TryTransition(IReadOnlyList<StateTransition<EntityType>> transitions, int layer)
    {
        foreach (var transition in transitions)
        {
            // 1. transition.TransitionCommand != StateTransition<EntityType>.kNullCommand
            // �� transition�� Command�� null�� �ƴ϶�� �Ѿ��. 
            // �� Command�� �����Ѵٸ�, Command�� �޾��� ����(Trigger) ���� �õ��� �ؾ������� �Ѿ
            // 2. !transition.IsTransferable
            // �� ���� ������ �������� ���ϸ� �Ѿ��. 
            if (transition.TransitionCommand != StateTransition<EntityType>.kNullCommand || !transition.IsTransferable)
                continue;

            // CanTrainsitionToSelf�� false �ε�, ToState�� ���� State�̸� �Ѿ��. 
            // �� �ڱ� �ڽ����� ���̰� �Ұ����ѵ��� �ڱ� �ڽ����� �����Ϸ��� �Ѿ��. 
            if (!transition.CanTransitionToSelf && currentStateDatasByLayer[layer].State == transition.ToState)
                continue;

            // ��� ������ �����Ѵٸ� ToState�� ����
            ChangeState(transition.ToState, layer);
            return true;
        }

        return false;
    }
    #endregion

    #region Update
    // Layer�� ���� �۾��� ���� State�� Update �۾��� �Ѵ�. 
    public void Update()
    {
        foreach (var layer in layers)
        {
            // Layer���� �������� ���� StateData�� ������
            var currentStateDate = currentStateDatasByLayer[layer];

            // �ش� Layer�� �ִ� AnyTransitions�� ã�ƿ�
            bool hasAnyTransitions = anyTransitionsByLayer.TryGetValue(layer, out var anyTransition);

            // 1.
            // hasAnyTransitions : AnyTansition�� ����
            // TryTransition(anyTransitions, layer)) : anyTransition���� ���� �õ� 
            // 2. ������ ���� �ʾ� anyTransition���� �������� ���� ���
            // �� ���� StateData�� Transition�� �̿��� ���̸� �õ�
            // �� ���̸� �����ߴٸ� continue�� ���� Layer�� ���� ó���� �Ѿ��.
            if ((hasAnyTransitions && TryTransition(anyTransition, layer)) ||
                TryTransition(currentStateDate.Transitions, layer))
                continue;

            // �������� ���ߴٸ� ���� State�� Update�� ������
            currentStateDate.State.Update();
        }
    }

    public void FixedUpdate()
    {
        foreach (var layer in layers)
        {
            // Layer���� �������� ���� StateData�� ������
            var currentStateDate = currentStateDatasByLayer[layer];

            // Layer���� ���� ���� ���� State�� ������� �ʾҴٸ�, FixedUpdate�� ����
            currentStateDate.State.FixedUpdate();
        }
    }
    #endregion

    /*
                                      �� ���ظ� ���� StateMachine�� �۵� ���� �Ⱦ�� ��
      1. Layer���� ���� ���� ���� StateData �������� 
      2. Layer�� ���� AnyTransitions�� ã�ƿ��� 
      3. ���� AnyTransitions�� �ִٸ� anyTransitions�� ������� TryTransition �Լ��� �����Ѵ�. 
      4. TryTransition �Լ����� ���ڷ� ���� Transitions�� foreach������ ��ȸ�ϱ� 
      5. ����, transition�� Command�� null�� �ƴϰų� ���� ������ �������� ���ߴٸ� continue�� �Ѿ��, 
         �� �� �����ϸ� ���� ���� Ȯ������ ����. 
      6. CanTransitionToSelf�� false�ε�, ������ State�� ���� State�� ���ٸ� continue�� �Ѿ��, �ƴ϶��
         ��� ������ ���������Ƿ� ChangeState �Լ��� ���� State�� ToState�� ���̽�Ų��. 
      7. ChangeState �Լ������� Layer�� State�� Type���� ��� State�� ���� StateData�� ã�ƿ� ChangeState �Լ��� �����Ѵ�. 
      8. ���� ChangeState �Լ����� ���ڷ� ���� StateData�� ������ �̿��� Layer���� ���� ���� ���� StateData�� ã�ƿ��� 
         State�� �������� �ǹ��ϴ� Exit �Լ��� �������ش�. �׸��� ���� Layer���� ���� ���� State Data�� newStateData�� �ٲ��ְ�,
         State�� ���۵����� �ǹ��ϴ� Enter �Լ��� �������ش�. ���������� State�� ���̵Ǿ����� �˸��� onStateChanged Event�� ȣ���Ѵ�. 
      9. �ٽ� Update �Լ��� ���ƿͼ� anyTransitions�� �̿��� ���̿� �����ߴٸ�, ���� StateData�� ���� Transitions�� �̿��ؼ� ���̸�
         �õ��ϰ�(StateData���� �ڽ��� ���� Transitions�� List�� ������ �ִ�), ���̿� �����ߴٸ� continue�� ���� Layer�� �Ѿ��, 
         �����ߴٸ�, ���� State�� Update �Լ��� �������ش�. 
     
      �� ���� ��� 
      �� State�� Update�Ǵٰ� ���� ������ ������ ���̵ǰ�, �ٽ� State�� Update�Ǵٰ� ���� ���ǿ� ������ ���̵Ǵ� ����!
    */

    #region AddState
    // Generic�� ���� StateMachine�� State�� �߰��ϴ� �Լ�
    public void AddState<T>(int layer = 0) where T : State<EntityType>
    {
        // Layer �߰�
        // �� SortedSet Ÿ���̹Ƿ� �̹� Layer�� �����Ѵٸ� �߰����� ����
        //    ���ٸ� �߰��Ǹ鼭 ����� ������������ �ڵ� ���ı��� �ȴ�. 
        layers.Add(layer);

        // �� Activator.CreateInstance : new�� �����ϰ� ��ü�� �����ϴ� ��� 
        // �� new vs Activator.CreateInstance
        // 1. ��Ÿ�ӿ��� Ŭ����Ÿ���� �˰� ������ new Ŭ������ �������ִ� ���� ����. 
        // 2. ���׸� �޼ҵ�� �����Ϸ��� Activator.CreateInstance<T>()�� ����ϴ� �� ����. 
        // �� Activator.CreateInstance�� T Type�� State�� �����Ѵ�
        var newState = Activator.CreateInstance<T>();
        newState.Setup(this, Owner, layer);

        // ���� stateDatasByLayer�� �߰����� ���� Layer��� Layer�� ��������
        if (!stateDatasByLayer.ContainsKey(layer))
        {
            stateDatasByLayer[layer] = new();
            anyTransitionsByLayer[layer] = new();
        }

        // �� Debug.Assert : ������ ������ �� �ߵ�
        // �� stateDatasByLayer[layer].ContainsKey(typeof(T))�� true�� �Ǹ� (T type StateData�� �̹� �����Ѵٸ�)
        // false�� �Ǿ� ���� ���� 
        Debug.Assert(!stateDatasByLayer[layer].ContainsKey(typeof(T)),
            $"StateMachine::AddState<{typeof(T).Name}> - �̹� ���°� �����մϴ�.");

        // ������ ���ο� State��� stateDatasByLayer[layer][typeof(T)]�� StateData�� ���� �߰��Ѵ�. 
        var stateDataByType = stateDatasByLayer[layer];
        // �켱������ Add�� ����(stateDataByType.Count)�̴�. 
        stateDataByType[typeof(T)] = new StateData(layer, stateDataByType.Count, newState);
    }
    #endregion

    #region MakeTransition
    // Transition�� �����ϴ� �Լ�
    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
    {
        // Layer�� StateDatas Dictionary �������� 
        var stateDatas = stateDatasByLayer[layer];

        // StateDatas Dictionary���� FromStateType�� Type�� StateData�� ã�ƿ�
        var fromStateData = stateDatas[typeof(FromStateType)];
        // StateDatas Dictionary���� ToStateType�� Type�� StateData�� ã�ƿ�
        var toStateData = stateDatas[typeof(ToStateType)];

        // StateTransition ���� 
        // �� AnyTransition�� �ƴ� �Ϲ� Transition�� canTransitionToSelf ���ڰ� ������ true (canTransitionToSelf ��� ����)
        var newTransition = new StateTransition<EntityType>(fromStateData.State, toStateData.State,
            transitionCommand, transitionCondition, true);

        // ������ Transition�� FromStateData�� Transition���� �߰�
        fromStateData.Transitions.Add(newTransition);
    }

    // �� MakeTransition �Լ��� Overloading

    // MakeTransition �Լ��� Enum Command ����
    // �� Enum������ ���� Command�� Int�� ��ȯ�Ͽ� ���� �Լ��� ȣ����
    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => MakeTransition<FromStateType, ToStateType>(Convert.ToInt32(transitionCommand), transitionCondition, layer);

    // MakeTransition �Լ��� Command ���ڰ� ���� ����
    // �� NullCommand�� �־ �ֻ���� MakeTransition �Լ��� ȣ����
    public void MakeTransition<FromStateType, ToStateType>(Func<State<EntityType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => MakeTransition<FromStateType, ToStateType>(StateTransition<EntityType>.kNullCommand, transitionCondition, layer);

    // MakeTransition �Լ��� Condition ���ڰ� ���� ����
    // �� Condition���� null�� �־ �ֻ���� MakeTransition �Լ��� ȣ���� 
    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => MakeTransition<FromStateType, ToStateType>(transitionCommand, null, layer);

    // �� �Լ��� Enum ����(Command ���ڰ� Enum���̰� Condition ���ڰ� ����)
    // �� ���� ���ǵ� Enum���� MakeTransition �Լ��� ȣ����
    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => MakeTransition<FromStateType, ToStateType>(transitionCommand, null, layer);
    #endregion

    #region MakeAnyTransition
    // AnyTransition�� ����� �Լ�
    // �� fromState Type�� ���ٴ� �� ���� ��ü������ MakeTransition �Լ��� �� �ٸ��� �ʴ�. 
    // �� �ٸ�, canTransitonToSelf Option�� �׻� true�� ����� MakeTransition�� �޸�, MakeAnyTransition��
    //    canTransitonToSelf Option�� ���ڷ� �޴´�. 
    public void MakeAnyTransition<ToStateType>(int transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<EntityType>
    {
        var stateDatasByType = stateDatasByLayer[layer];

        // StateDatas���� ToStateType�� State�� ���� StateData�� ã�ƿ�
        var state = stateDatasByType[typeof(ToStateType)].State;

        // Transition ����(new), �������� ���Ǹ� ������ ������ ���̹Ƿ� FromState�� �������� ����(null)
        var newTransition = new StateTransition<EntityType>(null, state, transitionCommand, transitionCondition, canTransitionToSelf);

        // Layer�� AnyTransition���� �߰�
        anyTransitionsByLayer[layer].Add(newTransition);
    }

    // �� MakeAnyTransition �Լ��� Overloading

    // MakeAnyTransition �Լ��� Enum Command ����
    // Enum������ ���� Command�� Int�� ��ȯ�Ͽ� ���� �Լ��� ȣ����
    public void MakeAnyTransition<ToStateType>(Enum transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0, bool canTransitonToSelf = false)
        where ToStateType : State<EntityType>
        => MakeAnyTransition<ToStateType>(Convert.ToInt32(transitionCommand), transitionCondition, layer, canTransitonToSelf);

    // MakeAnyTransition �Լ��� Command ���ڰ� ���� ����
    // NullCommand�� �־ �ֻ���� MakeTransition �Լ��� ȣ����
    public void MakeAnyTransition<ToStateType>(Func<State<EntityType>, bool> transitionCondition,
        int layer = 0, bool canTransitonToSelf = false)
        where ToStateType : State<EntityType>
        => MakeAnyTransition<ToStateType>(StateTransition<EntityType>.kNullCommand, transitionCondition, layer, canTransitonToSelf);

    // MakeAnyTransiiton�� Condition ���ڰ� ���� ����
    // Condition���� null�� �־ �ֻ���� MakeTransition �Լ��� ȣ���� 
    public void MakeAnyTransition<ToStateType>(int transitionCommand, int layer = 0, bool canTransitonToSelf = false)
    where ToStateType : State<EntityType>
        => MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitonToSelf);

    // �� �Լ��� Enum ����(Command ���ڰ� Enum���̰� Condition ���ڰ� ����)
    // ���� ���ǵ� Enum���� MakeAnyTransition �Լ��� ȣ����
    public void MakeAnyTransition<ToStateType>(Enum transitionCommand, int layer = 0, bool canTransitonToSelf = false)
        where ToStateType : State<EntityType>
        => MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitonToSelf);
    #endregion

    #region Command
    // Command�� �޾Ƽ� �ش� Command�� ���� Transition�� �����ϴ� �Լ�
    public bool ExecuteCommand(int transitionCommand, int layer)
    {
        // AnyTransition���� Command�� ��ġ�ϰ�, ���� ������ �����ϴ� Transiton�� ã�ƿ�
        var transition = anyTransitionsByLayer[layer].Find(x => x.TransitionCommand == transitionCommand
                                                                && x.IsTransferable);

        // �� ??= : ���� �ǿ����ڰ� null�� ���Ǵ� ��쿡�� ������ �ǿ������� ���� ���� �ǿ����ڿ� ����
        // �� AnyTransition���� Transtion�� �� ã�ƿͼ� null�̶�� ���� �������� CurrentStateData�� Transitions����
        //    Command�� ��ġ�ϰ�, ���� ������ �����ϴ� Transition�� ã�ƿ�
        transition ??= currentStateDatasByLayer[layer].Transitions.Find(x => x.TransitionCommand == transitionCommand
                                                                            && x.IsTransferable);
        // ������ Transtion�� ã�ƿ��� ���ߴٸ� ��� ������ ����
        if (transition == null)
            return false;

        // ������ Transiton�� ã�ƿԴٸ� �ش� Transition�� ToState�� ����
        ChangeState(transition.ToState, layer);
        return true;
    }

    // ExecuteCommand�� Enum Command ����
    public bool ExecuteCommand(Enum transitionCommand, int layer)
        => ExecuteCommand(Convert.ToInt32(transitionCommand), layer);

    // ��� Layer�� ������� ExecuteCommand �Լ��� �����ϴ� �Լ�
    // �� �ϳ��� Layer�� ���̿� �����ϸ� true�� ��ȯ 
    public bool ExecuteCommand(int transitionCommand)
    {
        bool isSuccess = false;

        foreach (var layer in layers)
        {
            if (ExecuteCommand(transitionCommand, layer))
                isSuccess = true;
        }

        return isSuccess;
    }

    // �� ExecuteCommand �Լ��� Enum Command ����
    public bool ExecuteCommand(Enum transitionCommand)
        => ExecuteCommand(Convert.ToInt32(transitionCommand));
    #endregion

    #region Message
    // ���� �������� CurrentStateData�� Message�� ������ �Լ�
    // �� Message�� �ʿ��ϴٸ� �߰� Data�� ���� �������� State�� OnReceiveMessage �Լ��� �Ѱ��ش�. 
    // �� OnReceiveMessage �Լ����� Message�� ���� State �ڽ��� �ؾ� �� ���� �� ���̴�. 
    //    (���� State�� ������ ���� Message�� �Ѿ�´ٸ� �׳� ������ ���� �ִ�.)
    // �� ó�� ����� OnReceiveMessage�� ����� �״�� return�ؼ� Message ó�� ����� �ܺο� �˷��ش�. 
    public bool SendMessage(int message, int layer, object extraData = null)
        => currentStateDatasByLayer[layer].State.OnReceiveMessage(message, extraData);


    // SendMessage �Լ��� Enum Message ����
    public bool SendMessage(Enum message, int layer, object extraData = null)
        => SendMessage(Convert.ToInt32(message), layer, extraData);

    // ��� Layer�� ���� �������� CurrentStateData�� ������� SendMessage �Լ��� �����ϴ� �Լ�
    // �ϳ��� CurrentStateData�� ������ Message�� �����ߴٸ� true�� ��ȯ
    public bool SendMessage(int message, object extraData = null)
    {
        bool isSuccess = false;

        foreach (var layer in layers)
        {
            if (SendMessage(message, layer, extraData))
                isSuccess = true;
        }

        return isSuccess;
    }

    // �� SendMessage �Լ��� Enum Message ����
    public bool SendMessage(Enum message, object extraData = null)
        => SendMessage(Convert.ToInt32(message), extraData);

    // ��� Layer�� ���� �������� CurrentState�� Ȯ���Ͽ�, ���� State�� T Type�� State���� Ȯ���ϴ� �Լ�
    // �� CurrentState�� T Type�ΰ� Ȯ�εǸ� ��� true�� ��ȯ��
    #endregion

    #region Get State & Type
    public bool IsInState<T>() where T : State<EntityType>
    {
        // �� _ : discard
        // �� layer key ���� ������, ����� ��ȹ�� ���� ������(�ش� ���� ���� ����)
        //    _ variable�� discard�Ѵ�.
        foreach ((_, StateData state) in currentStateDatasByLayer)
        {
            if (state.State.GetType() == typeof(T))
                return true;
        }
        return false;
    }

    // Ư�� Layer(���� ��)�� ������� �������� CurrentState�� T Type���� Ȯ���ϴ� �Լ�
    public bool IsInState<T>(int layer) where T : State<EntityType>
        => currentStateDatasByLayer[layer].State.GetType() == typeof(T);

    // Layer�� ���� �������� State�� ������
    public State<EntityType> GetCurrentState(int layer = 0) => currentStateDatasByLayer[layer].State;

    // Layer�� ���� �������� State�� Type�� ������
    public Type GetCurrentStateType(int layer = 0) => currentStateDatasByLayer[layer].State.GetType();
    #endregion

    #region StateMachine.Setup �Լ����� ���
    // �� StateMachine�� ��ӹ޴� Class���� �� �� �Լ��� override�Ͽ� 
    //    State�� �����ϴ� AddStates �Լ��� Transition�� �����ϴ� MakeTransitions �Լ���
    //    �ۼ��س����� �� �� �Լ��� Setup���� ����Ǹ鼭 StateMachine�� ������ State�� transition�� �����ȴ�. 

    // �� �ڽ� class���� ������ State �߰� �Լ�
    // �� �� �Լ� �ȿ��� AddState �Լ��� ����� State�� �߰����ָ�� (�ڡڡ� AddStates�� AddState �Լ��� �ٸ� �Ŵ� �ڡڡ�)
    // �� �߰��� State�� stateDatasByLayer ������ ����ȴ�.
    protected virtual void AddStates() { }

    // �� �ڽ� class���� ������ Transition ���� �Լ�
    // �� �� �Լ����� MakeTransition �Լ��� ����� Transition�� ������ָ� ��
    protected virtual void MakeTransitions() { }
    #endregion
}
