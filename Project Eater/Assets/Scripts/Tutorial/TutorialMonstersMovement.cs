using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMonstersMovement : TutorialBase
{
    [SerializeField]
    private TutorialEnemyMovement[] targetMonsters; // 이동할 몬스터들
    [SerializeField]
    private Vector3[] endPositions; // 목표 위치

    private bool[] isCompleted; // 이동 완료 여부

    public override void Enter()
    {
        isCompleted = new bool[targetMonsters.Length];

        for (int i = 0; i < targetMonsters.Length; i++)
        {
            int index = i; // 람다 캡처 방지

            StartCoroutine(targetMonsters[i].MoveToPosition(endPositions[i], result =>
            {
                // result는 MoveToPosition의 Action<bool> 콜백에서 전달받는 값으로, 이동 성공 여부를 의미
                if (result) isCompleted[index] = true;
            }));
        }
    }

    public override void Execute(TutorialController controller)
    {
        // 모든 몬스터가 도착했는지 확인
        if (AllMonstersCompleted())
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
    }

    private bool AllMonstersCompleted()
    {
        foreach (bool completed in isCompleted)
        {
            if (!completed) return false;
        }
        return true;
    }
}
