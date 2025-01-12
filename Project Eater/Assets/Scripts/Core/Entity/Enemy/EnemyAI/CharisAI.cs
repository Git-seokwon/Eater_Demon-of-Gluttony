using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharisAI : MonsterAI
{
    protected override void Awake()
    {
        base.Awake();

        // Target 설정 
        // → 카리스는 자기 자신의 공격력과 이동 속도를 증가시키기 때문에 Target을 자기자신으로 한다.
        entity.Target = entity;

        // 몬스터 사망시 코루틴 종료 
        entity.onDead += OnDead;
    }

    public override void SetEnemy()
    {
        base.SetEnemy();

        // 스킬 AI 시작 
        playerDistanceCheckCoroutine = StartCoroutine(CheckPlayerDistance());
    }

    // 일정 시간 간격으로 타겟과의 거리 체크
    private IEnumerator CheckPlayerDistance()
    {
        while (true)
        {
            if ((GameManager.Instance.player.transform.position - transform.position).sqrMagnitude 
                < PlayerDistanceToUseSkill * PlayerDistanceToUseSkill)
            {
                eqippedSkill.Use();

                // 코루틴 종료
                yield break;
            }

            // 지정된 시간 만큼 대기
            yield return waitForSeconds;
        }
    }

    private void OnDead(Entity entity)
    {
        if (playerDistanceCheckCoroutine != null)
            StopCoroutine(CheckPlayerDistance());

        playerDistanceCheckCoroutine = null;
    }
}
