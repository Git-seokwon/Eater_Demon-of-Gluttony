using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class MajinAI : BossAI
{
    // �� ��ų ���� 
    // 0. ����
    // 1. �罽 ��Ÿ 
    // 2. �۾��� 
    // 3. �ݰ� 

    protected override void Awake()
    {
        base.Awake();

        // Target ���� : �÷��̾� 
        entity.Target = GameManager.Instance.player;
    }

    protected override void UpdateState(Entity entity, Entity instigator, object causer, float damage)
    {
        switch (currentState)
        {
            case BossState.Phase1:
                if (entity.Stats.FullnessStat.DefaultValue < entity.Stats.FullnessStat.MaxValue * 0.7f)
                {
                    currentState = BossState.Phase2;
                    PrepareNextPattern();
                }
                break;

            case BossState.Phase2:
                if (entity.Stats.FullnessStat.DefaultValue < entity.Stats.FullnessStat.MaxValue * 0.35f)
                {
                    currentState = BossState.Phase3;
                    PrepareNextPattern();
                }
                break;

            case BossState.Phase3:
                break;
            default:
                break;
        }
    }

    protected override void PrepareNextPattern()
    {
        if (currentState == BossState.Phase3)
        {
            attackQueue.Clear(); // ���� ��⿭ �ʱ�ȭ

            // 0�� ��ų�� �׻� ù ��° ����
            attackQueue.Enqueue(0);

            // 2�� or 3�� ��ų �� �ϳ��� ���� ����
            int selectedSkill = random.Next(2, 4); // 2 �Ǵ� 3 �� �ϳ� ����

            if (random.Next(0, 2) == 0) // 50% Ȯ���� ���� ����
            {
                attackQueue.Enqueue(1);
                attackQueue.Enqueue(selectedSkill);
            }
            else
            {
                attackQueue.Enqueue(selectedSkill);
                attackQueue.Enqueue(1);
            }
        }
        else if (currentState == BossState.Phase2)
        {
            attackQueue.Clear(); // ���� ��⿭ �ʱ�ȭ

            // 0�� ��ų�� �׻� ù ��° ����
            attackQueue.Enqueue(0);

            // 1�� or 2�� ��ų �� �ϳ��� ���� ����
            int selectedSkill = random.Next(1, 3); // 1 �Ǵ� 2 �� �ϳ� ����
            // ���õ� ��ų ����
            attackQueue.Enqueue(selectedSkill);

            // ���õ��� ���� ��ų ���� 
            if (selectedSkill == 1)
                attackQueue.Enqueue(2);
            else
                attackQueue.Enqueue(1);
        }
        else
        {
            attackQueue.Clear(); // ���� ��⿭ �ʱ�ȭ

            attackQueue.Enqueue(0);
            attackQueue.Enqueue(1);
        }
    }
}
