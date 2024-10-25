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
    // �� �������� ��� ���� ���� ����� 

    // ���� �������� stageRoom�� �ڽ� ������Ʈ�� spawnPositions�� �Ҵ��Ѵ�. 
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
        // 3��° �ڽ��� spawnPositions
        Transform child = stageRoom.transform.GetChild(2);
        // spawnPositions�� �ڽ����� �ִ� �� Transform���� ������
        var enemySpawnPositions = child.GetComponentsInChildren<Transform>();
        // Set spawnPositions
        for (int i = 0; i < enemySpawnPositions.Length; i++)
            spawnPositions[i] = enemySpawnPositions[i];
    }

    private void SetSpawnBossPosition()
    {
        // 4��° �ڽ��� bossSpawnPosition
        Transform chid = stageBoss.transform.GetChild(3);
        var bossSpawnPosition = chid.GetComponentInChildren<Transform>();
        this.bossSpawnPosition = bossSpawnPosition;
    }
}
