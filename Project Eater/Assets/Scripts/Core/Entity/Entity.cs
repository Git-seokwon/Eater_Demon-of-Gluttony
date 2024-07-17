using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public enum EntityControlType // Entitiy�� Control ��ü�� ��Ÿ���� ���� enum
{
    Player,
    AI
}

public class Entity : MonoBehaviour
{
    #region Events
    // Damage�� �Ծ��� ��, ȣ��Ǵ� Event
    // �� Argument
    // entity     : ��� entity
    // instigator : ��� entity�� ������ entity
    // causer     : ���� �������� ���� ��ü ex) instigator�� �� ��ų or ����
    // damage     : ���ط�
    public delegate void TakeDamageHandler(Entity entity, Entity instigator, object causer,  float damage);
    // �׾��� ��, ȣ��Ǵ� Event
    public delegate void DeadHandler(Entity entity);

    public event TakeDamageHandler onTakeDamage;
    public event DeadHandler onDead;
    #endregion

    [SerializeField]
    protected Category[] categories; // ���⼭ Category�� ���� �Ʊ��� �����ϱ� ���� �뵵�� ����
    [SerializeField]
    protected EntityControlType controlType;

    // Entity�� Hunger DeadLine : �ش� ��⵵�� �ѱ�� Entity�� ���ó���� �ȴ�. 
    protected float deadLineHunger = float.MaxValue;

    // �� ���� �Ʊ� ���� 
    // �� ���� ȣ���� System�� ���� �����. (�̹� ���ǿ����� �����ϰ� ���� �Ʊ��� ���� ���´�.)

    // socket�� Entity Script�� ���� GameObject�� �ڽ� GameObject�� �ǹ���
    // �� ��ų�� �߻� ��ġ��, � Ư�� ��ġ�� �����صΰ� �ܺο��� ã�ƿ��� ���� ����
    // ex) Fireball�̶�� ������ ��ٰ� �ϸ� ������ ������ ��ġ�� �ʿ�, �ش� ��ġ�� Dictionary�� ���� 
    //     �׷��� �ٸ� ������ �̸��� ���ؼ� �ʿ��� ��ġ�� ������ �� �ִ�. 
    //     �� Fireball Skill���� "�չٴ�"�̶�� ���ڸ� ���� �չٴ��� ��ġ�� ������
    protected Dictionary<string, Transform> socketsByName = new();

    public EntityControlType ControlType => controlType;
    public IReadOnlyList<Category> Categories => categories;

    // Entity�� Player Entity���� AI Entity���� ���� Ȯ���ϱ� ���� ������Ƽ
    public bool IsPlayer => controlType == EntityControlType.Player;

    public Animator Animator { get; private set; }
    public SpriteRenderer Sprite { get; private set; }
    public new Rigidbody2D rigidbody { get; private set; }

    public Stats Stats { get; private set; }

    // �� Stats.HungerStat : Hunger�� ��� Bonus Value�� �� ���� DefaultValue�� �� ���̱� ���� 
    public bool IsDead => Stats.HungerStat != null && Mathf.Approximately(Stats.HungerStat.DefaultValue, deadLineHunger);

    // ��ǥ ������� Entity�� �����ؾ��ϴ� Target�� ���� �ְ�, ġ���ؾ��ϴ� Target�� ���� �ִ�.
    public Entity Target { get; set; }

    protected virtual void Awake()
    {
        Animator = GetComponent<Animator>();
        Sprite = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();

        Stats = GetComponent<Stats>();
        Stats.SetUp(this);
    }

    #region TakeDamage
    // ������ ó��
    public virtual void TakeDamage(Entity instigator, object causer, float damage)
    {
        if (IsDead)
            return;

        float prevValue = Stats.HungerStat.DefaultValue;
        Stats.HungerStat.DefaultValue -= damage;

        onTakeDamage?.Invoke(this, instigator, causer, damage);
    }

    protected void CallOnDead(Entity entity)
    {
        onDead?.Invoke(entity);
    }
    #endregion

    // root transform�� �ڽ� transform���� ��ȸ�ϸ� �̸��� socketName�� GameObject�� Transform�� ã�ƿ��� �Լ� 
    protected Transform GetTransformSocket(Transform root, string socketName)
    {
        // root�� socketName�� ��� 
        if (root.name == socketName)
            return root;

        foreach (Transform child in root)
        {
            // ����Լ��� ���� �ڽĵ� �߿� socketName�� �ִ��� �˻���
            var socket = GetTransformSocket(child, socketName);
            if (socket)
                return socket;
        }

        // ��ȸ�� �� �ߴµ� Target�� �� ã������ null�� return
        return null;
    }

    // ������ִ� Socket�� �������ų� ��ȸ�� ���� ã�ƿ�
    // �� socketName�� ���ڷ� �޴� GetTransformSocket �Լ��� �����ε�
    public Transform GetTransformSocket(string socketName)
    {
        // dictionary���� socketName�� �˻��Ͽ� �ִٸ� return
        if (socketsByName.TryGetValue(socketName, out var socket))
            return socket;

        // �� transform : �ڱ� �ڽ��� transform
        // dictionary�� �����Ƿ� ��ȸ �˻�
        socket = GetTransformSocket(transform, socketName);
        // socket�� ã���� dictionary�� �����Ͽ� ���Ŀ� �ٽ� �˻��� �ʿ䰡 ������ ��
        if (socket)
            socketsByName[socketName] = socket;
            
        return socket;
    }

    // ���ڷ� ���� Category�� �������� Ȯ���ϴ� �Լ� 
    public bool HasCategory(Category category) => categories.Any(x => x.ID == category.ID);
}
