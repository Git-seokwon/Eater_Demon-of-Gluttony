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

        // �÷��̾� Ÿ�� ���� 
        entity.Target = GameManager.Instance.player;
    }

    public override void SetEnemy(int wave, int stage)
    {
        base.SetEnemy(wave, stage);

        // ���� ���� ���� �� ���� 
        var enemy = entity as EnemyEntity;
        // ���� ���� ��ġ ��� 
        float hp = enemy.defaultHp + (0.5f * wave + 5 * stage);
        float attack = enemy.defaultAttack + (0.4f * wave + 4 * stage);
        float defence = enemy.defaultDefence + (0.4f * wave + 4 * stage);

        // ���� ����
        ApplyStatsCorrection(hp, attack, defence);
    }
}
