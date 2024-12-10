using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    [SerializeField]
    private int numUnitsPerSpawn;
    private List<GameObject> unitsInGame;

    private IReadOnlyList<SpawnableObjectsByWave<GameObject>> testWaveSpawnList;
    private RandomSpawnableObject<GameObject> randomEnemyHelperClass;
    private List<GameObject> instantiatedEnemyList = new List<GameObject>();

    // 테스트용 변수들
    public Stage roomTemplate;
    Vector2 tempPosition;
    // public GameObject enemyPrefab;

    public void Awake()
    {
        unitsInGame = new List<GameObject>();

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
            var stage = StageManager.Instance.CurrentStage;
            var spawnPositions = stage?.SpawnPositions;
            int count = spawnPositions.Count;
            int prevNum = -1;

            for (int i = 0; i < numUnitsPerSpawn; i++)
            {
                GameObject enemyPrefab = randomEnemyHelperClass.GetItem();
                if (enemyPrefab != null)
                {
                    int randomNum = UnityEngine.Random.Range(0, count);
                    if (prevNum == randomNum)
                    {
                        i--;
                        continue;
                    }

                    tempPosition = spawnPositions[randomNum];
                    var enemy = PoolManager.Instance.ReuseGameObject(enemyPrefab, tempPosition, Quaternion.identity);
                    instantiatedEnemyList.Add(enemy);
                    unitsInGame.Add(enemyPrefab);
                    prevNum = randomNum;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            foreach (var go in unitsInGame)
            {
                go.SetActive(false);
            }

            unitsInGame.Clear();
        }
    }
}
