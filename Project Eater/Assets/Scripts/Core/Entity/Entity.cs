using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public enum EntityControlType // Entitiy의 Control 주체를 나타내기 위한 enum
{
    Player,
    AI
}

public class Entity : MonoBehaviour
{
    #region Events
    // Damage를 입었을 때, 호출되는 Event
    // ※ Argument
    // entity     : 대상 entity
    // instigator : 대상 entity를 공격한 entity
    // causer     : 실제 데미지를 입힌 주체 ex) instigator가 쏜 스킬 or 함정
    // damage     : 피해량
    public delegate void TakeDamageHandler(Entity entity, Entity instigator, object causer,  float damage);
    // 죽었을 때, 호출되는 Event
    public delegate void DeadHandler(Entity entity);

    public event TakeDamageHandler onTakeDamage;
    public event DeadHandler onDead;
    #endregion

    [SerializeField]
    private Category[] categories; // 여기서 Category는 적과 아군을 구분하기 위한 용도로 사용됨
    [SerializeField]
    private EntityControlType controlType;

    // ※ 적과 아군 구분 
    // → 보통 호감도 System을 많이 만든다. (이번 강의에서는 간단하게 적과 아군을 구분 짓는다.)

    // socket은 Entity Script를 가진 GameObject의 자식 GameObject를 의미함
    // → 스킬의 발사 위치나, 어떤 특정 위치를 저장해두고 외부에서 찾아오기 위해 존재
    // ex) Fireball이라는 마법을 쏜다고 하면 마법이 나가는 위치가 필요, 해당 위치를 Dictionary에 저장 
    //     그러면 다른 곳에서 이름을 통해서 필요한 위치를 가져올 수 있다. 
    //     → Fireball Skill에서 "손바닥"이라는 문자를 통해 손바닥의 위치를 가져옴
    private Dictionary<string, Transform> socketsByName = new();

    public EntityControlType ControlType => controlType;
    public IReadOnlyList<Category> Categories => categories;

    // Entity가 Player Entity인지 AI Entity인지 쉽게 확인하기 위한 프로퍼티
    public bool IsPlayer => controlType == EntityControlType.Player;

    public Animator Animator { get; private set; }

    public Stats Stats { get; private set; }

    // ※ Stats.HungerStat : Hunger의 경우 Bonus Value를 안 쓰고 DefaultValue만 쓸 것이기 때문 
    public bool IsDead => Stats.HungerStat != null && Mathf.Approximately(Stats.HungerStat.DefaultValue, 100f);

    // 목표 대상으로 Entity가 공격해야하는 Target일 수도 있고, 치유해야하는 Target일 수도 있다.
    public Entity Target { get; set; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();

        Stats = GetComponent<Stats>();
        Stats.SetUp(this);
    }

    #region TakeDamage
    public void IncreaseHunger(Entity instigator, object causer, float damage)
    {
        if (IsDead)
            return;

        float prevValue = Stats.HungerStat.DefaultValue;
        Stats.HungerStat.DefaultValue += damage;

        onTakeDamage?.Invoke(this, instigator, causer, damage);

        if (Mathf.Approximately(Stats.HungerStat.DefaultValue, 100f))
            OnDead();
    }

    private void OnDead()
    {
        onDead?.Invoke(this);
    }
    #endregion

    // root transform의 자식 transform들을 순회하며 이름이 socketName인 GameObject의 Transform을 찾아오는 함수 
    private Transform GetTransformSocket(Transform root, string socketName)
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
}
