using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : IdentifiedObject
{
    [SerializeField]
    private GameObject stageRoom;
    [SerializeField]
    private Vector3 stageRoomPostion;
    [SerializeField]
    private List<SpawnableObjectsByWave<GameObject>> enemiesByWaveList;
    [SerializeField]
    private List<WaveEnemySpawnParameters> waveEnemySpawnParametersList;
    [SerializeField]
    private GameObject stageBoss;
    // TODO
    // → 스테이지 배경 음악 변수 만들기 

    private Vector3[] spawnPositions;
    private Vector3 bossSpawnPosition;
    private Vector3 playerSpawnPosition;

    public GameObject StageRoom => stageRoom;
    public IReadOnlyList<SpawnableObjectsByWave<GameObject>> EnemiesByWaveList => enemiesByWaveList;
    public IReadOnlyList<WaveEnemySpawnParameters> WaveEnemySpawnParametersList => waveEnemySpawnParametersList;
    public GameObject StageBoss => stageBoss;
    public Vector3[] SpawnPositions
    {
        get
        {
            SetSpawnPositions();
            return spawnPositions;
        }
    }
    public Vector3 BossSpawnPosition
    {
        get
        {
            SetSpawnBossPosition();
            return bossSpawnPosition;
        }
    }
    public Vector3 PlayerSpawnPosition
    {
        get
        {
            SetSpawnPlayerPosition();
            return playerSpawnPosition;
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
        {
            if (i == 0)
                continue;

            spawnPositions[i] = stageRoomPostion + enemySpawnPositions[i].position;
        }
    }

    private void SetSpawnBossPosition()
    {
        // 4번째 자식이 bossSpawnPosition
        Transform chid = stageRoom.transform.GetChild(3);
        bossSpawnPosition = stageRoomPostion + chid.position;
    }

    private void SetSpawnPlayerPosition()
    {
        // 5번째 자식이 playerSpawnPosition
        Transform chid = stageRoom.transform.GetChild(4);
        playerSpawnPosition = stageRoomPostion + chid.position;
    }
}
