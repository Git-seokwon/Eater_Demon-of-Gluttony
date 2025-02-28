using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Networking.Types;

public class BossTest : MonoBehaviour
{
    [SerializeField]
    protected Skill skill; // 보스 스킬들
    [SerializeField]
    protected float PlayerDistanceToUseSkill; // 보스 스킬별 사거리
    [SerializeField]
    protected float checkInterval = 0.1f; // 코루틴 주기 

    protected Skill eqippedSkill; // 장착된 스킬들 
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
        // 스킬 장착 해제 
        if (skill != null)
        {
            entity.SkillSystem.Disarm(eqippedSkill);
            entity.SkillSystem.Unregister(eqippedSkill);
            eqippedSkill = null;
        }

        // 비활성화 시, OnDead 이벤트 해제 
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
        // 스킬 장착 
        if (skill != null)
        {
            var clone = entity.SkillSystem.Register(skill);
            eqippedSkill = entity.SkillSystem.Equip(clone);
        }

        // 몬스터 사망시 코루틴 종료 
        entity.onDead += OnDead;

        // 스킬 AI 시작 
        bossBattleCoroutine = StartCoroutine(Battle());
    }

    // 보스 몬스터 사망 처리 
    protected virtual void OnDead(Entity entity)
    {
        // 전투 코루틴 종료 
        if (bossBattleCoroutine != null)
        {
            StopCoroutine(bossBattleCoroutine);
            bossBattleCoroutine = null;
        }
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

    // 플레이어가 사거리에 있는 지 체크하는 함수 
    protected bool IsPlayerInRange()
    { 
        if (skill == null) return false;

        // ※ attackQueue.Peek() : 다음 공격 스킬의 Index 번호 
        // → PlayerDistanceToUseSkill[attackQueue.Peek()] : 다음 공격 스킬의 사거리 
        return (GameManager.Instance.player.transform.position - transform.position).sqrMagnitude
                < PlayerDistanceToUseSkill * PlayerDistanceToUseSkill;
    }

    protected virtual void ExecuteNextSkill()
    {
        if (skill == null) return;

        eqippedSkill.Use();
    }
}
