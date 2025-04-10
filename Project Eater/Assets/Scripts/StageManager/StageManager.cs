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
    #region Event
    public delegate void DeActivateItem();
    public event DeActivateItem onDeActivateItem;
    #endregion
    [SerializeField]
    private GameObject waveTimer;
    [SerializeField]
    private GameObject waveNoticeWindow;
    [SerializeField]
    private GameObject skillInvetoryUI;
    [SerializeField]
    private GameObject testWindow;
    [SerializeField]
    private DisplayBossInfo bossInfoUI;

    private StageProgressUI stageProgressUI;
    private IReadOnlyList<SpawnableObjectsByWave<GameObject>> enemiesSpawnList;
    private IReadOnlyList<SpawnableObjectsByWave<GameObject>> eliteEnemiesSpawnList;
    private RandomSpawnableObject<GameObject> enemySpawnHelperClass;
    private RandomSpawnableObject<GameObject> eliteEnemySpawnHelperClass;
    private HashSet<EnemyMovement> spawnedEnemyList;
    private List<Vector3> spawnPositions;
    private bool isRest = false;

    private const int maxStageWave = 10;
    private const int maxFieldMonsterNum = 90;
    private const float maxWaveTime = 90f;              // 1 min 30sec;
    private const float timeBetweenSpawn = 5f;

    public int stageWave { get; private set; }      // Current Stage wave

    private WaitForSeconds waitUIEffect;            // wait for UI effect
    private WaitForSeconds waitOneSec;            // wait for Timer

    public bool isCombat { get; private set; } = false;
    public bool isBossSpawned { get; private set; } = false;

    // 플레이어가 스테이지에서 획득한 바알의 살점 
    public int GetBaalFlesh
    {
        get { return (int)(Mathf.Pow(1.1f, stageWave) * KillCount / 5); }
        private set { }
    }
    // 킬 카운트
    public int KillCount { get; private set; }
    // 스테이지 클리어 여부 
    public bool IsClear { get; private set; }
    public bool IsRest
    {
        get => isRest;
        set => isRest = value;
    }

    public HashSet<EnemyMovement> SpawnedEnemyList => spawnedEnemyList;

    #region Stage
    [SerializeField]
    private Transform returnPosition;
    public Transform ReturnPosition => returnPosition;
    [SerializeField]
    private GameObject stageLevel;
    [SerializeField]
    private List<Stage> stages;        // 스테이지 목록
    [SerializeField]
    private List<BossPreSpawnEffect> bossPreSpawnEffects; // 보스 스폰 전 연출 
    [SerializeField]
    private float stageClearDelayTime = 5f;

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
    private GameObject boss;
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

        stageWave = 1;
    }

    public void StartWave()
    {
        StartCoroutine(ProgressWave());
    }

    IEnumerator ProgressWave()
    {
        yield return waitUIEffect;

        isCombat = true;

        float waveTime = 0f;
        float spawnIntervalTime = 4f;       // to spawn enemies when player enter the stage
                                            // 스테이지 입장 1초 후에 바로 몬스터 스폰되도록 4초로 설정

        // UI - "Test Buttons"
        // testWindow.SetActive(true);

        // UI - "Wave Start"
        waveNoticeWindow.GetComponentInChildren<TMP_Text>().text = $"Wave {stageWave}";
        waveNoticeWindow.SetActive(true);
        StartCoroutine(stageProgressUI.ShowProgress(2f, "실험체들이 달려듭니다!"));

        // UI - "wave timer"
        waveTimer.SetActive(true);

        // Stage마다 1초당 주인공 체력 감소 실행 - 포만감이 줄어들어 허기짐을 나타냄
        StartCoroutine(DecreaseFullness(Mathf.Pow(1.17f, stageWave) - 0.7f));
        // 몬스터 분리 코루틴 실행
        SeparationManager.Instance.StartSeparationForAllEnemies();

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
        StartCoroutine(stageProgressUI.ShowProgress(2f, "실험체들이 난폭해지려 합니다."));

        // count down
        while (0 <= angerRemainTotalTime)
        {
            yield return waitOneSec;
            angerRemainTotalTime--;

            SetTimer(angerRemainTotalTime);

            // 몬스터 모두 처치 시, 스테이지 종료 처리 
            if (spawnedEnemyList.Count <= 0)
            {
                StopAllCoroutines();
                StartCoroutine(WaveFin());
            }
        }

        // UI - "Anger"
        StartCoroutine(stageProgressUI.ShowProgress(2f, "실험체들이 난폭해지고 있습니다."));

        // Monster Roar 효과음 재생
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.monsterRoar);

        // make spawnedEnemyList anger
        foreach (var monster in spawnedEnemyList)
        {
            monster.GetComponent<EnemyEntity>().GetAnger();
        }

        yield return new WaitUntil(() => spawnedEnemyList.Count() <= 0);

        // wave end
        StopAllCoroutines();
        StartCoroutine(WaveFin());
    }

    IEnumerator DecreaseFullness(float amount)
    {
        // 포만감 감소 안내 문구 띄우기

        // 웨이브 시작 후 5초 동안 포만감 감소 없음
        yield return new WaitForSeconds(5f);

        while (true)
        {
            yield return waitOneSec;

            GameManager.Instance.player.DecreaseFullness(amount);
        }
    }

    IEnumerator MonsterSpawn(float waveTime)
    {
        bool isFieldMax = false;

        int monsterSpawnNum = 0;                                // 기본 스폰
        int eliteSpawnNum = 0;                                  // 정예 몬스터 스폰(최종)
        int properMonsterFieldNum = 0;                          // 적정 몬스터(최종)

        float M = Mathf.Pow(1.24f, stageWave + 4) + 0.7f;
        float m = Mathf.Pow(1.16f, stageWave + 3) - 0.4f;

        // calculate monster numbers to spawn
        // eliteSpawnNum = 1; 
        eliteSpawnNum = Mathf.RoundToInt(-0.0018f * Mathf.Pow(waveTime - 60, 2) + (0.9f * stageWave) - 2.6f); // f 다 붙여야함
        eliteSpawnNum = Mathf.Max(eliteSpawnNum, 0);

        // 기본 스폰량 계산
        // monsterSpawnNum = 0;
        monsterSpawnNum = (int)(((m - M) / 2500) * Mathf.Pow(waveTime - 50, 2) + M);

        // 적정 몬스터 스폰량 계산
        // properMonsterFieldNum = 0;
        properMonsterFieldNum = (int)(monsterSpawnNum - 0.7f * stageWave);

        // elite enemies
        if (eliteEnemiesSpawnList[stageWave - 1].spawnableObjectRatioList.Count != 0)
        {
            isFieldMax = SpawnEnemy(eliteSpawnNum, eliteEnemySpawnHelperClass);
        }

        // 일반 몬스터 스폰
        if (!isFieldMax)
        {
            SpawnEnemy(monsterSpawnNum, enemySpawnHelperClass);

            if (!isFieldMax)
            {
                // additional enemies
                isFieldMax = SpawnEnemy(properMonsterFieldNum, enemySpawnHelperClass);
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

                int randomNumber = (int)UnityEngine.Random.Range(0.0f, 100.0f);
                Vector3 spawnPosition = spawnPositions[randomNumber % spawnPositions.Count];

                var enemyObject = PoolManager.Instance.ReuseGameObject(enemyPrefab, spawnPosition, Quaternion.identity);
                enemyObject.GetComponent<MonsterAI>()?.SetEnemy(stageWave, CurrentStage.StageNumber); // 몬스터 AI SetUp
                enemyObject.GetComponent<EnemyEntity>().onDead += RemoveEnemyFromList;
                enemyObject.GetComponent<EnemyEntity>().onDead += IncreaseKillCount;
                spawnedEnemyList.Add(enemyObject.GetComponent<EnemyMovement>());
            }
            else
            {
                Debug.Log($"{spawnedEnemyList.Count}, Max to Spawn");
                isMax = true;
                break;
            }
        }
        return isMax;
    }

    private IEnumerator WaveFin()
    {
        SeparationManager.Instance.StopSeparationForAllEnemies();
        ResetTimer();
        IsRest = true;
        stageProgressUI.ProgressNoticeWindow.SetActive(false); // 테스트 코드 

        // 모든 몬스터 처치되고 2초 후 웨이브 종료
        yield return new WaitForSeconds(3f);

        if (stageWave < maxStageWave)
        {
            stageWave++;
            PlayerController.Instance.enabled = false;
            GameManager.Instance.CinemachineTarget.enabled = false;

            // TODO : 안내 문구 띄우기 

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
        var effect = bossPreSpawnEffects[currentStage.StageNumber];
        if (effect != null)
        {
            effect.OnBossSpawnRequested += HandleBossSpawn; // 이벤트 구독
            effect.PlayEffect();
        }
    }

    private void HandleBossSpawn()
    {
        // 보스 전투 BGM 재생
        MusicManager.Instance.PlayMusic(GameResources.Instance.bossBattleMusic);
        // 보스 플래그 활성화 
        isBossSpawned = true;

        var effect = bossPreSpawnEffects[currentStage.StageNumber];

        // 현재 스테이지에서 보스 정보 가져오기
        var bossPrefab = currentStage.StageBoss;
        var spawnPosition = effect.gameObject.transform.position;

        boss = PoolManager.Instance.ReuseGameObject(bossPrefab, spawnPosition, Quaternion.identity);
        boss.GetComponent<BossAI>()?.SetEnemy(0, 0);
        boss.GetComponent<Entity>().onDead += StartDelayedClearStage;

        // 기존 웨이브 UI 비활성화
        waveTimer.SetActive(false);
        waveNoticeWindow.SetActive(false);
        // bossInfo UI 띄우기 
        bossInfoUI.Show(boss.GetComponent<BossEntity>());
        StartCoroutine(stageProgressUI.ShowProgress(2f, "바알의 힘이 각성하여 이제 포만감이 감소하지 않습니다.\n싸우세요!"));

        effect.OnBossSpawnRequested -= HandleBossSpawn;
    }

    private void StartDelayedClearStage(Entity enemy, bool isRealDead)
    {
        if (!isRealDead) return;

        StartCoroutine(DelayedClearStage());
    }

    private IEnumerator DelayedClearStage()
    {
        yield return new WaitForSeconds(stageClearDelayTime);
        ClearStage();
    }

    public void LoseStage()
    {
        // 게임 오버 BGM 재생
        MusicManager.Instance.PlayMusic(GameResources.Instance.loseMusic);

        StopAllCoroutines();
        waveTimer.SetActive(false);
        waveNoticeWindow.SetActive(false);
        stageProgressUI.ProgressNoticeWindow.SetActive(false); // 테스트 용

        // test UI
        testWindow.SetActive(false);

        // 모든 몬스터 비활성화
        spawnedEnemyList.RemoveWhere(spawnedEnemy =>
        {
            spawnedEnemy.Owner.TakeDamage(null, null, 10000, false, false, false, false);
            return true; // 모든 요소 삭제
        });
        spawnedEnemyList.Clear();
        // 보스가 살아있으면 비활성화 해주기
        if (boss)
        {
            var bossEntity = boss.GetComponent<BossEntity>();
            bossEntity.TakeDamage(null, null, 100000, false, false, false, false);
            bossInfoUI.gameObject.SetActive(false);
        }

        ClearEquipSlots();
        ClearFieldItems();

        StartCoroutine(stageProgressUI.ShowResultWindow(2f));
    }

    public void ClearStage()
    {
        // 스테이지 클리어 BGM 재생
        MusicManager.Instance.PlayMusic(GameResources.Instance.winMusic);

        // 첫 번째 스테이지, 첫 번째 클리어 라면 시그마 대화 분기 변동
        if (currentStage.StageNumber == 0 && currentStage.ClearCount >= 0 
            && GameManager.Instance.sigma.Affinity == 2)
            GameManager.Instance.sigma.Affinity = 3;

        StopAllCoroutines();
        IsClear = true;
        UpClearCount();
        waveTimer.SetActive(false);
        waveNoticeWindow.SetActive(false);
        bossInfoUI.gameObject.SetActive(false);
        stageProgressUI.ProgressNoticeWindow.SetActive(false); // 테스트 용

        // 몬스터가 혹시라도 남아있다면 비활성화 - 테스트 용
        spawnedEnemyList.RemoveWhere(spawnedEnemy =>
        {
            spawnedEnemy.Owner.TakeDamage(null, null, 10000, false, false, false, false);
            return true; // 모든 요소 삭제
        });
        spawnedEnemyList.Clear();
        // 보스가 살아있으면 비활성화 해주기 - 테스트 용
        if (boss)
        {
            var bossEntity = boss.GetComponent<BossEntity>();
            bossEntity.TakeDamage(null, null, 100000, false, false, false, false);
        }

        ClearEquipSlots();
        ClearFieldItems();

        StartCoroutine(stageProgressUI.ShowResultWindow(2f));
    }

    private void UpClearCount()
    {
        currentStage.ClearCount++;
    }

    private void ClearEquipSlots()
    {
        // 스킬 장착 칸 초기화
        foreach (var slot in GameManager.Instance.EquipActiveSlots)
        {
            slot.StageEnd();
        }
        foreach (var slot in GameManager.Instance.EquipPassiveSlots)
        {
            slot.StageEnd();
        }
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

    public void ResetVariable()
    {
        stageWave = 1;
        GetBaalFlesh = 0;
        KillCount = 0;
        IsClear = false;
        isCombat = false;
        isBossSpawned = false;
        ResetTimer();
    }

    private void RemoveEnemyFromList(Entity enemy, bool isRealDead)
    {
        if (spawnedEnemyList.Contains(enemy.GetComponent<EnemyMovement>()))
        {
            spawnedEnemyList.Remove(enemy.GetComponent<EnemyMovement>());
        }
    }

    private void IncreaseKillCount(Entity enemy, bool isRealDead)
    {
        if (!isRealDead) return;

        KillCount++;
    }

    public void StartWaveCoroutine() => StartCoroutine(ProgressWave());

    // 테스트용 승리 버튼
    public void OnClearStage()
    {
        GameManager.Instance.player.StageClear();
        GameManager.Instance.player.gameObject.SetActive(false);

        stageWave = 11;
        ClearStage();
    }

    // 테스트용 패배 버튼
    public void OnDefeatStage()
    {
        GameManager.Instance.player.TakeDamage(null, null, 1000, false, false);
    }

    // 테스트용 보스전 버튼
    public void OnSkipToBoss()
    {
        // 모든 몬스터 비활성화 
        spawnedEnemyList.RemoveWhere(spawnedEnemy =>
        {
            spawnedEnemy.Owner.TakeDamage(null, null, 10000, false, false, false, false);
            return true; // 모든 요소 삭제
        });
        spawnedEnemyList.Clear();

        stageWave = maxStageWave;
        StopAllCoroutines();
        StartCoroutine(WaveFin());
    }

    // 테스트용 웨이브 스킵 버튼
    public void OnSkipWave()
    {
        // 모든 몬스터 비활성화 
        spawnedEnemyList.RemoveWhere(spawnedEnemy =>
        {
            spawnedEnemy.Owner.TakeDamage(null, null, 10000, false, false, false, false);
            return true; // 모든 요소 삭제
        });
        spawnedEnemyList.Clear();
        StopAllCoroutines();
        StartCoroutine(WaveFin());
    }

    public void ClearFieldItems()
    {
        onDeActivateItem?.Invoke();
    }
}
