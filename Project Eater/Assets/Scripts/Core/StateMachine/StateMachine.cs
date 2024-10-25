using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ※ StateMachine 기능
/* 1.  State가 전이되었음을 알림 (Event)
   2.  Layer 별 StateData들을 저장
   3.  Layer 별 AnyTransition들을 저장
   4.  Layer 별 현재 실행중인 StateData를 저장
   5.  Layer 저장
   6.  StateMachine Setup 설정 (State, Transition, Layer)
   7.  현재 State 전이 기능
   8.  전이를 시도하는 기능 (현재 State 전이 때 사용)
   9.  Update 기능 (StateMahcine 전이를 계속 시도하면서 전이 조건을 만족하면 전이 or 현재 State의 Update를 실행)
   10. State를 생성하여 StateMachine에 넣는 기능 
   11. Transition을 생성하여 State에 연결시켜주기(romStateData.Transitions.Add(newTransition))
   12. Any Transition을 생성하여 StateMachine에 등록하기(anyTransitionsByLayer[layer].Add(newTransition))
   13. Command를 받아서 해당 Command를 가진 Transition을 실행하는 기능
   14. 현재 실행중인 CurrentStateData에 Message를 보내는 기능 ()
   15. 현재 실행중인 CurrentState를 확인하여, 현재 State가 T Type의 State인지 확인하는 기능
   16. 현재 실행중인 State의 Type을 가져오는 기능
*/
public class StateMachine<EntityType>
{
    #region Event
    // State가 전이되었음을 알리는 Event
    public delegate void StateChangedHandler(StateMachine<EntityType> stateMachine, // 현재 State
                                             State<EntityType> newState,
                                             State<EntityType> prevState,
                                             int layer); // State가 속한 Layer 번호 
    #endregion

    #region Variable
    // StateData Class : State와 State의 여러 부가 정보를 담고 있다.
    private class StateData
    {
        // State가 실행되는 Layer
        public int Layer { get; private set; }

        // State가 Layer에 등록된 순서
        public int Priority { get; private set; }

        // 실제 State 정보
        public State<EntityType> State { get; private set; }

        // 위의 State가 from인 Transitions List
        public List<StateTransition<EntityType>> Transitions { get; private set; } = new();

        // 생성자
        public StateData(int layer, int priority, State<EntityType> state)
            => (Layer, Priority, State) = (layer, priority, state); // 해당 문법 기억하기!
    }

    // Layer별 가지고 있는 StateDatas(=Layer Dictionary)
    // → int Key : StateData가 속한 Layer 번호 
    // Value Dictionary의 Key는 Value인 StateData가 가진 State의 Type
    // → 즉, State의 Type을 통해 해당 StateData(State)를 쉽게 찾아올 수 있음
    private readonly Dictionary<int, Dictionary<Type, StateData>> stateDatasByLayer = new();

    // Layer별 Any Tansitions(조건만 만족하면 언제든지 ToState로 전이되는 Transition)
    // → 해당 Layer에 속해 있는 모든 State는 Any Tansition으로 전이할 수 있다. 즉, 해당 Layer의 State가 모두 공동으로 가지고 있는
    //    Transitions이다. (전이 조건도 동일)
    private readonly Dictionary<int, List<StateTransition<EntityType>>> anyTransitionsByLayer = new();

    // Layer별 현재 실행중인 StateData(=현재 실행중인 State)
    private readonly Dictionary<int, StateData> currentStateDatasByLayer = new();

    // StateMachine에 존재하는 Layer들, Layer는 중복되지 않아야 하고, 자동 정렬을 위해서 SortedSet이라는 자료구조를 사용함
    private readonly SortedSet<int> layers = new();

    // StateMachine의 소유주
    public EntityType Owner { get; private set; }

    // event 선언
    public event StateChangedHandler onStateChanged;
    #endregion

    #region Setup
    public void Setup(EntityType owner)
    {
        Debug.Assert(owner != null, $"StateMachine<{typeof(EntityType).Name}>::Setup - owner는 null이 될 수 없습니다.");

        this.Owner = owner;

        AddStates();
        MakeTransitions();
        SetupLayers();
    }

