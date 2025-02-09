using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charis_EliteAI : MonsterAI
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

        // ��ų AI ���� 
        playerDistanceCheckCoroutine = StartCoroutine(CheckPlayerDistance());

        // ���� ����� �ڷ�ƾ ���� 
        entity.onDead += OnDead;

        // ���� ���� ���� �� ���� 
        var enemy = entity as EnemyEntity;
        // ���� ���� ��ġ ��� 
        float hp = enemy.defaultHp + (0.6f * wave + 6 * stage);
        float attack = enemy.defaultAttack + (0.42f * wave + 4.2f * stage);
        float defence = enemy.defaultDefence + (0.3f * wave + 3 * stage);

        // ���� ����
        ApplyStatsCorrection(hp, attack, defence);
    }

    // ���� �ð� �������� Ÿ�ٰ��� �Ÿ� üũ
    private IEnumerator CheckPlayerDistance()
    {
        while (true)
        {
            if ((GameManager.Instance.player.transform.position - transform.position).sqrMagnitude
                < PlayerDistanceToUseSkill * PlayerDistanceToUseSkill)
            {
                eqippedSkill.Use();

                // �ڷ�ƾ ����
                yield break;
            }

            // ������ �ð� ��ŭ ���
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
