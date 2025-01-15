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

    public int stageWave { get; private set; }      // 현재 스테이지의 wave

    private IEnumerator waveCoroutine;
    private WaitForSeconds waitUIEffect;            // wait for UI effect
    private WaitForSeconds waitOneSec;            // wait for Timer

    // 플레이어가 스테이지에서 획득한 바알의 살점 
    public int GetBaalFlesh {  get; private set; }
    // 킬 카운트
    public int KillCount { get; private set; }
    // 스테이지 클리어 여부 
    public bool IsClear { get; private set; }

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

    // Stage가 변경되면 event가 발생하여 currentRoom도 같이 변경된다. 
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
        // 임시로 넣음
        clearCount.Add(currentStage.CodeName, 0);

        StartCoroutine(waveCoroutine);
        Debug.Log("Start Wave of" + $" {currentStage.StageRoom.name}!");
    }

    // GameManager - DisplayMessageRoutine 코루틴에 짜여져 있는 타이머. yield return null;이 다음 update문 호출까지 기다린다고 한다. 이런 게 있는데 왜 서장원은 말을 안 ㅎ
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
        StartCoroutine(stageProgressUI.ShowProgress(2f, "실험체들이 달려듭니다!"));

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
        StartCoroutine(stageProgressUI.ShowProgress(2f, "실험체들이 화가 나려 한다!!!"));

        // count down
        while (0 <= angerRemainTotalTime)
        {
            yield return waitOneSec;
            angerRemainTotalTime--;

            SetTimer(angerRemainTotalTime);
        }

        // UI - "Anger"
        StartCoroutine(stageProgressUI.ShowProgress(2f, "실험체들이 난폭해지고 있습니다."));

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

        // 고려해야 할 사항 : 몬스터 스폰 도중 필드 최대 소환량을 충족했을 경우, 그냥 [엘리트 -> 추가 -> 기본] 순서대로 스폰하다가 도중에 멈출 것인가?
        // 일단 이대로 하겠음

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
                Debug.Log("문제일까?");
                // monster spawn
                GameObject enemyPrefab = enemiesSpawnHelperClass.GetItem();
                Vector3 tempPosition = spawnPositions[i % spawnPositions.Count];
                spawnedEnemyList.Add(PoolManager.Instance.ReuseGameObject(enemyPrefab, tempPosition, Quaternion.identity));
            }
            else
            {
                Debug.Log($"{spawnedEnemyList.Count}입니다");
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

    // 스테이지 클리어 성공 or 실패 시 어떤 처리를 해야 하는가? 변경해야 할 변수가 있는가?
    // IsClear 변수는 Stage별로 저장해 두는 게 좋지 않은가?
    public void LoseStage()
    {
        StopAllCoroutines();
        waveTimer.SetActive(false);
        StartCoroutine(stageProgressUI.ShowResultWindow(2f));
    }

    public void ClearStage()
    {
        // boss의 onDead 함수에서 실행
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
