using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class Coachella_EliteAI : MonsterAI
{
    [SerializeField]
    protected Skill extraSkill; // 자폭 스킬 

    protected Skill extraEqippedSkill;

    protected override void Awake()
    {
        base.Awake();

        // Target 설정 : 플레이어 
        entity.Target = GameManager.Instance.player;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (extraEqippedSkill != null)
        {
            extraEqippedSkill.onDeactivated -= OnDeactivatedSkill;
            entity.onSelfDestruct -= OnSelfDestruct;
            entity.SkillSystem.Disarm(extraEqippedSkill);
            entity.SkillSystem.Unregister(extraEqippedSkill);
            extraEqippedSkill = null;
        }
    }

    public override void SetEnemy(int wave, int stage)
    {
        base.SetEnemy(wave, stage);

        // 자폭 스킬 장착 & event 등록
        if (extraSkill != null)
        {
            var clone = entity.SkillSystem.Register(extraSkill);
            extraEqippedSkill = entity.SkillSystem.Equip(clone);
            extraEqippedSkill.onDeactivated += OnDeactivatedSkill;
            entity.onSelfDestruct += OnSelfDestruct;
        }

        // 스킬 AI 시작 
        playerDistanceCheckCoroutine = StartCoroutine(CheckPlayerDistance());

        // 몬스터 사망시 코루틴 종료 
        entity.onDead += OnDead;

        // 몬스터 스텟 복구 및 보정 
        var enemy = entity as EnemyEntity;
        // 보정 스텟 수치 계산 
        float hp = enemy.defaultHp + (0.45f * wave + 4.5f * (stage + 1));
        float attack = enemy.defaultAttack + (0.28f * wave + 2.8f * (stage + 1));
        float defence = enemy.defaultDefence + (0.15f * wave + 1.5f * (stage + 1));

        // 스텟 적용
        ApplyStatsCorrection(hp, attack, defence);
    }

    // 일정 시간 간격으로 타겟과의 거리 체크
    private IEnumerator CheckPlayerDistance()
    {
        while (!entity.IsDead)
        {
            if ((GameManager.Instance.player.transform.position - transform.position).sqrMagnitude
                < PlayerDistanceToUseSkill * PlayerDistanceToUseSkill)
            {
                eqippedSkill.Use();
            }

            // 지정된 시간 만큼 대기
            yield return waitForSeconds;
        }
    }

    private void OnDead(Entity entity, bool isRealDead)
    {
        if (playerDistanceCheckCoroutine != null)
            StopCoroutine(CheckPlayerDistance());

        playerDistanceCheckCoroutine = null;
    }

    private void OnSelfDestruct()
    {
        // 기본 공격 중일 수도 있기 때문에 스킬 사용을 취소한다. 
        entity.SkillSystem.Cancel(eqippedSkill);

        // 코첼라 자폭 효과음 재생
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.coachellaSuicide);


        extraEqippedSkill.Use();
    }

    private void OnDeactivatedSkill(Skill skill)
    {
        (entity as EnemyEntity).OnDead();        
    }
}