    // Layer를 만들고 Layer별로 Current State를 설정해주는 해주는 함수
    public void SetupLayers()
    {
        // ※ (int, var) : 튜플 패턴 매칭 
        // → Dictionary의 각 요소를 반복하면서 각 요소를 튜플 형태로 추출한다. 
        // → 튜플은 (int, var) 형태를 가지며, 첫 번째 요소(Key)는 layer 변수에 두 번째 요소(Value)는 stateDatasByType 변수에 할당된다. 
        // → var stateDatasByType : Dictionary<Type, StateData>
        // 따라서, stateDatasByLayer Dictionary의 각 요소를 반복하면서, 각 요소의 키를 layer 변수에 값(Dictionary<Type, StateData>)을 
        // stateDatasByType 변수에 할당한다. 
        foreach ((int layer, var stateDatasByType) in stateDatasByLayer)
        {
            // State를 실행시킬 Layer(Key 값만)를 만들어줌
            currentStateDatasByLayer[layer] = null;

            // 해당 Layer에서 우선 순위가 가장 높은 StateData를 찾아옴
            // → stateDatasByType.Values : Dictionary<Type, StateData>에 저장된 모든 값들을 가져온다. 
            // → Values.First(x => x.Priority == 0) : 조건(람다식)에 맞는 첫 번째 요소를 선택하는 메서드
            var firstStateData = stateDatasByType.Values.First(x => x.Priority == 0);

            // 찾아온 StateData의 State를 현재 Layer의 Current State로 설정해줌
            ChangeState(firstStateData);
        }
    }

    // 인자로 받은 StateData를 currentStateDatasByLayer로 설정(전이)하는 함수 
    private void ChangeState(StateData newStateData)
    {
        // 인자로 받은 StateData의 Layer를 이용해서 Layer에서 현재 실행중인 CurrentStateData를 가져온다.
        var prevState = currentStateDatasByLayer[newStateData.Layer];

        // prevState가 null이 아니라면 Exit 함수를 통해 State 종료처리 
        prevState?.State.Exit();

        // 현재 실행중인 CurrentStateData를 인자로 받은 newStateData로 교체해줌
        currentStateDatasByLayer[newStateData.Layer] = newStateData;
        newStateData.State.Enter();

        // State가 전이되었음을 알림
        onStateChanged?.Invoke(this, newStateData.State, prevState.State, newStateData.Layer);
    }

    // ※ 오버로딩 ChangeState
    // → layer와 변경할 newState의 Type을 이용해 StateData를 찾아와서 현재 실행중인 CurrentStateData를 변경하는 함수
    private void ChangeState(State<EntityType> newState, int layer)
    {
        // Layer에 저장된 StateDatas중 newState를 가진 StateData를 찾아옴
        var newStateData = stateDatasByLayer[layer][newState.GetType()];
        ChangeState(newStateData);
    }
    #endregion

    #region Transition
    // Transition의 조건을 확인하여 전이를 시도하는 함수
    // ※ transtions : 전이 시도를 할 transition
    // ※ layer : transition이 속한 Layer
    private bool TryTransition(IReadOnlyList<StateTransition<EntityType>> transitions, int layer)
    {
        foreach (var transition in transitions)
        {
            // 1. transition.TransitionCommand != StateTransition<EntityType>.kNullCommand
            // → transition의 Command가 null이 아니라면 넘어간다. 
            // → Command가 존재한다면, Command를 받았을 때만(Trigger) 전이 시도를 해야함으로 넘어감
            // 2. !transition.IsTransferable
            // → 전이 조건을 만족하지 못하면 넘어간다. 
            if (transition.TransitionCommand != StateTransition<EntityType>.kNullCommand || !transition.IsTransferable)
                continue;

            // CanTrainsitionToSelf가 false 인데, ToState가 현재 State이면 넘어간다. 
            // → 자기 자신으로 전이가 불가능한데도 자기 자신으로 전이하려면 넘어간다. 
            if (!transition.CanTransitionToSelf && currentStateDatasByLayer[layer].State == transition.ToState)
                continue;

            // 모든 조건을 만족한다면 ToState로 전이
            ChangeState(transition.ToState, layer);
            return true;
        }

        return false;
    }
    #endregion

