using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class MajinAI : BossAI
{
    // ※ 스킬 순서 
    // 0. 돌진
    // 1. 사슬 강타 
    // 2. 휩쓸기 
    // 3. 반격 

    protected override void Awake()
    {
        base.Awake();

        // Target 설정 : 플레이어 
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
            attackQueue.Clear(); // 기존 대기열 초기화

            // 0번 스킬은 항상 첫 번째 실행
            attackQueue.Enqueue(0);

            // 2번 or 3번 스킬 중 하나를 랜덤 선택
            int selectedSkill = random.Next(2, 4); // 2 또는 3 중 하나 선택

            if (random.Next(0, 2) == 0) // 50% 확률로 순서 변경
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
            attackQueue.Clear(); // 기존 대기열 초기화

            // 0번 스킬은 항상 첫 번째 실행
            attackQueue.Enqueue(0);

            // 1번 or 2번 스킬 중 하나를 랜덤 선택
            int selectedSkill = random.Next(1, 3); // 1 또는 2 중 하나 선택
            // 선택된 스킬 실행
            attackQueue.Enqueue(selectedSkill);

            // 선택되지 못한 스킬 실행 
            if (selectedSkill == 1)
                attackQueue.Enqueue(2);
            else
                attackQueue.Enqueue(1);
        }
        else
        {
            attackQueue.Clear(); // 기존 대기열 초기화

            attackQueue.Enqueue(0);
            attackQueue.Enqueue(1);
        }
    }
}
