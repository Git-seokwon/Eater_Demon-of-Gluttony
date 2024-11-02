using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : SingletonMonobehaviour<StageManager>
{
    private enum StageState { NONE = -1, ENTER = 0, SPAWN, FIGHT, BREAK }

    private int stageWave;      // ���� ���������� wave
    private int maxStageWave;
    private float waveTime;
    private float maxWaveTime;
    private float timeBetweenWaves;

    private StageState stageState;        // ���� �������� ����
    private WaitForSeconds wait;

    private GameObject player;
    private GameObject stageRoom;       // ���� �������� ��

    #region Stage
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

            currentStage = value;
            currentRoom = stageLevel.transform.Find(currentStage.StageRoom.name).GetComponent<Room>();
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
    }

    void Start()
    {
        stageState = StageState.NONE;
        wait = new WaitForSeconds(3.0f);
        
        maxStageWave = 10;
        maxWaveTime = 195f;              // 3�� 15��
        timeBetweenWaves = 3f;

        waveTime = timeBetweenWaves;
        stageWave = 1;

        rooms = mapLevel.GetComponentsInChildren<Room>().ToList();
    }

    void Update()
    {
        // �������� ���� �̺�Ʈ �߻�
        // ���η� ��Ȱ��ȭ �� Ư�� �������� Ȱ��ȭ
        // �÷��̾� ����

        //if (stageState == StageState.NONE)        // �÷��̾� ���� �˻�
        //{
        //    IsPlayerEnter();
        //}
        //else
        //{
        //    WaveManage();
        //}
    }

    private void WaveManage()
    {
        if (stageState == StageState.FIGHT)
        {
            if (!EnemiesAreAlive())
            {
                WaveFin();
            }
        }

        if (waveTime <= 0)
        {
            if (stageState != StageState.SPAWN)
            {
                StartCoroutine(MonsterSpawn());
            }
        }
        else
        {
            waveTime -= Time.deltaTime;
        }
    }

    public bool IsPlayerEnter()
    {
        // �÷��̾� ������ Enter & true
        stageState = StageState.ENTER;
        return true;

        // �ƴϸ� �״�� & false
    }

    private void WaveFin()
    {
        stageState = StageState.BREAK;

        waveTime = timeBetweenWaves;

        if (stageWave < maxStageWave)
        {
            stageWave++;
        }
        else
        {
            // Boss Wave
            stageWave++;        // stageWave = 11
        }
    }

    private void MonsterFurious()     // waveTime�� 0���� �۰ų� ������ �̺�Ʈ�� ȣ��Ǵ� �Լ�
    {
        // ����ִ� ���� ���� ����ȭ (���ݷ� +30%, �̵��ӵ� +50%)
    }

    IEnumerator MonsterSpawn()
    {
        stageState = StageState.SPAWN;

        // monster spawn

        // or

        // boss spawn

        waveTime = maxWaveTime;

        stageState = StageState.FIGHT;

        yield break;
    }

    public void FinishGame()        // �÷��̾� Ȥ�� ������ �׾��� �� �̺�Ʈ�� ȣ��Ǵ� �Լ�
    {

    }

    private bool EnemiesAreAlive()
    {
        bool isExisting = false;

        return isExisting;
    }

    public int GetCurrentStageWave()
    {
        return stageWave;
    }
}
