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
    // �� �������� ��� ���� ���� ����� 

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
        // 3��° �ڽ��� spawnPositions
        Transform child = stageRoom.transform.GetChild(2);
        // spawnPositions�� �ڽ����� �ִ� �� Transform���� ������
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
        // 4��° �ڽ��� bossSpawnPosition
        Transform chid = stageRoom.transform.GetChild(3);
        bossSpawnPosition = stageRoomPostion + chid.position;
    }

    private void SetSpawnPlayerPosition()
    {
        // 5��° �ڽ��� playerSpawnPosition
        Transform chid = stageRoom.transform.GetChild(4);
        playerSpawnPosition = stageRoomPostion + chid.position;
    }
}
