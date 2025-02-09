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

    public override void SetEnemy(int wave, int stage)
    {
        base.SetEnemy(wave, stage);

        // ���� ���� ���� �� ���� 
        var enemy = entity as EnemyEntity;
        // ���� ���� ��ġ ��� 
        float hp = enemy.defaultHp + (0.4f * wave + 4 * stage);
        float attack = enemy.defaultAttack + (0.3f * wave + 3 * stage);
        float defence = enemy.defaultDefence + (0.2f * wave + 2 * stage);

        // ���� ����
        ApplyStatsCorrection(hp, attack, defence);
    }
}
