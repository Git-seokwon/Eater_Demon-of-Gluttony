using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    private IReadOnlyList<SpawnableObjectsByWave<GameObject>> testWaveSpawnList;
    private RandomSpawnableObject<GameObject> randomEnemyHelperClass;
    private List<GameObject> instantiatedEnemyList = new List<GameObject>();

    // 테스트용 변수들
    public Stage roomTemplate;
    Vector2 tempPosition = new Vector2(-85, 50);
    // public GameObject enemyPrefab;

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

        if (roomTemplate != null)
        {
            testWaveSpawnList = roomTemplate.EnemiesByWaveList;

            // Create RandomSpawnableObject helper class
            randomEnemyHelperClass = new RandomSpawnableObject<GameObject>(testWaveSpawnList);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameObject enemyPrefab = randomEnemyHelperClass.GetItem();
            
            if (enemyPrefab != null)
            {
                instantiatedEnemyList.Add(PoolManager.Instance.ReuseGameObject(enemyPrefab, tempPosition, Quaternion.identity));
            }
        }
    }
}
