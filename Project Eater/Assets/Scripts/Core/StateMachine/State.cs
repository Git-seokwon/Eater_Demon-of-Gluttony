using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� State Class 
// �� �߻� Class���� �� �� �ֵ��� State Class�� Ʋ�� ����ְ� ������ �ؾ� �� �۾��� �ڽ� Class���� �������ش�. 
// �� EntityType : State�� �����ϴ� Entity�� Type (�ڡڡ� ���׸��� T�� EntityType�� �̸��� �ٲپ� ����ϴ� �����̴�! �ڡڡ�)
// �� State Machine�� ������ � Class�� EntityType���� ���� �� �ִ�. 
// ex) Player, Enemy, Skill, Category
public abstract class State<EntityType>
{
    // State�� EntityType�� StateMachine�� EntityType�� �����ؾ� �Ѵ�. 
    public StateMachine<EntityType> Owner {  get; private set; }

    // ���� State�� StateMachine�� �����ϴ� Entity ��ü 
    // ex) Entity�� Type�� Skill�̶�� Skill Entity�� �ȴ�.
    public EntityType Entity {  get; private set; }

    // State�� Layer ��ȣ
    public int Layer {  get; private set; }

    public void Setup(StateMachine<EntityType> owner, EntityType entity, int layer)
    {
        this.Owner = owner;
        this.Entity = entity;
        this.Layer = layer;

        Setup();
    }

    // Awake ������ ���� Setup �Լ�
    protected virtual void Setup() { }

    // State�� ���۵� �� ����� �Լ�
    public virtual void Enter() { }

    // State�̰� �������� �� �� �����Ӹ��� ����Ǵ� �Լ�
    public virtual void Update() { }

    // State�̰� �������� �� �� ���� �����Ӹ��� ����Ǵ� �Լ�
    public virtual void FixedUpdate() { }

    // State�� ���� �� ����� �Լ�
    public virtual void Exit() { }

    // StateMachine�� ���� �ܺο��� Message�� �Ѿ���� �� ó���ϴ� �Լ�
    // ex) SleepingState�� ��, �ܺο��� "�Ͼ!"��� Message�� �Ѿ����
    //     �װͿ� ���� ó���� ���ִ� �Լ�  
    // Message��°� State���� Ư�� �۾��� �϶�� ����ϱ� ���� �����ڰ� ���� ��ȣ
    // �� enum�� ���� Message�� ������ ���̴�. 
    public virtual bool OnReceiveMessage(int message, object data) => false;
}
