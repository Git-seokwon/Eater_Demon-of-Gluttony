using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearHog_EliteAI : MonsterAI
{
    protected override void Awake()
    {
        base.Awake();

        // �÷��̾� Ÿ�� ���� 
        entity.Target = GameManager.Instance.player;
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
        float hp = enemy.defaultHp + (0.72f * wave + 7.2f * (stage));
        float attack = enemy.defaultAttack + (0.42f * wave + 4.2f * (stage));
        float defence = enemy.defaultDefence + (0.4f * wave + 4 * (stage));

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
                // ��ų ����� �����ϸ� 
                if (eqippedSkill.Use())
                    // Bear Hog ȿ���� ���
                    SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.bearSkill);
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
