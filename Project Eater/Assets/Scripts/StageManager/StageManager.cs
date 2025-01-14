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

    private int stageWave;      // ���� ���������� wave
    private float waveTime;

    private IEnumerator waveCoroutine;
    private WaitForSeconds waitUIEffect;            // wait for UI effect
    private WaitForSeconds waitOneSec;            // wait for Timer

    // �÷��̾ ������������ ȹ���� �پ��� ���� 
    public int GetBaalFlesh {  get; private set; }
    // ų ī��Ʈ
    public int KillCount { get; private set; }
    // �������� Ŭ���� ���� 
    public bool IsClear { get; private set; }
    public bool isPlayerDead { get { return isPlayerDead; } set { isPlayerDead = value; IsClear = !value; } }

    // �������� Ŭ���� Ƚ���� �����ϴ� �ڷᱸ��
    // �� key : Stage�� CodeName, Value : Clear Ƚ��
    private Dictionary<string, int> clearCount = new Dictionary<string, int>();
    public IReadOnlyDictionary<string, int> ClearCount => clearCount;

    #region Stage
    [SerializeField]
    private Transform returnPosition;
    public Transform ReturnPosition => returnPosition;
    [SerializeField]
    private GameObject stageLevel;
    [SerializeField]
    private List<Stage> stages;        // �������� ���
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

    // Stage�� ����Ǹ� event�� �߻��Ͽ� currentRoom�� ���� ����ȴ�. 
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

        // ����
        // �߰�
        // �⺻

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
                Debug.Log("140�Դϴ�");
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
