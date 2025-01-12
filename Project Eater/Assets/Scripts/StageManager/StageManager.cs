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

    private int stageWave;      // 현재 스테이지의 wave
    private int maxStageWave;
    private int maxFieldMonsterNum;
    private float waveTime;
    private float maxWaveTime;
    private float timeBetweenSpawn;

    private IEnumerator waveCoroutine;
    private WaitForSeconds wait;            // wait wave spawn interval

    // 플레이어가 스테이지에서 획득한 바알의 살점 
    public int GetBaalFlesh {  get; private set; }
    // 킬 카운트
    public int KillCount { get; private set; }
    // 스테이지 클리어 여부 
    public bool IsClear {  get; private set; }

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

        wait = new WaitForSeconds(5f);
        waveCoroutine = ProgressWave();

        maxFieldMonsterNum = 140;
        maxStageWave = 10;
        maxWaveTime = 170f;              // 2 min 50 sec
        timeBetweenSpawn = 5f;

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
        float currentCountInterval = 0;
        int spawnMonsterNum = 0;
        float M = stageWave * 2.5f + 1, m = stageWave * 1.7f;

        // UI - "Wave Start"

        while (waveTime <= maxWaveTime)
        {
            // timer
            waveTime += Time.deltaTime;
            currentCountInterval += Time.deltaTime;

            // check current interval is 5 sec
            if (currentCountInterval >= timeBetweenSpawn)
            {
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
                        Vector3 tempPosition = spawnPositions[i];
                        spawnedEnemyList.Add(PoolManager.Instance.ReuseGameObject(enemyPrefab, tempPosition, Quaternion.identity));
                    }
                    else
                    {
                        break;
                    }
                }

                currentCountInterval = 0;
            }
        }

        // evaluate anger time



        // UI - "Anger Warning"
        // count down

        // UI - "Anger"
        // make spawnedEnemyList anger

        yield return new WaitUntil(() => spawnedEnemyList.Count() == 0);

        // wave end
        WaveFin();
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

    public bool IsPlayerEnter()
    {
        // 플레이어 들어오면 true
        return true;

        // 아니면 false

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
