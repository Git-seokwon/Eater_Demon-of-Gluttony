using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class CoachellaAI : MonsterAI
{
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        // Target 설정 : 플레이어 
        entity.Target = GameManager.Instance.player;
    }

    protected override IEnumerator SetEnemyCoroutine(int wave, int stage)
    {
        yield return StartCoroutine(base.SetEnemyCoroutine(wave, stage));

        // 스킬 AI 시작 
        playerDistanceCheckCoroutine = StartCoroutine(CheckPlayerDistance());

        // 몬스터 사망시 코루틴 종료 
        entity.onDead += OnDead;

        // 몬스터 스텟 복구 및 보정 
        var enemy = entity as EnemyEntity;
        // 보정 스텟 수치 계산 
        float hp = enemy.defaultHp + (0.2f * wave + 2 * (stage));
        float attack = enemy.defaultAttack + (0.15f * wave + 1.5f * (stage));
        float defence = enemy.defaultDefence + (0.1f * wave + 1 * (stage));

        // 스텟 적용
        ApplyStatsCorrection(hp, attack, defence);
    }

    public void SetTutorialEnemy(int wave, int stage)
    {
        // 스킬 장착 
        if (skill != null)
        {
            var clone = entity.SkillSystem.Register(skill);
            eqippedSkill = entity.SkillSystem.Equip(clone);
        }

        // 스킬 AI 시작 
        playerDistanceCheckCoroutine = StartCoroutine(CheckPlayerDistance());

        // 몬스터 사망시 코루틴 종료 
        entity.onDead += OnDead;
    }

    // 일정 시간 간격으로 타겟과의 거리 체크 
    // → 플레이어가 해당 반경으로 들어가면 
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
}
