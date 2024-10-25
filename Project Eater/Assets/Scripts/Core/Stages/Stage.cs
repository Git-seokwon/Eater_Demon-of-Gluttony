using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : IdentifiedObject
{
    [SerializeField]
    private GameObject stageRoom;
    [SerializeField]
    private List<SpawnableObjectsByWave<GameObject>> enemiesByWaveList;
    [SerializeField]
    private List<WaveEnemySpawnParameters> waveEnemySpawnParametersList;
    [SerializeField]
    private GameObject stageBoss;
    // TODO
    // → 스테이지 배경 음악 변수 만들기 

    // 스폰 포지션은 stageRoom에 자식 오브젝트인 spawnPositions를 할당한다. 
    private Transform[] spawnPositions;
    private Transform bossSpawnPosition;

    public GameObject StageRoom => stageRoom;
    public IReadOnlyList<SpawnableObjectsByWave<GameObject>> EnemiesByWaveList => enemiesByWaveList;
    public IReadOnlyList<WaveEnemySpawnParameters> WaveEnemySpawnParametersList => waveEnemySpawnParametersList;
    public GameObject StageBoss => stageBoss;
    public Transform[] SpawnPositions
    {
        get
        { 
            if (spawnPositions == null)
                SetSpawnPositions();

            return spawnPositions;
        }
    }
    public Transform BossSpawnPositions
    {
        get
        {
            if (spawnPositions == null)
                SetSpawnBossPosition();

            return bossSpawnPosition;
        }
    }

    private void SetSpawnPositions()
    {
        // 3번째 자식이 spawnPositions
        Transform child = stageRoom.transform.GetChild(2);
        // spawnPositions의 자식으로 있는 각 Transform들을 가져옴
        var enemySpawnPositions = child.GetComponentsInChildren<Transform>();
        // Set spawnPositions
        for (int i = 0; i < enemySpawnPositions.Length; i++)
            spawnPositions[i] = enemySpawnPositions[i];
    }

    private void SetSpawnBossPosition()
    {
        // 4번째 자식이 bossSpawnPosition
        Transform chid = stageBoss.transform.GetChild(3);
        var bossSpawnPosition = chid.GetComponentInChildren<Transform>();
        this.bossSpawnPosition = bossSpawnPosition;
    }
}