    #region Update
    // Layer별 전이 작업과 현재 State의 Update 작업을 한다. 
    public void Update()
    {
        foreach (var layer in layers)
        {
            // Layer에서 실행중인 현재 StateData를 가져옴
            var currentStateDate = currentStateDatasByLayer[layer];

            // 해당 Layer에 있는 AnyTransitions를 찾아옴
            bool hasAnyTransitions = anyTransitionsByLayer.TryGetValue(layer, out var anyTransition);

            // 1.
            // hasAnyTransitions : AnyTansition이 존재
            // TryTransition(anyTransitions, layer)) : anyTransition으로 전이 시도 
            // 2. 조건이 맞지 않아 anyTransition으로 전이하지 못한 경우
            // → 현재 StateData의 Transition을 이용해 전이를 시도
            // → 전이를 성공했다면 continue로 다음 Layer에 대한 처리로 넘어간다.
            if ((hasAnyTransitions && TryTransition(anyTransition, layer)) ||
                TryTransition(currentStateDate.Transitions, layer))
                continue;

            // 전이하지 못했다면 현재 State의 Update를 실행함
            currentStateDate.State.Update();
        }
    }

    public void FixedUpdate()
    {
        foreach (var layer in layers)
        {
            // Layer에서 실행중인 현재 StateData를 가져옴
            var currentStateDate = currentStateDatasByLayer[layer];

            // Layer에서 실행 중인 현재 State가 종료되지 않았다면, FixedUpdate도 실행
            currentStateDate.State.FixedUpdate();
        }
    }
    #endregion

    /*
                                      ★ 이해를 위해 StateMachine의 작동 과정 훑어보기 ★
      1. Layer에서 현재 실행 중인 StateData 가져오기 
      2. Layer가 가진 AnyTransitions를 찾아오기 
      3. 만약 AnyTransitions가 있다면 anyTransitions를 대상으로 TryTransition 함수를 실행한다. 
      4. TryTransition 함수에서 인자로 받은 Transitions를 foreach문으로 순회하기 
      5. 만약, transition의 Command가 null이 아니거나 전이 조건을 만족하지 못했다면 continue로 넘어가고, 
         둘 다 만족하면 다음 조건 확인으로 간다. 
      6. CanTransitionToSelf가 false인데, 전이할 State가 현재 State와 같다면 continue로 넘어가고, 아니라면
         모든 조건을 만족했으므로 ChangeState 함수로 현재 State를 ToState로 전이시킨다. 
      7. ChangeState 함수에서는 Layer와 State의 Type으로 대상 State를 가진 StateData를 찾아와 ChangeState 함수를 실행한다. 
      8. 원본 ChangeState 함수에서 인자로 받은 StateData의 정보를 이용해 Layer에서 현재 실행 중인 StateData를 찾아오고 
         State가 끝났음을 의미하는 Exit 함수를 실행해준다. 그리고 현재 Layer에서 실행 중인 State Data를 newStateData로 바꿔주고,
         State가 시작됐음을 의미하는 Enter 함수를 실행해준다. 마지막으로 State가 전이되었음을 알리는 onStateChanged Event를 호출한다. 
      9. 다시 Update 함수로 돌아와서 anyTransitions를 이용한 전이에 실패했다면, 현재 StateData가 가진 Transitions를 이용해서 전이를
         시도하고(StateData에는 자신이 가진 Transitions를 List로 가지고 있다), 전이에 성공했다면 continue로 다음 Layer로 넘어가고, 
         실패했다면, 현재 State의 Update 함수를 실행해준다. 
     
      ※ 완전 요약 
      → State가 Update되다가 전이 조건이 맞으면 전이되고, 다시 State가 Update되다가 전이 조건에 맞으면 전이되는 구조!
    */

