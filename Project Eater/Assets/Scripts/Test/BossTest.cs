using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Networking.Types;

public class BossTest : MonoBehaviour
{
    [SerializeField]
    protected Skill skill; // ���� ��ų��
    [SerializeField]
    protected float PlayerDistanceToUseSkill; // ���� ��ų�� ��Ÿ�
    [SerializeField]
    protected float checkInterval = 0.1f; // �ڷ�ƾ �ֱ� 

    protected Skill eqippedSkill; // ������ ��ų�� 
    protected WaitForSeconds waitForSeconds;
    protected Coroutine bossBattleCoroutine;
    protected BossEntity entity;

    protected virtual void Awake()
    {
        entity = GetComponent<BossEntity>();

        waitForSeconds = new WaitForSeconds(checkInterval);
    }

    private void Start()
    {
        SetEnemy();
    }

    protected virtual void OnDisable()
    {
        // ��ų ���� ���� 
        if (skill != null)
        {
            entity.SkillSystem.Disarm(eqippedSkill);
            entity.SkillSystem.Unregister(eqippedSkill);
            eqippedSkill = null;
        }

        // ��Ȱ��ȭ ��, OnDead �̺�Ʈ ���� 
        entity.onDead -= OnDead;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            entity.Stats.IncreaseDefaultValue(entity.Stats.FullnessStat, -entity.Stats.FullnessStat.MaxValue);
        }
    }

    public virtual void SetEnemy()
    {
        // ��ų ���� 
        if (skill != null)
        {
            var clone = entity.SkillSystem.Register(skill);
            eqippedSkill = entity.SkillSystem.Equip(clone);
        }

        // ���� ����� �ڷ�ƾ ���� 
        entity.onDead += OnDead;

        // ��ų AI ���� 
        bossBattleCoroutine = StartCoroutine(Battle());
    }

    // ���� ���� ��� ó�� 
    protected virtual void OnDead(Entity entity)
    {
        // ���� �ڷ�ƾ ���� 
        if (bossBattleCoroutine != null)
        {
            StopCoroutine(bossBattleCoroutine);
            bossBattleCoroutine = null;
        }
    }

    // ���� ��ų ��� �õ� �ڷ�ƾ �Լ� 
    protected IEnumerator Battle()
    {
        while (true)
        {
            if (IsPlayerInRange())
                ExecuteNextSkill();

            // ������ �ð� ��ŭ ���
            yield return waitForSeconds;
        }
    }

    // �÷��̾ ��Ÿ��� �ִ� �� üũ�ϴ� �Լ� 
    protected bool IsPlayerInRange()
    { 
        if (skill == null) return false;

        // �� attackQueue.Peek() : ���� ���� ��ų�� Index ��ȣ 
        // �� PlayerDistanceToUseSkill[attackQueue.Peek()] : ���� ���� ��ų�� ��Ÿ� 
        return (GameManager.Instance.player.transform.position - transform.position).sqrMagnitude
                < PlayerDistanceToUseSkill * PlayerDistanceToUseSkill;
    }

    protected virtual void ExecuteNextSkill()
    {
        if (skill == null) return;

        eqippedSkill.Use();
    }
}
