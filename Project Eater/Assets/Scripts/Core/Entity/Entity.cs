using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static UnityEngine.EventSystems.EventTrigger;

// Entitiy의 Control 주체를 나타내기 위한 enum
public enum EntityControlType 
{
    Player,
    AI
}

public abstract class Entity : MonoBehaviour
{
    #region Events
    // Damage를 입었을 때, 호출되는 Event
    // ※ Argument
    // entity     : 대상 entity
    // instigator : 대상 entity를 공격한 entity
    // causer     : 실제 데미지를 입힌 주체 ex) instigator가 쏜 스킬 or 함정
    // damage     : 피해량
    public delegate void TakeDamageHandler(Entity entity, Entity instigator, object causer,  float damage, 
        bool isCrit, bool isHitImpactOn);
    // 죽었을 때, 호출되는 Event
    public delegate void DeadHandler(Entity entity, bool isRealDead = true);
    // 적에게 기본 공격을 적중했을 때, 호출되는 Event
    public delegate void DealBasicDamageHandler(object causer, Entity target, float damage);
    // 적을 처치했을 때, 호출되는 Event
    // ※ instigator : 대상 Entity를 공격한 Entity
    public delegate void KillHandler(Entity instigator, object causer, Entity target);
    public delegate void SelfDestructHandler();

    public event TakeDamageHandler onTakeDamage;
    public event DeadHandler onDead;
    public event DealBasicDamageHandler onDealBasicDamage;
    public event KillHandler onKilled;
    public event SelfDestructHandler onSelfDestruct;
    #endregion

    // ※ 적과 아군 구분 
    // → 보통 호감도 System을 많이 만든다. (여기서 Category는 적과 아군을 구분하기 위한 용도로 사용됨)
    [SerializeField]
    protected Category[] categories;
    [SerializeField]
    protected EntityControlType controlType;
    [SerializeField]
    protected bool isSelfDestructive;

    // socket은 Entity Script를 가진 GameObject의 자식 GameObject를 의미함
    // → 스킬의 발사 위치나, 어떤 특정 위치를 저장해두고 외부에서 찾아오기 위해 존재
    // ex) Fireball이라는 마법을 쏜다고 하면 마법이 나가는 위치가 필요, 해당 위치를 Dictionary에 저장 
    //     그러면 다른 곳에서 이름을 통해서 필요한 위치를 가져올 수 있다. 
    //     → Fireball Skill에서 "손바닥"이라는 문자를 통해 손바닥의 위치를 가져옴
    protected Dictionary<string, Transform> socketsByName = new();

    public EntityControlType ControlType => controlType;
    public IReadOnlyList<Category> Categories => categories;

    // Entity가 Player Entity인지 AI Entity인지 쉽게 확인하기 위한 프로퍼티
    public bool IsPlayer => controlType == EntityControlType.Player;

    public Animator Animator { get; protected set; }
    public SpriteRenderer Sprite { get; protected set; }
    public new Rigidbody2D rigidbody { get; protected set; }
    public Collider2D Collider { get; protected set; }

    // 1) 1  : 왼쪽 
    // 2) -1 : 오른쪽
    public int EntitytSight
    {
        get => transform.localScale.x > 0f ? -1 : 1;
    }

    public Stats Stats { get; private set; }

    // ※ Stats.FullnessStat : Fullness의 경우 Bonus Value를 안 쓰고 DefaultValue만 쓸 것이기 때문 
    public bool IsDead => Stats.FullnessStat != null && Mathf.Approximately(Stats.FullnessStat.DefaultValue, 0f);
    public bool IsSelfDestructive => isSelfDestructive;

    public SkillSystem SkillSystem { get; private set; }

    // 목표 대상으로 Entity가 공격해야하는 Target일 수도 있고, 치유해야하는 Target일 수도 있다.
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
        // 플레이어로부터 받고 있던 모든 Effect 효과들 해제 
        SkillSystem.RemoveEffectAll();

        onTakeDamage += PlayHitImpact;

