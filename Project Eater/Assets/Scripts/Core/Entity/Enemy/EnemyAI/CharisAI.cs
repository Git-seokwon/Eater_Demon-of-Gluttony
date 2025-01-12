using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharisAI : MonsterAI
{
    protected override void Awake()
    {
        base.Awake();

        // Target ���� 
        // �� ī������ �ڱ� �ڽ��� ���ݷ°� �̵� �ӵ��� ������Ű�� ������ Target�� �ڱ��ڽ����� �Ѵ�.
        entity.Target = entity;

        // ���� ����� �ڷ�ƾ ���� 
        entity.onDead += OnDead;
    }

    public override void SetEnemy()
    {
        base.SetEnemy();

        // ��ų AI ���� 
        playerDistanceCheckCoroutine = StartCoroutine(CheckPlayerDistance());
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
