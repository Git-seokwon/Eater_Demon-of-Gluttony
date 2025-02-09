using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum BossState { Phase1, Phase2, Phase3 }

public abstract class BossAI : MonoBehaviour
{
    [SerializeField]
    protected Skill[] skills; // 보스 스킬들
    [SerializeField]
    protected float[] PlayerDistanceToUseSkill; // 보스 스킬별 사거리
    [SerializeField]
    protected float checkInterval = 0.1f; // 코루틴 주기 

    protected Skill[] eqippedSkills; // 장착된 스킬들 
    protected WaitForSeconds waitForSeconds; 
    protected Coroutine bossBattleCoroutine; 
    protected BossEntity entity; 
    protected BossState currentState = BossState.Phase1;

    protected Queue<int> attackQueue = new Queue<int>(); // 실행할 공격 패턴 큐
    protected System.Random random = new System.Random();

    protected virtual void Awake()
    {
        entity = GetComponent<BossEntity>();

        waitForSeconds = new WaitForSeconds(checkInterval);

        eqippedSkills = new Skill[skills.Length];
    }

    protected virtual void OnDisable()
    {
        // 스킬 장착 해제 
        if (skills.Length != 0)
        {
            for (int i = skills.Length - 1; i >= 0; i--)
            {
                entity.SkillSystem.Disarm(eqippedSkills[i]);
                entity.SkillSystem.Unregister(eqippedSkills[i]);
                eqippedSkills[i] = null;
            }
        }

        // 비활성화 시, OnDead 이벤트 해제 
        entity.onDead -= OnDead;
    }

    // 보스 몬스터 BossState 관리 함수 
    // → 보스 몬스터의 현재 체력에 따라 State가 변함
    protected abstract void UpdateState(Entity entity, Entity instigator, object causer, float damage);

    // 보스 몬스터 공격 패턴 생성 함수 
    protected abstract void PrepareNextPattern();

    // 스테이지 매니저에서 몬스터를 스폰할 때, 해당 함수 호출
    public virtual void SetEnemy(int wave, int stage)
    {
        // 스킬 장착 
        if (skills.Length != 0)
        {
            for (int i = 0; i < skills.Length; i++)
            {
                var clone = entity.SkillSystem.Register(skills[i]);
                eqippedSkills[i] = entity.SkillSystem.Equip(clone);
            }
        }

        // 공격을 당할 때마다, State Update 함수 실행
        entity.onTakeDamage += UpdateState;
        // 몬스터 사망시 코루틴 종료 
        entity.onDead += OnDead;
        // 보스 페이즈 초기화
        currentState = BossState.Phase1;
        // 첫번째 페이즈 스킬 패턴
        PrepareNextPattern();

        // 스킬 AI 시작 
        bossBattleCoroutine = StartCoroutine(Battle());
    }

    // 보스 몬스터 사망 처리 
    protected virtual void OnDead(Entity entity)
    {
        // 이벤트 해제 
        entity.onTakeDamage -= UpdateState;

        // 전투 코루틴 종료 
        if (bossBattleCoroutine != null)
        {
            StopCoroutine(bossBattleCoroutine);
            bossBattleCoroutine = null;
        }

        // 전투 패턴 큐 초기화 
        attackQueue.Clear();
    }

    // 다음 스킬 실행 함수 
    protected virtual void ExecuteNextSkill()
    {
        if (skills.Length == 0) return;

        int skillIndex = attackQueue.Dequeue();
        eqippedSkills[skillIndex].Use();

        if (attackQueue.Count == 0)
            PrepareNextPattern(); // 패턴이 끝나면 새로운 패턴 준비
    }

    // 플레이어가 사거리에 있는 지 체크하는 함수 
    protected bool IsPlayerInRange()
    {
        // 공격 패턴이 없으면 사거리 체크를 하지 않아도 된다. 
        if (attackQueue.Count == 0) return false;

        // ※ attackQueue.Peek() : 다음 공격 스킬의 Index 번호 
        // → PlayerDistanceToUseSkill[attackQueue.Peek()] : 다음 공격 스킬의 사거리 
        return (GameManager.Instance.player.transform.position - transform.position).sqrMagnitude
                < PlayerDistanceToUseSkill[attackQueue.Peek()] * PlayerDistanceToUseSkill[attackQueue.Peek()];
    } 

    // 보스 스킬 사용 시도 코루틴 함수 
    protected IEnumerator Battle()
    {
        while (true)
        {
            if (IsPlayerInRange())
                ExecuteNextSkill();

            // 지정된 시간 만큼 대기
            yield return waitForSeconds;
        }
    }
}
