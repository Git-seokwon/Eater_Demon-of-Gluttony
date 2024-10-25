using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static UnityEngine.EventSystems.EventTrigger;

// Entitiy�� Control ��ü�� ��Ÿ���� ���� enum
public enum EntityControlType 
{
    Player,
    AI
}

public abstract class Entity : MonoBehaviour
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
    // ������ �⺻ ������ �������� ��, ȣ��Ǵ� Event
    public delegate void DealBasicDamageHandler(object causer, Entity target, float damage);
    // ���� óġ���� ��, ȣ��Ǵ� Event
    // �� instigator : ��� Entity�� ������ Entity
    public delegate void KillHandler(Entity instigator, object causer, Entity target);

    public event TakeDamageHandler onTakeDamage;
    public event DeadHandler onDead;
    public event DealBasicDamageHandler onDealBasicDamage;
    public event KillHandler onKill;
    #endregion

    // �� ���� �Ʊ� ���� 
    // �� ���� ȣ���� System�� ���� �����. (���⼭ Category�� ���� �Ʊ��� �����ϱ� ���� �뵵�� ����)
    [SerializeField]
    protected Category[] categories;
    [SerializeField]
    protected EntityControlType controlType;

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
    public Collider2D Collider { get; private set; }

    // 1) 1  : ������ 
    // 2) -1 : ���� 
    public int EntitytSight { get; private set; }

    public Stats Stats { get; private set; }

    // �� Stats.FullnessStat : Fullness�� ��� Bonus Value�� �� ���� DefaultValue�� �� ���̱� ���� 
    public bool IsDead => Stats.FullnessStat != null && Mathf.Approximately(Stats.FullnessStat.DefaultValue, 0f);

    public SkillSystem SkillSystem { get; private set; }

    // ��ǥ ������� Entity�� �����ؾ��ϴ� Target�� ���� �ְ�, ġ���ؾ��ϴ� Target�� ���� �ִ�.
    public Entity Target { get; set; }

    protected EffectAnimation effectAnimation;

    protected virtual void Awake()
    {
        Animator = GetComponent<Animator>();
        Sprite = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        effectAnimation = GetComponent<EffectAnimation>();
        Collider = GetComponent<Collider2D>();

        Stats = GetComponent<Stats>();
        Stats.SetUp(this);

        SetUpMovement();

        SetUpStateMachine();

        SkillSystem = GetComponent<SkillSystem>();
        SkillSystem?.Setup(this);
    }

    protected virtual void Update()
    {
        // ���� : -1 / ������ : 1
        EntitytSight = transform.localScale.x > 0f ? -1 : 1;
    }

    protected virtual void FixedUpdate()
    {

    }

    protected abstract void SetUpMovement();

    protected abstract void SetUpStateMachine();

    #region TakeDamage
    // ������ ó��
    public virtual void TakeDamage(Entity instigator, object causer, float damage, bool isTrueDamage = false, bool isTakeDamageEffect = true)
    {
        if (IsDead)
            return;

        if (isTrueDamage)
            Stats.FullnessStat.DefaultValue -= damage;
        else
            Stats.FullnessStat.DefaultValue -= (damage / Stats.DefenceStat.Value);

        onTakeDamage?.Invoke(this, instigator, causer, damage);

        if (Mathf.Approximately(Stats.FullnessStat.DefaultValue, 0f))
        {
            onKill?.Invoke(instigator, causer, this);
            OnDead();
        }
    }

    public void DealBasicDamage(object causer, Entity target, float damage) => onDealBasicDamage?.Invoke(causer, target, damage);

    private void OnDead()
    {
        StopMovement();

        onDead?.Invoke(this);

        SkillSystem.CancelAll();

        // effectAnimation?.EndEffect();

        SkillSystem.RemoveEffectAll();

        gameObject.SetActive(false);
    }

    protected abstract void StopMovement();
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