    #region AddState
    // Generic을 통해 StateMachine에 State를 추가하는 함수
    public void AddState<T>(int layer = 0) where T : State<EntityType>
    {
        // Layer 추가
        // → SortedSet 타입이므로 이미 Layer가 존재한다면 추가되지 않음
        //    없다면 추가되면서 목록이 오름차순으로 자동 정렬까지 된다. 
        layers.Add(layer);

        // ※ Activator.CreateInstance : new와 동일하게 객체를 생성하는 기능 
        // ※ new vs Activator.CreateInstance
        // 1. 런타임에서 클래스타입을 알고 있으면 new 클래스로 생성해주는 것이 좋다. 
        // 2. 제네릭 메소드로 구현하려면 Activator.CreateInstance<T>()를 사용하는 게 좋다. 
        // → Activator.CreateInstance로 T Type의 State를 생성한다
        var newState = Activator.CreateInstance<T>();
        newState.Setup(this, Owner, layer);

        // 아직 stateDatasByLayer에 추가되지 않은 Layer라면 Layer를 생성해줌
        if (!stateDatasByLayer.ContainsKey(layer))
        {
            stateDatasByLayer[layer] = new();
            anyTransitionsByLayer[layer] = new();
        }

        // ※ Debug.Assert : 조건이 거짓일 때 발동
        // → stateDatasByLayer[layer].ContainsKey(typeof(T))이 true가 되면 (T type StateData가 이미 존재한다면)
        // false가 되어 오류 보고 
        Debug.Assert(!stateDatasByLayer[layer].ContainsKey(typeof(T)),
            $"StateMachine::AddState<{typeof(T).Name}> - 이미 상태가 존재합니다.");

        // 완전히 새로운 State라면 stateDatasByLayer[layer][typeof(T)]에 StateData를 만들어서 추가한다. 
        var stateDataByType = stateDatasByLayer[layer];
        // 우선순위는 Add된 순서(stateDataByType.Count)이다. 
        stateDataByType[typeof(T)] = new StateData(layer, stateDataByType.Count, newState);
    }
    #endregion

    #region MakeTransition
    // Transition을 생성하는 함수
    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
    {
        // Layer의 StateDatas Dictionary 가져오기 
        var stateDatas = stateDatasByLayer[layer];

        // StateDatas Dictionary에서 FromStateType의 Type인 StateData를 찾아옴
        var fromStateData = stateDatas[typeof(FromStateType)];
        // StateDatas Dictionary에서 ToStateType의 Type인 StateData를 찾아옴
        var toStateData = stateDatas[typeof(ToStateType)];

        // StateTransition 생성 
        // → AnyTransition이 아닌 일반 Transition은 canTransitionToSelf 인자가 무조건 true (canTransitionToSelf 사용 안함)
        var newTransition = new StateTransition<EntityType>(fromStateData.State, toStateData.State,
            transitionCommand, transitionCondition, true);

        // 생성한 Transition을 FromStateData의 Transition으로 추가
        fromStateData.Transitions.Add(newTransition);
    }

    // ※ MakeTransition 함수의 Overloading

    // MakeTransition 함수의 Enum Command 버전
    // → Enum형으로 받은 Command를 Int로 변환하여 위의 함수를 호출함
    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => MakeTransition<FromStateType, ToStateType>(Convert.ToInt32(transitionCommand), transitionCondition, layer);

