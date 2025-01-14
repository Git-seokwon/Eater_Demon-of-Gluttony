using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class StageManager : SingletonMonobehaviour<StageManager>
{
    private IReadOnlyList<SpawnableObjectsByWave<GameObject>> WaveSpawnList;
    private RandomSpawnableObject<GameObject> EnemySpawnHelperClass;
    private List<GameObject> spawnedEnemyList;
    private List<Vector3> spawnPositions;

    private const int maxStageWave = 10;
    private const int maxFieldMonsterNum = 140;
    private const float maxWaveTime = 170f;              // 2 min 50 sec;
    private const float timeBetweenSpawn = 5f;

    private int stageWave;      // 현재 스테이지의 wave
    private float waveTime;

    private IEnumerator waveCoroutine;
    private WaitForSeconds waitUIEffect;            // wait for UI effect
    private WaitForSeconds waitOneSec;            // wait for Timer

    // 플레이어가 스테이지에서 획득한 바알의 살점 
    public int GetBaalFlesh {  get; private set; }
    // 킬 카운트
    public int KillCount { get; private set; }
    // 스테이지 클리어 여부 
    public bool IsClear { get; private set; }
    public bool isPlayerDead { get { return isPlayerDead; } set { isPlayerDead = value; IsClear = !value; } }

    // 스테이지 클리어 횟수를 저장하는 자료구조
    // → key : Stage의 CodeName, Value : Clear 횟수
    private Dictionary<string, int> clearCount = new Dictionary<string, int>();
    public IReadOnlyDictionary<string, int> ClearCount => clearCount;

    #region Stage
    [SerializeField]
    private Transform returnPosition;
    public Transform ReturnPosition => returnPosition;
    [SerializeField]
    private GameObject stageLevel;
    [SerializeField]
    private List<Stage> stages;        // 스테이지 목록
    public IReadOnlyList<Stage> Stages => stages;
    private Stage currentStage;
    public Stage CurrentStage
    {
        get { return  currentStage; }
        set
        {
            if (currentStage == value) return;

            if (value != null)
            {
                currentStage = value;
                currentRoom = stageLevel.transform.Find(currentStage.StageRoom.name).GetComponent<Room>();
                WaveSpawnList = CurrentStage.EnemiesByWaveList;
                EnemySpawnHelperClass = new RandomSpawnableObject<GameObject>(WaveSpawnList);
                spawnPositions = CurrentStage.SpawnPositions;
            }
            else
            {
                currentStage = null;
                currentRoom = null;
                WaveSpawnList = null;
                EnemySpawnHelperClass = null;
                spawnPositions = null;
            }
        }
    }
    #endregion

    #region Room
    private GameObject mapLevel;
    private List<Room> rooms = new();
    public IReadOnlyList<Room> Rooms => rooms;

    // Stage가 변경되면 event가 발생하여 currentRoom도 같이 변경된다. 
    private Room currentRoom;
    public Room CurrentRoom => currentRoom;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        mapLevel = GameObject.Find("Level");

        // Destroy any spawned enemies
        if (spawnedEnemyList != null && spawnedEnemyList.Count > 0)
        {
            foreach (GameObject enemy in spawnedEnemyList)
            {
                Destroy(enemy);
            }
        }
        else if (spawnedEnemyList == null)
        {
            spawnedEnemyList = new List<GameObject>();
        }
    }

    void Start()
    {
        rooms = mapLevel.GetComponentsInChildren<Room>().ToList();

        waitUIEffect = new WaitForSeconds(2f);
        waitOneSec = new WaitForSeconds(1f);
        waveCoroutine = ProgressWave();

        waveTime = 0f;
        stageWave = 1;
    }

    public void StartWave()
    {
        StartCoroutine(waveCoroutine);
        Debug.Log("Start Wave of" + $" {currentStage.StageRoom.name}!");
    }

    IEnumerator ProgressWave()
    {
        yield return waitUIEffect;

        waveTime = 0f;
        float spawnIntervalTime = 0f;

        // UI - "Wave Start"
        while (waveTime <= maxWaveTime)
        {
            Debug.Log($"wave time : {waveTime}");
            yield return waitOneSec;
            waveTime++;
            spawnIntervalTime++;

            if (timeBetweenSpawn <= spawnIntervalTime)
            {
                StartCoroutine(MonsterSpawn());
                spawnIntervalTime = 0f;
            }
        }

        // evaluate anger interval time
        float angerCountTime = 0f;
        float angerRemainTime = 5f;

        // UI - "Monsters Anger Warning"
        // count down
        while (angerCountTime <= angerRemainTime)
        {
            Debug.Log($"anger interval time : {angerCountTime}");
            yield return waitOneSec;
            angerCountTime++;
        }

        // UI - "Anger"
        // make spawnedEnemyList anger
        foreach (var monster in spawnedEnemyList)
        {
            monster.GetComponent<EnemyEntity>().GetAnger();
        }

        yield return new WaitUntil(() => spawnedEnemyList.Count() == 0);

        // wave end
        WaveFin();
    }

    IEnumerator MonsterSpawn()
    {
        int spawnMonsterNum = 0;
        float M = stageWave * 2.5f + 1, m = stageWave * 1.7f;

        // 정예
        // 추가
        // 기본

        // need to repeat spawning monsters
        spawnMonsterNum = (int)((m - M) / 1000);
        spawnMonsterNum = (int)(waveTime - 100) * (int)(waveTime - 100);
        spawnMonsterNum += (int)M;

        for (int i = 0; i < spawnMonsterNum; ++i)
        {
            // check field monster numbers
            if (spawnedEnemyList.Count < maxFieldMonsterNum)
            {
                // monster spawn
                GameObject enemyPrefab = EnemySpawnHelperClass.GetItem();
                Vector3 tempPosition = spawnPositions[i % spawnPositions.Count];
                spawnedEnemyList.Add(PoolManager.Instance.ReuseGameObject(enemyPrefab, tempPosition, Quaternion.identity));
            }
            else
            {
                Debug.Log("140입니다");
                break;
            }
        }

        yield break;
    }

    private void WaveFin()
    {
        StopCoroutine(waveCoroutine);

        waveTime = 0f;

        if (stageWave < maxStageWave)
        {
            stageWave++;
            StartCoroutine(waveCoroutine);
        }
        else
        {
            // Boss Wave
            stageWave++;        // stageWave = 11
            SpawnBoss();
        }
    }

    private void SpawnBoss()
    {
        // spawn stage boss
    }

    public int GetCurrentStageWave()
    {
        return stageWave;
    }

    public void ResetVariable(bool isReStart = false)
    {
        GetBaalFlesh = 0;
        KillCount = 0;
        IsClear = false;

        if (!isReStart)
            CurrentStage = null;
    }
}
