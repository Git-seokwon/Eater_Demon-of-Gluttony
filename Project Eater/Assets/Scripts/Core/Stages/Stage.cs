using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : IdentifiedObject
{
    [SerializeField]
    private int stageNumber; 
    [SerializeField]
    private GameObject stageRoom;
    [SerializeField]
    private Vector3 stageRoomPostion;
    [SerializeField]
    private int clearCount;       
    [SerializeField]
    private List<SpawnableObjectsByWave<GameObject>> enemiesByWaveList;
    [SerializeField]
    private List<SpawnableObjectsByWave<GameObject>> eliteEnemiesByWaveList;
    [SerializeField]
    private List<WaveEnemySpawnParameters> waveEnemySpawnParametersList;
    [SerializeField]
    private GameObject stageBoss;
    [SerializeField]
    private MusicTrackSO stageEnterMusic;
    [SerializeField]
    private MusicTrackSO waveStartMusic;
    [SerializeField]
    private MusicTrackSO berserkMusic;
    [SerializeField]
    private MusicTrackSO clearMusic;
    [SerializeField]
    private MusicTrackSO defeatMusic;
    
    // TODO
    // → 스테이지 배경 음악 변수 만들기 

    private List<Vector3> spawnPositions;
    private Vector3 bossSpawnPosition;
    private Vector3 playerSpawnPosition;

    public int StageNumber => stageNumber;
    public GameObject StageRoom => stageRoom;
    public IReadOnlyList<SpawnableObjectsByWave<GameObject>> EnemiesByWaveList => enemiesByWaveList;
    public IReadOnlyList<SpawnableObjectsByWave<GameObject>> EliteEnemiesByWaveList => eliteEnemiesByWaveList;
    public IReadOnlyList<WaveEnemySpawnParameters> WaveEnemySpawnParametersList => waveEnemySpawnParametersList;
    public GameObject StageBoss => stageBoss;
    public MusicTrackSO StageEnterMusic => stageEnterMusic;
    public MusicTrackSO WaveStartMusic => waveStartMusic;
    public MusicTrackSO BerserkMusic => berserkMusic;
    public MusicTrackSO ClearMusic => clearMusic;
    public MusicTrackSO DefeatMusic => defeatMusic;

    public List<Vector3> SpawnPositions
    {
        get
        {
            spawnPositions = new List<Vector3>();

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

    public int ClearCount
    {
        get
        {
            return clearCount;
        }
        set
        {
            clearCount = Mathf.Max(value, 0);
        }
    }

    private void SetSpawnPositions()
    {
        // 3번째 자식이 spawnPositions
        Transform child = stageRoom.transform.GetChild(2);
        // spawnPositions의 자식으로 있는 각 Transform들을 가져옴
        var enemySpawnPositions = child.GetComponentsInChildren<Transform>();
        // Set spawnPositions
        for (int i = 1; i < enemySpawnPositions.Length; i++)
            spawnPositions.Add(stageRoomPostion + enemySpawnPositions[i].position);
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
