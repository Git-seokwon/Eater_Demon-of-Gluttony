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
    private HashSet<EnemyMovement> spawnedEnemyList;
    private List<Vector3> spawnPositions;
    private bool isRest = false;

    private const int maxStageWave = 10;
    private const int maxFieldMonsterNum = 120;
    private const float maxWaveTime = 170f;              // 2 min 50 sec;
    private const float timeBetweenSpawn = 5f;

    public int stageWave { get; private set; }      // 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙占쏙옙 wave

    private IEnumerator waveCoroutine;
    private WaitForSeconds waitUIEffect;            // wait for UI effect
    private WaitForSeconds waitOneSec;            // wait for Timer

    // �댁닿 ㅽ댁  諛 댁 
    public int GetBaalFlesh {  get; private set; }
    //  移댁댄
    public int KillCount { get; private set; }
    // ㅽ댁 대━ щ 
    public bool IsClear { get; private set; }
    public bool IsRest
    {
        get => isRest;
        set => isRest = value;
    }

    public HashSet<EnemyMovement> SpawnedEnemyList => spawnedEnemyList;

    // ㅽ댁 대━ 瑜 �ν 猷援ъ“
    //  key : Stage CodeName, Value : Clear 
    private Dictionary<string, int> clearCount = new Dictionary<string, int>();
    public IReadOnlyDictionary<string, int> ClearCount => clearCount;

    #region Stage
    [SerializeField]
    private Transform returnPosition;
    public Transform ReturnPosition => returnPosition;
    [SerializeField]
    private GameObject stageLevel;
    [SerializeField]
    private List<Stage> stages;        // ㅽ댁 紐⑸
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

    // Stage媛 蹂寃쎈硫 event媛 諛 currentRoom 媛 蹂寃쎈. 
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
            foreach (var enemy in spawnedEnemyList)
            {
                enemy.gameObject.SetActive(false);
            }
        }
        else if (spawnedEnemyList == null)
        {
            spawnedEnemyList = new HashSet<EnemyMovement>();
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
        // 濡 ｌ
        clearCount.Add(currentStage.CodeName, 0);

        StartCoroutine(waveCoroutine);
        Debug.Log("Start Wave of" + $" {currentStage.StageRoom.name}!");
    }

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
                                            // ㅽ댁  1珥  諛濡 紐ъㅽ ㅽ곕濡 4珥濡 ㅼ

        // UI - "Wave Start"
        StartCoroutine(stageProgressUI.ShowProgress(2f, "ㅽ泥대ㅼ щㅻ�!"));

        // UI - "wave timer"
        waveTimer.SetActive(true);

        // Stage留 1珥 二쇱멸났 泥대 媛 ㅽ - щ媛 以대ㅼ 湲곗 
        StartCoroutine(DecreaseFullness(Mathf.Pow(1.3f, stageWave) + 0.3f));
        // 紐ъㅽ 遺由 肄猷⑦ ㅽ
        SeparationManager.Instance.StartSeparationForAllEnemies();

        // 2遺 50珥  紐ъㅽ ㅽ Loop ㅽ
        while (waveTime <= maxWaveTime)
        {
            yield return waitOneSec; // 1珥 湲 

            // 湲고 媛 留 媛 蹂 利媛
            waveTime++;         
            spawnIntervalTime++;

            // 대㉧ ㅼ
            SetTimer(waveTime);

            // 5珥 留 紐ъㅽ ㅽ
            if (timeBetweenSpawn <= spawnIntervalTime)
            {
                StartCoroutine(MonsterSpawn(waveTime));
                spawnIntervalTime = 0f;
            }
        } 

        waveTime = 0f;
        ResetTimer();

        // 愿� 媛 怨
        float angerRemainTotalTime = 0f;
        const float X = 1.17f;
        const float Y = 0.6f;

        // calculate anger remain time
        angerRemainTotalTime = spawnedEnemyList.Count() / (Mathf.Pow(X, stageWave) - Y);

        // UI - "Monsters Anger Warning"
        StartCoroutine(stageProgressUI.ShowProgress(2f, "占쏙옙占쏙옙체占쏙옙占쏙옙 화占쏙옙 占쏙옙占쏙옙 占싼댐옙!!!"));

        // count down
        while (0 <= angerRemainTotalTime)
        {
            yield return waitOneSec;
            angerRemainTotalTime--;

            SetTimer(angerRemainTotalTime);

            // 紐ъㅽ 紐⑤ 泥移 , ㅽ댁 醫猷 泥由 
            if (spawnedEnemyList.Count <= 0)
                WaveFin();
        }

        // UI - "Anger"
        StartCoroutine(stageProgressUI.ShowProgress(2f, "ㅽ泥대ㅼ �댁怨 듬."));

        // make spawnedEnemyList anger
        foreach (var monster in spawnedEnemyList)
        {
            monster.GetComponent<EnemyEntity>().GetAnger();
        }

        yield return new WaitUntil(() => spawnedEnemyList.Count() == 0);

        // wave end
        WaveFin();
    }

    IEnumerator DecreaseFullness(float amount)
    {
        while (true)
        {
            yield return waitOneSec;

            GameManager.Instance.player.DecreaseFullness(amount);
        }
    }

    IEnumerator MonsterSpawn(float waveTime)
    {
        bool isFieldMax = false;

        int monsterSpawnNum = 0;                                // 湲곕낯 ㅽ
        int eliteSpawnNum = 0;                                  // � 紐ъㅽ ㅽ(理醫)
        int properMonsterFieldNum = 0;                          // �� 紐ъㅽ(理醫)

        float M = Mathf.Pow(1.295f, stageWave + 4) + 0.6f;
        float m = Mathf.Pow(1.2f, stageWave + 4) - 0.8f;

        // calculate monster numbers to spawn
        eliteSpawnNum = (int)(-0.001f * Mathf.Pow(waveTime - 80, 2) + 0.7f * stageWave - 2);
        eliteSpawnNum = Mathf.Max(eliteSpawnNum, 0);

        // 湲곕낯 ㅽ곕 怨
        monsterSpawnNum = (int)(((m - M) / 10000f) * Mathf.Pow(waveTime - 100, 2) + M);

        // �� 紐ъㅽ ㅽ곕 怨
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
                // 쇰 紐ъㅽ ㅽ
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
                go.GetComponent<MonsterAI>()?.SetEnemy(stageWave, CurrentStage.StageNumber); // 紐ъㅽ AI SetUp
                go.GetComponent<EnemyEntity>().onDead += RemoveEnemyFromList;
                spawnedEnemyList.Add(go.GetComponent<EnemyMovement>());
            }
            else
            {
                Debug.Log($"{spawnedEnemyList.Count}");
                isMax = true;
                break;
            }
        }
        return isMax;
    }

    private void WaveFin()
    {
        StopCoroutine(waveCoroutine);
        SeparationManager.Instance.StopSeparationForAllEnemies();
        IsRest = true;

        if (stageWave < maxStageWave)
        {
            stageWave++;
            PlayerController.Instance.enabled = false;
            GameManager.Instance.CinemachineTarget.enabled = false;
            // ㅽ 몃깽由 UI 곌린 
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

    // ㅽ댁 대━ 깃났 or ㅽ  대 泥由щ� 댁 媛? 蹂寃쏀댁  蹂媛 媛?
    // IsClear 蹂 Stage蹂濡 �ν  寃 醫吏 媛?
    public void LoseStage()
    {
        StopAllCoroutines();
        waveTimer.SetActive(false);

        // 紐⑤ 紐ъㅽ 鍮깊 
        foreach (var spawnedEnemy in spawnedEnemyList)
        {
            spawnedEnemy.gameObject.SetActive(false);
        }
        spawnedEnemyList.Clear();

        StartCoroutine(stageProgressUI.ShowResultWindow(2f));
    }

    public void ClearStage()
    {
        // boss onDead ⑥ ㅽ
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
        isRest = false;

        if (!isReStart)
            CurrentStage = null;
    }

    private void RemoveEnemyFromList(Entity enemy)
    {
        if (spawnedEnemyList.Contains(enemy.GetComponent<EnemyMovement>()))
        {
            spawnedEnemyList.Remove(enemy.GetComponent<EnemyMovement>());
        }
    }

    public void StartWaveCoroutine() => StartCoroutine(waveCoroutine);
}
