using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class ReaperAI : MonsterAI
{
    protected override void Awake()
    {
        base.Awake();

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
        float hp = enemy.defaultHp + (0.75f * wave + 7.5f * (stage + 1));
        float attack = enemy.defaultAttack + (0.7f * wave + 7 * (stage + 1));
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
}
