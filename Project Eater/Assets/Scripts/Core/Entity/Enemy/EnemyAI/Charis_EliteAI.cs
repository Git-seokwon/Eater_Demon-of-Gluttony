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

    protected override IEnumerator SetEnemyCoroutine(int wave, int stage)
    {
        yield return StartCoroutine(base.SetEnemyCoroutine(wave, stage));

        // ��ų AI ���� 
        playerDistanceCheckCoroutine = StartCoroutine(CheckPlayerDistance());

        // ���� ����� �ڷ�ƾ ���� 
        entity.onDead += OnDead;

        // ���� ���� ���� �� ���� 
        var enemy = entity as EnemyEntity;
        // ���� ���� ��ġ ��� 
        float hp = enemy.defaultHp + (0.48f * wave + 4.8f * (stage));
        float attack = enemy.defaultAttack + (0.35f * wave + 3.5f * (stage));
        float defence = enemy.defaultDefence + (0.3f * wave + 3 * (stage));

        // ���� ����
        ApplyStatsCorrection(hp, attack, defence);
    }

    // ���� �ð� �������� Ÿ�ٰ��� �Ÿ� üũ
    private IEnumerator CheckPlayerDistance()
    {
        while (!entity.IsDead)
        {
            if ((GameManager.Instance.player.transform.position - transform.position).sqrMagnitude
                < PlayerDistanceToUseSkill * PlayerDistanceToUseSkill)
            {
                if (eqippedSkill.Use())
                    SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.charisSkill);

                // �ڷ�ƾ ����
                yield break;
            }

            // ������ �ð� ��ŭ ���
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