        Collider.enabled = true;
    }

    protected virtual void OnDisable()
    {
        StopBleedingEffect();

        // event는 내부에서만 초기화가 가능하다.
        // (자식 클래스라 할지라도 초기화할 수 없다)
        onDead = null;

        onTakeDamage -= PlayHitImpact;
    }

    protected abstract void SetUpMovement();

    protected abstract void SetUpStateMachine();

    #region TakeDamage
    // 데미지 처리
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
            // 망멸의 낫으로 처형된 경우, Animator.speed가 0이 되기 때문에 1로 초기화 한다. 
            if (Mathf.Approximately(Animator.speed, 0f))
                Animator.speed = 1f;

            if (isSelfDestructive)
            {
                // 자폭 몬스터가 스턴 상태일 때, 죽으면 자폭 공격을 하지 않고 일반적인 죽음 처리를 한다. 
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

    // Entity가 피해를 입었을 경우, Hit Impact Animation을 재생하는 코드 
    // → Entity Enable, Disable 시 각각, 구독 및 해제 처리를 한다. 
    private void PlayHitImpact(Entity entity, Entity instigator, object causer, float damage, bool isCrit, bool isHitImpactOn)
    {
        if (!isHitImpactOn) return;

        Vector3 hitPosition = GetHitImpactPosition(entity, instigator);
        // ※ Quaternion.LookRotation : 특정 방향을 바라보는 회전(Quaternion) 값을 생성하는 함수
        // → Quaternion.LookRotation(Vector3 forward, Vector3 up);
        // 1) forward : 바라볼 방향
        // 2) up (선택적, 기본값: Vector3.up) : 위쪽 방향 기준 (월드 좌표 기준)
        // ※ 주의점 
        // Unity는 3D 엔진이 기본이라 Quaternion.LookRotation은 기본적으로 Z축을 기준으로 회전하는 3D 회전 값을 반환
        // 2D 게임에서는 Z축 회전(즉, Quaternion.Euler(0, 0, angle))만 필요하므로 Vector3.forward로 Z축 방향을 고정해야 한다.
        Quaternion hitRotation = Quaternion.LookRotation(Vector3.forward, (entity.transform.position - instigator.transform.position).normalized);

        HelperUtilities.PlayHitImpactAnimation(hitPosition, isCrit, hitRotation);
    }

    private Vector3 GetHitImpactPosition(Entity entity, Entity instigator)
    {
        if (entity.Collider == null)
            return entity.transform.position;

        Vector2 instigatorPos = instigator.transform.position;
        Vector2 hitPosition = entity.Collider.ClosestPoint(instigatorPos); // 가장 가까운 콜라이더 표면 좌표 반환
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

    // root transform의 자식 transform들을 순회하며 이름이 socketName인 GameObject의 Transform을 찾아오는 함수 
    protected Transform GetTransformSocket(Transform root, string socketName)
    {
        // root가 socketName인 경우 
        if (root.name == socketName)
            return root;

        foreach (Transform child in root)
        {
            // 재귀함수를 통해 자식들 중에 socketName이 있는지 검색함
            var socket = GetTransformSocket(child, socketName);
            if (socket)
                return socket;
        }

        // 순회를 다 했는데 Target을 못 찾았으면 null을 return
        return null;
    }

    // 저장되있는 Socket을 가져오거나 순회를 통해 찾아옴
    // → socketName만 인자로 받는 GetTransformSocket 함수의 오버로딩
    public Transform GetTransformSocket(string socketName)
    {
        // dictionary에서 socketName을 검색하여 있다면 return
        if (socketsByName.TryGetValue(socketName, out var socket))
            return socket;

        // ※ transform : 자기 자신의 transform
        // dictionary에 없으므로 순회 검색
        socket = GetTransformSocket(transform, socketName);
        // socket을 찾으면 dictionary에 저장하여 이후에 다시 검색할 필요가 없도록 함
        if (socket)
            socketsByName[socketName] = socket;
            
        return socket;
    }

    // 인자로 받은 Category를 가졌는지 확인하는 함수 
    public bool HasCategory(Category category) => categories.Any(x => x.ID == category.ID);

    public virtual void PlayBleedingEffect() { }

    public virtual void StopBleedingEffect() { }

    protected virtual void ExecutionGrit(ref float damage) { }
}
