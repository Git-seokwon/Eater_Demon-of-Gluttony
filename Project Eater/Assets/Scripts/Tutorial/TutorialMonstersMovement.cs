using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMonstersMovement : TutorialBase
{
    [SerializeField]
    private TutorialEnemyMovement[] targetMonsters; // �̵��� ���͵�
    [SerializeField]
    private Vector3[] endPositions; // ��ǥ ��ġ

    private bool[] isCompleted; // �̵� �Ϸ� ����

    public override void Enter()
    {
        isCompleted = new bool[targetMonsters.Length];

        for (int i = 0; i < targetMonsters.Length; i++)
        {
            int index = i; // ���� ĸó ����

            StartCoroutine(targetMonsters[i].MoveToPosition(endPositions[i], result =>
            {
                // result�� MoveToPosition�� Action<bool> �ݹ鿡�� ���޹޴� ������, �̵� ���� ���θ� �ǹ�
                if (result) isCompleted[index] = true;
            }));
        }
    }

    public override void Execute(TutorialController controller)
    {
        // ��� ���Ͱ� �����ߴ��� Ȯ��
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
