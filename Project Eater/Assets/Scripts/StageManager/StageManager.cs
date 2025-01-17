using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using static UnityEngine.Rendering.DebugUI;

public class StageManager : SingletonMonobehaviour<StageManager>
{
    [SerializeField]
    private GameObject waveTimer;
    [SerializeField]
    private GameObject skillInvetoryUI;

    private StageProgressUI stageProgressUI;
    private IReadOnlyList<SpawnableObjectsByWave<GameObject>> enemiesSpawnList;
    private IReadOnlyList<SpawnableObjectsByWave<GameObject>> eliteEnemiesSpawnList;
    private RandomSpawnableObject<GameObject> enemySpawnHelperClass;
    private RandomSpawnableObject<GameObject> eliteEnemySpawnHelperClass;
    private HashSet<GameObject> spawnedEnemyList;
    private List<Vector3> spawnPositions;
    private bool isRest = false;

    private const int maxStageWave = 10;
    private const int maxFieldMonsterNum = 120;
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
    public bool IsRest
    {
        get => isRest;
        set => isRest = value;
    }

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
            spawnedEnemyList = new HashSet<GameObject>();
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
                                            // 스테이지 입장 1초 후에 바로 몬스터 스폰되도록 4초로 설정

        // UI - "Wave Start"
        StartCoroutine(stageProgressUI.ShowProgress(2f, "실험체들이 달려듭니다!"));

        // UI - "wave timer"
        waveTimer.SetActive(true);

        // 2분 50초 동안 몬스터 스폰 Loop 실행
        while (waveTime <= maxWaveTime)
        {
            yield return waitOneSec; // 1초 대기 

            // 대기한 시간 만큼 시간 변수 증가
            waveTime++;         
            spawnIntervalTime++;

            // 타이머 설정
            SetTimer(waveTime);

            // 5초 마다 몬스터 스폰
            if (timeBetweenSpawn <= spawnIntervalTime)
            {
                StartCoroutine(MonsterSpawn(waveTime));
                spawnIntervalTime = 0f;
            }
        }

        waveTime = 0f;
        ResetTimer();

        // 광폭화 시간 계산
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

            // 몬스터 모두 처치 시, 스테이지 종료 처리 
            if (spawnedEnemyList.Count <= 0)
                WaveFin();
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

        int monsterSpawnNum = 0;                                // 기본 스폰
        int eliteSpawnNum = 0;                                  // 정예 몬스터 스폰(최종)
        int properMonsterFieldNum = 0;                          // 적정 몬스터(최종)

        float M = Mathf.Pow(1.295f, stageWave + 4) + 0.6f;
        float m = Mathf.Pow(1.2f, stageWave + 4) - 0.8f;

        // calculate monster numbers to spawn
        eliteSpawnNum = (int)(-0.001f * Mathf.Pow(waveTime - 80, 2) + 0.7f * stageWave - 2);
        eliteSpawnNum = Mathf.Max(eliteSpawnNum, 0);

        // 기본 스폰량 계산
        monsterSpawnNum = (int)(((m - M) / 10000f) * Mathf.Pow(waveTime - 100, 2) + M);

        // 적정 몬스터 스폰량 계산
        properMonsterFieldNum = (int)(monsterSpawnNum - 0.7f * stageWave);

        // elite enemies
        if (eliteEnemiesSpawnList[stageWave].spawnableObjectRatioList.Count != 0)
        {
            isFieldMax = SpawnEnemy(eliteSpawnNum, eliteEnemySpawnHelperClass);
        }

        if (!isFieldMax)
        {
            // additional enemies
            isFieldMax = SpawnEnemy(properMonsterFieldNum, enemySpawnHelperClass);

            if (!isFieldMax)
            {
                // 일반 몬스터 스폰
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
                // monster spawn
                GameObject enemyPrefab = enemiesSpawnHelperClass.GetItem();
                Vector3 tempPosition = spawnPositions[i % spawnPositions.Count];
                var go = PoolManager.Instance.ReuseGameObject(enemyPrefab, tempPosition, Quaternion.identity);
                go.GetComponent<MonsterAI>().SetEnemy(); // 몬스터 AI SetUp
                go.GetComponent<EnemyEntity>().onDead += RemoveEnemyFromList;
                spawnedEnemyList.Add(go);
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
        IsRest = true;

        if (stageWave < maxStageWave)
        {
            stageWave++;
            PlayerController.Instance.enabled = false;
            GameManager.Instance.CinemachineTarget.enabled = false;
            // 스킬 인벤토리 UI 띄우기 
            skillInvetoryUI.gameObject.SetActive(true);
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
        IsRest = false;
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

    private void RemoveEnemyFromList(Entity enemy)
    {
        if (spawnedEnemyList.Contains(enemy.gameObject))
        {
            spawnedEnemyList.Remove(enemy.gameObject);
        }
    }

    public void StartWaveCoroutine() => StartCoroutine(waveCoroutine);
}
