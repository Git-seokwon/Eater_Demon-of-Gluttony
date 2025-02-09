using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum BossState { Phase1, Phase2, Phase3 }

public abstract class BossAI : MonoBehaviour
{
    [SerializeField]
    protected Skill[] skills; // ���� ��ų��
    [SerializeField]
    protected float[] PlayerDistanceToUseSkill; // ���� ��ų�� ��Ÿ�
    [SerializeField]
    protected float checkInterval = 0.1f; // �ڷ�ƾ �ֱ� 

    protected Skill[] eqippedSkills; // ������ ��ų�� 
    protected WaitForSeconds waitForSeconds; 
    protected Coroutine bossBattleCoroutine; 
    protected BossEntity entity; 
    protected BossState currentState = BossState.Phase1;

    protected Queue<int> attackQueue = new Queue<int>(); // ������ ���� ���� ť
    protected System.Random random = new System.Random();

    protected virtual void Awake()
    {
        entity = GetComponent<BossEntity>();

        waitForSeconds = new WaitForSeconds(checkInterval);

        eqippedSkills = new Skill[skills.Length];
    }

    protected virtual void OnDisable()
    {
        // ��ų ���� ���� 
        if (skills.Length != 0)
        {
            for (int i = skills.Length - 1; i >= 0; i--)
            {
                entity.SkillSystem.Disarm(eqippedSkills[i]);
                entity.SkillSystem.Unregister(eqippedSkills[i]);
                eqippedSkills[i] = null;
            }
        }

        // ��Ȱ��ȭ ��, OnDead �̺�Ʈ ���� 
        entity.onDead -= OnDead;
    }

    // ���� ���� BossState ���� �Լ� 
    // �� ���� ������ ���� ü�¿� ���� State�� ����
    protected abstract void UpdateState(Entity entity, Entity instigator, object causer, float damage);

    // ���� ���� ���� ���� ���� �Լ� 
    protected abstract void PrepareNextPattern();

    // �������� �Ŵ������� ���͸� ������ ��, �ش� �Լ� ȣ��
    public virtual void SetEnemy(int wave, int stage)
    {
        // ��ų ���� 
        if (skills.Length != 0)
        {
            for (int i = 0; i < skills.Length; i++)
            {
                var clone = entity.SkillSystem.Register(skills[i]);
                eqippedSkills[i] = entity.SkillSystem.Equip(clone);
            }
        }

        // ������ ���� ������, State Update �Լ� ����
        entity.onTakeDamage += UpdateState;
        // ���� ����� �ڷ�ƾ ���� 
        entity.onDead += OnDead;
        // ���� ������ �ʱ�ȭ
        currentState = BossState.Phase1;
        // ù��° ������ ��ų ����
        PrepareNextPattern();

        // ��ų AI ���� 
        bossBattleCoroutine = StartCoroutine(Battle());
    }

    // ���� ���� ��� ó�� 
    protected virtual void OnDead(Entity entity)
    {
        // �̺�Ʈ ���� 
        entity.onTakeDamage -= UpdateState;

        // ���� �ڷ�ƾ ���� 
        if (bossBattleCoroutine != null)
        {
            StopCoroutine(bossBattleCoroutine);
            bossBattleCoroutine = null;
        }

        // ���� ���� ť �ʱ�ȭ 
        attackQueue.Clear();
    }

    // ���� ��ų ���� �Լ� 
    protected virtual void ExecuteNextSkill()
    {
        if (skills.Length == 0) return;

        int skillIndex = attackQueue.Dequeue();
        eqippedSkills[skillIndex].Use();

        if (attackQueue.Count == 0)
            PrepareNextPattern(); // ������ ������ ���ο� ���� �غ�
    }

    // �÷��̾ ��Ÿ��� �ִ� �� üũ�ϴ� �Լ� 
    protected bool IsPlayerInRange()
    {
        // ���� ������ ������ ��Ÿ� üũ�� ���� �ʾƵ� �ȴ�. 
        if (attackQueue.Count == 0) return false;

        // �� attackQueue.Peek() : ���� ���� ��ų�� Index ��ȣ 
        // �� PlayerDistanceToUseSkill[attackQueue.Peek()] : ���� ���� ��ų�� ��Ÿ� 
        return (GameManager.Instance.player.transform.position - transform.position).sqrMagnitude
                < PlayerDistanceToUseSkill[attackQueue.Peek()] * PlayerDistanceToUseSkill[attackQueue.Peek()];
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
}
