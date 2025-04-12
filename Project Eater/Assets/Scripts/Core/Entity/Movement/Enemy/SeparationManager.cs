using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeparationManager : SingletonMonobehaviour<SeparationManager>
{
    [SerializeField]
    private float separationUpdateInterval = 0.2f; // ������Ʈ ���� (��)

    private List<EnemyMovement> enemies = new List<EnemyMovement>();
    private Vector3[] separationVectors;
    private const int maxEnemies = 120;
    private WaitForSeconds separationUpdate;

    private Coroutine separationUpdateCoroutine;

    protected override void Awake()
    {
        base.Awake();

        separationVectors = new Vector3[maxEnemies];
        separationUpdate = new WaitForSeconds(separationUpdateInterval);
    }

    public void StartSeparationForAllEnemies()
    {
        if (separationUpdateCoroutine != null)
            StopCoroutine(separationUpdateCoroutine);
        separationUpdateCoroutine = StartCoroutine(SeparationUpdateCoroutine());
    }

    public void StopSeparationForAllEnemies()
    {
        if (separationUpdateCoroutine != null)
        {
            StopCoroutine(separationUpdateCoroutine);
            separationUpdateCoroutine = null;
        }
    }

    private IEnumerator SeparationUpdateCoroutine()
    {
        while (true)
        {
            UpdateSeparationForAllEnemies();
            yield return separationUpdate; // �ֱ������� ����
        }
    }

    private void UpdateSeparationForAllEnemies()
    {
        // enemies ����Ʈ ��ü�� ����ִ� ������ ���� ����Ʈ�� SpawnedEnemyList�� ������� �ʴ´�. 
        enemies.Clear();
        enemies.AddRange(StageManager.Instance.SpawnedEnemyList);

        int count = enemies.Count;
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            // ���� ���(��Ȱ��ȭ��) ��� ����
            if (enemies[i] == null || !enemies[i].gameObject.activeInHierarchy) continue;

            Vector3 separationForce = Vector3.zero;
            Vector3 currentPosition = enemies[i].transform.position;

            for (int j = 0; j < count; j++)
            {
                // ���� ���(��Ȱ��ȭ�Ǿ�����) �ǳʶٱ�
                if (i == j || enemies[j] == null || !enemies[j].gameObject.activeInHierarchy) continue;

                Vector3 directionAway = currentPosition - enemies[j].transform.position;
                float distance = directionAway.magnitude;

                if (distance > 0f && distance < enemies[i].SeparationRadius)
                {
                    separationForce += directionAway.normalized / distance;
                }
            }

            separationVectors[i] = separationForce.normalized;
        }
    }

    public Vector3 GetSeparationForceForEnemy(EnemyMovement enemy)
    {
        int index = enemies.IndexOf(enemy);
        if (index >= 0)
        {
            return separationVectors[index];
        }
        return Vector3.zero; // ���� ã�� �� ���� �� �⺻ �� ��ȯ
    }
}
