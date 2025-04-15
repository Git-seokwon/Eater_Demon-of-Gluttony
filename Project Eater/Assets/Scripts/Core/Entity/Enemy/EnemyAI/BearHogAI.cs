using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;
using static UnityEngine.EventSystems.EventTrigger;

public class BearHogAI : MonsterAI
{
    protected override void Awake()
    {
        base.Awake();

        // 플레이어 타겟 설정 
        entity.Target = GameManager.Instance.player;
    }

    protected override IEnumerator SetEnemyCoroutine(int wave, int stage)
    {
        yield return StartCoroutine(base.SetEnemyCoroutine(wave, stage));

        // 몬스터 스텟 복구 및 보정 
        var enemy = entity as EnemyEntity;
        // 보정 스텟 수치 계산 
        float hp = enemy.defaultHp + (0.9f * wave + 4.5f * (stage + 1));
        float attack = enemy.defaultAttack + (0.4f * wave + 4 * (stage + 1));
        float defence = enemy.defaultDefence + (0.4f * wave + 4 * (stage + 1));

        // 스텟 적용
        ApplyStatsCorrection(hp, attack, defence);
    }
}
