using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class StageManager : SingletonMonobehaviour<StageManager>
{
    [SerializeField]
    private GameObject waveTimer;

    private StageProgressUI stageProgressUI;
    private IReadOnlyList<SpawnableObjectsByWave<GameObject>> enemiesSpawnList;
    private IReadOnlyList<SpawnableObjectsByWave<GameObject>> eliteEnemiesSpawnList;
    private RandomSpawnableObject<GameObject> enemySpawnHelperClass;
    private RandomSpawnableObject<GameObject> eliteEnemySpawnHelperClass;
    private List<GameObject> spawnedEnemyList;
    private List<Vector3> spawnPositions;

    private const int maxStageWave = 10;
    private const int maxFieldMonsterNum = 140;
    private const float maxWaveTime = 170f;              // 2 min 50 sec;
    private const float timeBetweenSpawn = 5f;

    public int stageWave { get; private set; }      // ���� ���������� wave

    private IEnumerator waveCoroutine;
    private WaitForSeconds waitUIEffect;            // wait for UI effect
    private WaitForSeconds waitOneSec;            // wait for Timer

    // �÷��̾ ������������ ȹ���� �پ��� ���� 
    public int GetBaalFlesh {  get; private set; }
    // ų ī��Ʈ
    public int KillCount { get; private set; }
    // �������� Ŭ���� ���� 
    public bool IsClear { get; private set; }

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
                enemiesSpawnList = CurrentStage.EnemiesByWaveList;
                eliteEnemiesSpawnList = CurrentStage.EliteEnemiesByWaveList;
                enemySpawnHelperClass = new RandomSpawnableObject<GameObject>(enemiesSpawnList);
                eliteEnemySpawnHelperClass = new RandomSpawnableObject<GameObject>(eliteEnemiesSpawnList);
                spawnPositions = CurrentStage.SpawnPositions;
            }
            else
            {
                currentStage = null;
                currentRoom = null;
                enemiesSpawnList = null;
                eliteEnemiesSpawnList = null;
                enemySpawnHelperClass = null;
                eliteEnemySpawnHelperClass = null;
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
        stageProgressUI = GetComponent<StageProgressUI>();

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

        stageWave = 1;
    }

    public void StartWave()
    {
        // �ӽ÷� ����
        clearCount.Add(currentStage.CodeName, 0);

        StartCoroutine(waveCoroutine);
        Debug.Log("Start Wave of" + $" {currentStage.StageRoom.name}!");
    }

    // GameManager - DisplayMessageRoutine �ڷ�ƾ�� ¥���� �ִ� Ÿ�̸�. yield return null;�� ���� update�� ȣ����� ��ٸ��ٰ� �Ѵ�. �̷� �� �ִµ� �� ������� ���� �� ��
    //// display the message for the given time
    //if (displaySeconds > 0f)
    //{
    //    float timer = displaySeconds;

    //    while (timer > 0f)
    //    {
    //        timer -= Time.deltaTime;
    //        yield return null;  
    //    }
    //}

    IEnumerator ProgressWave()
    {
        yield return waitUIEffect;

        float waveTime = 0f;
        float spawnIntervalTime = 4f;       // to spawn enemies when player enter the stage

        // UI - "Wave Start"
        StartCoroutine(stageProgressUI.ShowProgress(2f, "����ü���� �޷���ϴ�!"));

        // UI - "wave timer"
        waveTimer.SetActive(true);

        while (waveTime <= maxWaveTime)
        {
            yield return waitOneSec;
            waveTime++;
            spawnIntervalTime++;

            SetTimer(waveTime);

            if (timeBetweenSpawn <= spawnIntervalTime)
            {
                StartCoroutine(MonsterSpawn(waveTime));
                spawnIntervalTime = 0f;
            }
        }

        waveTime = 0f;
        ResetTimer();

        // anger remain time
        float angerRemainTotalTime = 0f;
        const float X = 1.17f;
        const float Y = 0.6f;

        // calculate anger remain time
        angerRemainTotalTime = spawnedEnemyList.Count() / (Mathf.Pow(X, stageWave) - Y);

        // UI - "Monsters Anger Warning"
        StartCoroutine(stageProgressUI.ShowProgress(2f, "����ü���� ȭ�� ���� �Ѵ�!!!"));

        // count down
        while (0 <= angerRemainTotalTime)
        {
            yield return waitOneSec;
            angerRemainTotalTime--;

            SetTimer(angerRemainTotalTime);
        }

        // UI - "Anger"
        StartCoroutine(stageProgressUI.ShowProgress(2f, "����ü���� ���������� �ֽ��ϴ�."));

        // make spawnedEnemyList anger
        foreach (var monster in spawnedEnemyList)
        {
            monster.GetComponent<EnemyEntity>().GetAnger();
        }

        yield return new WaitUntil(() => spawnedEnemyList.Count() == 0);

        // wave end
        WaveFin();
    }

    IEnumerator MonsterSpawn(float waveTime)
    {
        bool isFieldMax = false;
        int monsterSpawnNum = 0;
        int properMonsterFieldNum = 0;
        int eliteSpawnNum = 0;
        float M = stageWave * 2.5f + 1, m = stageWave * 1.7f;

        // calculate monster numbers to spawn
        eliteSpawnNum = (int)(-0.01 * (waveTime - 100) * (waveTime - 100)) + stageWave;
        eliteSpawnNum = (eliteSpawnNum < 0) ? 0 : eliteSpawnNum;

        monsterSpawnNum = (int)((m - M) / 1000);
        monsterSpawnNum = (int)((waveTime - 100) * (waveTime - 100)) + (int)M;

        properMonsterFieldNum = monsterSpawnNum + eliteSpawnNum - stageWave;

        // ����ؾ� �� ���� : ���� ���� ���� �ʵ� �ִ� ��ȯ���� �������� ���, �׳� [����Ʈ -> �߰� -> �⺻] ������� �����ϴٰ� ���߿� ���� ���ΰ�?
        // �ϴ� �̴�� �ϰ���

        // elite enemies
        if (eliteEnemiesSpawnList[stageWave].spawnableObjectRatioList.Count != 0)
        {
            isFieldMax = SpawnEnemy(eliteSpawnNum, eliteEnemySpawnHelperClass);
        }

        if (!isFieldMax)
        {
            // additional enemies
            isFieldMax = SpawnEnemy(properMonsterFieldNum - monsterSpawnNum, enemySpawnHelperClass);

            if (!isFieldMax)
            {
                // basic enemies
                SpawnEnemy(monsterSpawnNum, enemySpawnHelperClass);
            }
        }

        yield break;
    }

    private bool SpawnEnemy(int numberToSpawn, RandomSpawnableObject<GameObject> enemiesSpawnHelperClass)
    {
        bool isMax = false;

        for (int i = 0; i < numberToSpawn; ++i)
        {
            // check field monster numbers
            if (spawnedEnemyList.Count < maxFieldMonsterNum)
            {
                Debug.Log("�����ϱ�?");
                // monster spawn
                GameObject enemyPrefab = enemiesSpawnHelperClass.GetItem();
                Vector3 tempPosition = spawnPositions[i % spawnPositions.Count];
                spawnedEnemyList.Add(PoolManager.Instance.ReuseGameObject(enemyPrefab, tempPosition, Quaternion.identity));
            }
            else
            {
                Debug.Log($"{spawnedEnemyList.Count}�Դϴ�");
                isMax = true;
                break;
            }
        }
        return isMax;
    }

    private void WaveFin()
    {
        StopCoroutine(waveCoroutine);

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

    // �������� Ŭ���� ���� or ���� �� � ó���� �ؾ� �ϴ°�? �����ؾ� �� ������ �ִ°�?
    // IsClear ������ Stage���� ������ �δ� �� ���� ������?
    public void LoseStage()
    {
        StopAllCoroutines();
        waveTimer.SetActive(false);
        StartCoroutine(stageProgressUI.ShowResultWindow(2f));
    }

    public void ClearStage()
    {
        // boss�� onDead �Լ����� ����
        StopAllCoroutines();
        clearCount[currentStage.CodeName]++;
        waveTimer.SetActive(false);
        StartCoroutine(stageProgressUI.ShowResultWindow(2f));
    }

    public void SetTimer(float time)
    {
        int minute = 0, second = 0;

        minute = (int)time / 60;
        second = (int)time % 60;

        waveTimer.GetComponentInChildren<TMP_Text>().text = minute.ToString("00") + ":" + second.ToString("00");
    }

    public void ResetTimer()
    {
        waveTimer.GetComponentInChildren<TMP_Text>().text = "00:00";
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
