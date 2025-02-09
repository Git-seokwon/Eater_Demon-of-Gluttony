using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharisAI : MonsterAI
{
    protected override void Awake()
    {
        base.Awake();

        // Target 설정 
        entity.Target = entity;
    }

    public override void SetEnemy(int wave, int stage)
    {
        base.SetEnemy(wave, stage);

        // 몬스터 스텟 복구 및 보정 
        var enemy = entity as EnemyEntity;
        // 보정 스텟 수치 계산 
        float hp = enemy.defaultHp + (0.4f * wave + 4 * stage);
        float attack = enemy.defaultAttack + (0.3f * wave + 3 * stage);
        float defence = enemy.defaultDefence + (0.2f * wave + 2 * stage);

        // 스텟 적용
        ApplyStatsCorrection(hp, attack, defence);
    }
}
