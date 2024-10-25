using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ※ State Class 
// → 추상 Class에서 알 수 있듯이 State Class는 틀만 잡아주고 실제로 해야 할 작업은 자식 Class에서 구현해준다. 
// ※ EntityType : State를 소유하는 Entity의 Type (★★★ 제네릭의 T를 EntityType로 이름만 바꾸어 사용하는 형식이다! ★★★)
// → State Machine을 적용할 어떤 Class든 EntityType으로 들어올 수 있다. 
// ex) Player, Enemy, Skill, Category
public abstract class State<EntityType>
{
    // State의 EntityType과 StateMachine의 EntityType이 동일해야 한다. 
    public StateMachine<EntityType> Owner {  get; private set; }

    // 실제 State와 StateMachine을 소유하는 Entity 객체 
    // ex) Entity의 Type이 Skill이라면 Skill Entity가 된다.
    public EntityType Entity {  get; private set; }

    // State의 Layer 번호
    public int Layer {  get; private set; }

    public void Setup(StateMachine<EntityType> owner, EntityType entity, int layer)
    {
        this.Owner = owner;
        this.Entity = entity;
        this.Layer = layer;

        Setup();
    }

    // Awake 역할을 해줄 Setup 함수
    protected virtual void Setup() { }

    // State가 시작될 때 실행될 함수
    public virtual void Enter() { }

    // State이가 실행중일 때 매 프레임마다 실행되는 함수
    public virtual void Update() { }

    // State이가 실행중일 때 매 물리 프레임마다 실행되는 함수
    public virtual void FixedUpdate() { }

    // State가 끝날 때 실행될 함수
    public virtual void Exit() { }

    // StateMachine을 통해 외부에서 Message가 넘어왔을 때 처리하는 함수
    // ex) SleepingState일 때, 외부에서 "일어나!"라는 Message가 넘어오면
    //     그것에 대한 처리를 해주는 함수  
    // Message라는건 State에게 특정 작업을 하라고 명령하기 위해 개발자가 정한 신호
    // → enum을 통해 Message를 정의할 것이다. 
    public virtual bool OnReceiveMessage(int message, object data) => false;
}
