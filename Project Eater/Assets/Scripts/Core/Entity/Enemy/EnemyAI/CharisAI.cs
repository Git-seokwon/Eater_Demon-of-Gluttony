using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharisAI : MonsterAI
{
    protected override void Awake()
    {
        base.Awake();

        // Target ���� 
        entity.Target = entity;
    }

    protected override IEnumerator SetEnemyCoroutine(int wave, int stage)
    {
        yield return StartCoroutine(base.SetEnemyCoroutine(wave, stage));

        // ���� ���� ���� �� ���� 
        var enemy = entity as EnemyEntity;
        // ���� ���� ��ġ ��� 
        float hp = enemy.defaultHp + (0.4f * wave + 4 * (stage + 1));
        float attack = enemy.defaultAttack + (0.3f * wave + 3 * (stage + 1));
        float defence = enemy.defaultDefence + (0.2f * wave + 2 * (stage + 1));

        // ���� ����
        ApplyStatsCorrection(hp, attack, defence);
    }
}
