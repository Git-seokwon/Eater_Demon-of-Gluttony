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
    public delegate void TakeDamageHandler(Entity entity, Entity instigator, object causer,  float damage, 
        bool isCrit, bool isHitImpactOn);
    // �׾��� ��, ȣ��Ǵ� Event
    public delegate void DeadHandler(Entity entity, bool isRealDead = true);
    // ������ �⺻ ������ �������� ��, ȣ��Ǵ� Event
    public delegate void DealBasicDamageHandler(object causer, Entity target, float damage);
    // ���� óġ���� ��, ȣ��Ǵ� Event
    // �� instigator : ��� Entity�� ������ Entity
    public delegate void KillHandler(Entity instigator, object causer, Entity target);
    public delegate void SelfDestructHandler();

    public event TakeDamageHandler onTakeDamage;
    public event DeadHandler onDead;
    public event DealBasicDamageHandler onDealBasicDamage;
    public event KillHandler onKilled;
    public event SelfDestructHandler onSelfDestruct;
    #endregion

    // �� ���� �Ʊ� ���� 
    // �� ���� ȣ���� System�� ���� �����. (���⼭ Category�� ���� �Ʊ��� �����ϱ� ���� �뵵�� ����)
    [SerializeField]
    protected Category[] categories;
    [SerializeField]
    protected EntityControlType controlType;
    [SerializeField]
    protected bool isSelfDestructive;

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

    public Animator Animator { get; protected set; }
    public SpriteRenderer Sprite { get; protected set; }
    public new Rigidbody2D rigidbody { get; protected set; }
    public Collider2D Collider { get; protected set; }

    // 1) 1  : ���� 
    // 2) -1 : ������
    public int EntitytSight
    {
        get => transform.localScale.x > 0f ? -1 : 1;
    }

    public Stats Stats { get; private set; }

    // �� Stats.FullnessStat : Fullness�� ��� Bonus Value�� �� ���� DefaultValue�� �� ���̱� ���� 
    public bool IsDead => Stats.FullnessStat != null && Mathf.Approximately(Stats.FullnessStat.DefaultValue, 0f);
    public bool IsSelfDestructive => isSelfDestructive;

    public SkillSystem SkillSystem { get; private set; }

    // ��ǥ ������� Entity�� �����ؾ��ϴ� Target�� ���� �ְ�, ġ���ؾ��ϴ� Target�� ���� �ִ�.
    public Entity Target { get; set; }

    protected virtual void Awake()
    {
        Animator = GetComponent<Animator>();
        Sprite = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
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
        
    }

    protected virtual void FixedUpdate()
    {
        
    }

    protected virtual void OnEnable()
    {
        // �÷��̾�κ��� �ް� �ִ� ��� Effect ȿ���� ���� 
        SkillSystem.RemoveEffectAll();

        onTakeDamage += PlayHitImpact;

        Collider.enabled = true;
    }

    protected virtual void OnDisable()
    {
        StopBleedingEffect();

        // event�� ���ο����� �ʱ�ȭ�� �����ϴ�.
        // (�ڽ� Ŭ������ ������ �ʱ�ȭ�� �� ����)
        onDead = null;

        onTakeDamage -= PlayHitImpact;
    }

    protected abstract void SetUpMovement();

    protected abstract void SetUpStateMachine();

    #region TakeDamage
    // ������ ó��
    public virtual void TakeDamage(Entity instigator, object causer, float damage, bool isCrit, 
        bool isHitImpactOn = true, bool isTrueDamage = false, bool isRealDead = true)
    {
        if (IsDead)
            return;

        ExecutionGrit(ref damage);

        if (isTrueDamage || Mathf.Approximately(Stats.DefenceStat.Value, 0))
            Stats.FullnessStat.DefaultValue -= Mathf.Max(damage, 0,5f);
        else
            Stats.FullnessStat.DefaultValue -= Mathf.Max((damage - (Stats.DefenceStat.Value/2)), 1f);

        onTakeDamage?.Invoke(this, instigator, causer, damage, isCrit, isHitImpactOn);

        if (Mathf.Approximately(Stats.FullnessStat.DefaultValue, 0f))
        {
            Collider.enabled = false;
            // ������ ������ ó���� ���, Animator.speed�� 0�� �Ǳ� ������ 1�� �ʱ�ȭ �Ѵ�. 
            if (Mathf.Approximately(Animator.speed, 0f))
                Animator.speed = 1f;

            if (isSelfDestructive)
            {
                // ���� ���Ͱ� ���� ������ ��, ������ ���� ������ ���� �ʰ� �Ϲ����� ���� ó���� �Ѵ�. 
                if (this is EnemyEntity enemy && enemy.IsInState<EnemyStunningState>())
                {
                    onKilled?.Invoke(instigator, causer, this);
                    Animator.SetBool("IsDead", true);
                    OnDead(isRealDead);
                    return;
                }

                onKilled?.Invoke(instigator, causer, this);
                onSelfDestruct?.Invoke();
                return;
            }

            onKilled?.Invoke(instigator, causer, this);
            OnDead(isRealDead);
        }
    }

    // Entity�� ���ظ� �Ծ��� ���, Hit Impact Animation�� ����ϴ� �ڵ� 
    // �� Entity Enable, Disable �� ����, ���� �� ���� ó���� �Ѵ�. 
    private void PlayHitImpact(Entity entity, Entity instigator, object causer, float damage, bool isCrit, bool isHitImpactOn)
    {
        if (!isHitImpactOn) return;

        Vector3 hitPosition = GetHitImpactPosition(entity, instigator);
        // �� Quaternion.LookRotation : Ư�� ������ �ٶ󺸴� ȸ��(Quaternion) ���� �����ϴ� �Լ�
        // �� Quaternion.LookRotation(Vector3 forward, Vector3 up);
        // 1) forward : �ٶ� ����
        // 2) up (������, �⺻��: Vector3.up) : ���� ���� ���� (���� ��ǥ ����)
        // �� ������ 
        // Unity�� 3D ������ �⺻�̶� Quaternion.LookRotation�� �⺻������ Z���� �������� ȸ���ϴ� 3D ȸ�� ���� ��ȯ
        // 2D ���ӿ����� Z�� ȸ��(��, Quaternion.Euler(0, 0, angle))�� �ʿ��ϹǷ� Vector3.forward�� Z�� ������ �����ؾ� �Ѵ�.
        Quaternion hitRotation = Quaternion.LookRotation(Vector3.forward, (entity.transform.position - instigator.transform.position).normalized);

        HelperUtilities.PlayHitImpactAnimation(hitPosition, isCrit, hitRotation);
    }

    private Vector3 GetHitImpactPosition(Entity entity, Entity instigator)
    {
        if (entity.Collider == null)
            return entity.transform.position;

        Vector2 instigatorPos = instigator.transform.position;
        Vector2 hitPosition = entity.Collider.ClosestPoint(instigatorPos); // ���� ����� �ݶ��̴� ǥ�� ��ǥ ��ȯ
        return hitPosition;
    }

    public void DealBasicDamage(object causer, Entity target, float damage) 
        => onDealBasicDamage?.Invoke(causer, target, damage);

    public virtual void OnDead(bool isReadDead = true)
    {
        StopMovement();

        SkillSystem.CancelAll(true);
        onDead?.Invoke(this, isReadDead);
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

    public virtual void PlayBleedingEffect() { }

    public virtual void StopBleedingEffect() { }

    protected virtual void ExecutionGrit(ref float damage) { }
}
