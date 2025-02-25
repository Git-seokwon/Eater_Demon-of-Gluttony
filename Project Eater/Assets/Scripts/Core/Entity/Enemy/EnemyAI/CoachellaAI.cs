using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class CoachellaAI : MonsterAI
{
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        // Target ���� : �÷��̾� 
        entity.Target = GameManager.Instance.player;
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
        float hp = enemy.defaultHp + (0.3f * wave + 3 * (stage + 1));
        float attack = enemy.defaultAttack + (0.2f * wave + 2 * (stage + 1));
        float defence = enemy.defaultDefence + (0.1f * wave + 1 * (stage + 1));

        // ���� ����
        ApplyStatsCorrection(hp, attack, defence);
    }

    public void SetTutorialEnemy(int wave, int stage)
    {
        base.SetEnemy(wave, stage);

        // ��ų AI ���� 
        playerDistanceCheckCoroutine = StartCoroutine(CheckPlayerDistance());

        // ���� ����� �ڷ�ƾ ���� 
        entity.onDead += OnDead;
    }

    // ���� �ð� �������� Ÿ�ٰ��� �Ÿ� üũ 
    // �� �÷��̾ �ش� �ݰ����� ���� 
    private IEnumerator CheckPlayerDistance()
    {
        while (true)
        {
            if ((GameManager.Instance.player.transform.position - transform.position).sqrMagnitude
                < PlayerDistanceToUseSkill * PlayerDistanceToUseSkill)
            {
                eqippedSkill.Use();
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
