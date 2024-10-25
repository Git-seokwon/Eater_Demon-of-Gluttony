using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private IReadOnlyList<SpawnableObjectsByWave<GameObject>> testWaveSpawnList;
    private RandomSpawnableObject<GameObject> EnemySpawnHelperClass;
    private List<GameObject> instantiatedEnemyList = new List<GameObject>();

    // �׽�Ʈ�� ������
    public Stage stage;
    Vector2 tempPosition = new Vector2(100, 0);

    public void Awake()
    {
        // Destroy any spawned enemies
        if (instantiatedEnemyList != null && instantiatedEnemyList.Count > 0)
        {
            foreach (GameObject enemy in instantiatedEnemyList)
            {
                Destroy(enemy);
            }
        }

        if (stage != null)
        {
            testWaveSpawnList = stage.EnemiesByWaveList;

            // Create RandomSpawnableObject helper class
            EnemySpawnHelperClass = new RandomSpawnableObject<GameObject>(testWaveSpawnList);
        }


    }

    public void MonsterSpawn()
    {
        GameObject enemyPrefab = EnemySpawnHelperClass.GetItem();

        if (enemyPrefab != null)
        {
            instantiatedEnemyList.Add(PoolManager.Instance.ReuseGameObject(enemyPrefab, tempPosition, Quaternion.identity));
        }
    }

    public void BossSpawn()
    {
        // ��� ���̺갡 ���� �� ����Ǵ� �Լ�
    }
}