    // MakeTransition 함수의 Command 인자가 없는 버전
    // → NullCommand를 넣어서 최상단의 MakeTransition 함수를 호출함
    public void MakeTransition<FromStateType, ToStateType>(Func<State<EntityType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => MakeTransition<FromStateType, ToStateType>(StateTransition<EntityType>.kNullCommand, transitionCondition, layer);

    // MakeTransition 함수의 Condition 인자가 없는 버전
    // → Condition으로 null을 넣어서 최상단의 MakeTransition 함수를 호출함 
    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => MakeTransition<FromStateType, ToStateType>(transitionCommand, null, layer);

    // 위 함수의 Enum 버전(Command 인자가 Enum형이고 Condition 인자가 없음)
    // → 위에 정의된 Enum버전 MakeTransition 함수를 호출함
    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => MakeTransition<FromStateType, ToStateType>(transitionCommand, null, layer);
    #endregion

    #region MakeAnyTransition
    // AnyTransition을 만드는 함수
    // → fromState Type이 없다는 것 빼곤 전체적으로 MakeTransition 함수와 별 다르지 않다. 
    // → 다만, canTransitonToSelf Option을 항상 true로 해줬던 MakeTransition과 달리, MakeAnyTransition은
    //    canTransitonToSelf Option을 인자로 받는다. 
    public void MakeAnyTransition<ToStateType>(int transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<EntityType>
    {
        var stateDatasByType = stateDatasByLayer[layer];

        // StateDatas에서 ToStateType의 State를 가진 StateData를 찾아옴
        var state = stateDatasByType[typeof(ToStateType)].State;

        // Transition 생성(new), 언제든지 조건만 맞으면 전이할 것이므로 FromState는 존재하지 않음(null)
        var newTransition = new StateTransition<EntityType>(null, state, transitionCommand, transitionCondition, canTransitionToSelf);

        // Layer의 AnyTransition으로 추가
        anyTransitionsByLayer[layer].Add(newTransition);
    }

    // ※ MakeAnyTransition 함수의 Overloading

    // MakeAnyTransition 함수의 Enum Command 버전
    // Enum형으로 받은 Command를 Int로 변환하여 위의 함수를 호출함
    public void MakeAnyTransition<ToStateType>(Enum transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0, bool canTransitonToSelf = false)
        where ToStateType : State<EntityType>
        => MakeAnyTransition<ToStateType>(Convert.ToInt32(transitionCommand), transitionCondition, layer, canTransitonToSelf);

    // MakeAnyTransition 함수의 Command 인자가 없는 버전
    // NullCommand를 넣어서 최상단의 MakeTransition 함수를 호출함
    public void MakeAnyTransition<ToStateType>(Func<State<EntityType>, bool> transitionCondition,
        int layer = 0, bool canTransitonToSelf = false)
        where ToStateType : State<EntityType>
        => MakeAnyTransition<ToStateType>(StateTransition<EntityType>.kNullCommand, transitionCondition, layer, canTransitonToSelf);

    // MakeAnyTransiiton의 Condition 인자가 없는 버전
    // Condition으로 null을 넣어서 최상단의 MakeTransition 함수를 호출함 
    public void MakeAnyTransition<ToStateType>(int transitionCommand, int layer = 0, bool canTransitonToSelf = false)
    where ToStateType : State<EntityType>
        => MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitonToSelf);

    // 위 함수의 Enum 버전(Command 인자가 Enum형이고 Condition 인자가 없음)
    // 위에 정의된 Enum버전 MakeAnyTransition 함수를 호출함
    public void MakeAnyTransition<ToStateType>(Enum transitionCommand, int layer = 0, bool canTransitonToSelf = false)
        where ToStateType : State<EntityType>
        => MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitonToSelf);
    #endregion

    #region Command
    // Command를 받아서 해당 Command를 가진 Transition을 실행하는 함수
    public bool ExecuteCommand(int transitionCommand, int layer)
    {
        // AnyTransition에서 Command가 일치하고, 전이 조건을 만족하는 Transiton을 찾아옴
        var transition = anyTransitionsByLayer[layer].Find(x => x.TransitionCommand == transitionCommand
                                                                && x.IsTransferable);

        // ※ ??= : 왼쪽 피연산자가 null로 계산되는 경우에만 오른쪽 피연산자의 값을 왼쪽 피연산자에 대입
        // → AnyTransition에서 Transtion을 못 찾아와서 null이라면 현재 실행중인 CurrentStateData의 Transitions에서
        //    Command가 일치하고, 전이 조건을 만족하는 Transition을 찾아옴
        transition ??= currentStateDatasByLayer[layer].Transitions.Find(x => x.TransitionCommand == transitionCommand
                                                                            && x.IsTransferable);
        // 적합한 Transtion을 찾아오지 못했다면 명령 실행은 실패
        if (transition == null)
            return false;

        // 적합한 Transiton을 찾아왔다면 해당 Transition의 ToState로 전이
        ChangeState(transition.ToState, layer);
        return true;
    }

    // ExecuteCommand의 Enum Command 버전
    public bool ExecuteCommand(Enum transitionCommand, int layer)
        => ExecuteCommand(Convert.ToInt32(transitionCommand), layer);

    // 모든 Layer를 대상으로 ExecuteCommand 함수를 실행하는 함수
    // → 하나의 Layer라도 전이에 성공하면 true를 반환 
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

    // 위 ExecuteCommand 함수의 Enum Command 버전
    public bool ExecuteCommand(Enum transitionCommand)
        => ExecuteCommand(Convert.ToInt32(transitionCommand));
    #endregion

    #region Message
    // 현재 실행중인 CurrentStateData에 Message를 보내는 함수
    // → Message와 필요하다면 추가 Data를 현재 실행중인 State에 OnReceiveMessage 함수로 넘겨준다. 
    // → OnReceiveMessage 함수에서 Message에 따라서 State 자신이 해야 할 일을 할 것이다. 
    //    (현재 State와 관련이 없는 Message가 넘어온다면 그냥 무시할 수도 있다.)
    // → 처리 결과는 OnReceiveMessage의 결과를 그대로 return해서 Message 처리 결과를 외부에 알려준다. 
    public bool SendMessage(int message, int layer, object extraData = null)
        => currentStateDatasByLayer[layer].State.OnReceiveMessage(message, extraData);


    // SendMessage 함수의 Enum Message 버전
    public bool SendMessage(Enum message, int layer, object extraData = null)
        => SendMessage(Convert.ToInt32(message), layer, extraData);

    // 모든 Layer의 현재 실행중인 CurrentStateData를 대상으로 SendMessage 함수를 실행하는 함수
    // 하나의 CurrentStateData라도 적절한 Message를 수신했다면 true를 반환
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

    // 위 SendMessage 함수의 Enum Message 버전
    public bool SendMessage(Enum message, object extraData = null)
        => SendMessage(Convert.ToInt32(message), extraData);

    // 모든 Layer의 현재 실행중인 CurrentState를 확인하여, 현재 State가 T Type의 State인지 확인하는 함수
    // → CurrentState가 T Type인게 확인되면 즉시 true를 반환함
    #endregion

    #region Get State & Type
    public bool IsInState<T>() where T : State<EntityType>
    {
        // ※ _ : discard
        // → layer key 값을 받지만, 사용할 계획이 없기 때문에(해당 값을 쓰지 않음)
        //    _ variable로 discard한다.
        foreach ((_, StateData state) in currentStateDatasByLayer)
        {
            if (state.State.GetType() == typeof(T))
                return true;
        }
        return false;
    }

    // 특정 Layer(인자 값)를 대상으로 실행중인 CurrentState가 T Type인지 확인하는 함수
    public bool IsInState<T>(int layer) where T : State<EntityType>
        => currentStateDatasByLayer[layer].State.GetType() == typeof(T);

    // Layer의 현재 실행중인 State를 가져옴
    public State<EntityType> GetCurrentState(int layer = 0) => currentStateDatasByLayer[layer].State;

    // Layer의 현재 실행중인 State의 Type을 가져옴
    public Type GetCurrentStateType(int layer = 0) => currentStateDatasByLayer[layer].State.GetType();
    #endregion

    #region StateMachine.Setup 함수에서 사용
    // → StateMachine을 상속받는 Class에서 이 두 함수를 override하여 
    //    State를 생성하는 AddStates 함수와 Transition을 생성하는 MakeTransitions 함수를
    //    작성해놓으면 이 두 함수가 Setup에서 실행되면서 StateMachine에 실제로 State와 transition이 생성된다. 

    // ※ 자식 class에서 정의할 State 추가 함수
    // → 이 함수 안에서 AddState 함수를 사용해 State를 추가해주면됨 (★★★ AddStates와 AddState 함수는 다른 거다 ★★★)
    // → 추가한 State는 stateDatasByLayer 변수에 저장된다.
    protected virtual void AddStates() { }

    // ※ 자식 class에서 정의할 Transition 생성 함수
    // → 이 함수에서 MakeTransition 함수를 사용해 Transition을 만들어주면 됨
    protected virtual void MakeTransitions() { }
    #endregion
}
