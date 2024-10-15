using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingletonMonobehaviour<StageManager>
{
    private enum StageState { NONE = -1, ENTER = 0, SPAWN, FIGHT, BREAK }

    private int stageWave;      // 현재 스테이지의 wave
    private int maxStageWave;
    private float waveTime;
    private float maxWaveTime;
    private float timeBetweenWaves;

    private StageState stageState;        // 현재 스테이지 상태
    private WaitForSeconds wait;

    private GameObject player;
    private GameObject stageRoom;       // 현재 스테이지 룸

    public List<StageRoomTemplateSO> stageTemplates;        // 스테이지 목록

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        stageState = StageState.NONE;
        wait = new WaitForSeconds(3.0f);
        
        maxStageWave = 10;
        maxWaveTime = 195f;              // 3분 15초
        timeBetweenWaves = 3f;

        waveTime = timeBetweenWaves;
        stageWave = 1;
    }

    void Update()
    {
        // 스테이지 선택 이벤트 발생
        // 메인룸 비활성화 및 특정 스테이지 활성화
        // 플레이어 입장

        //if (stageState == StageState.NONE)        // 플레이어 입장 검사
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
        // 플레이어 들어오면 Enter & true
        stageState = StageState.ENTER;
        return true;

        // 아니면 그대로 & false
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

    private void MonsterFurious()     // waveTime이 0보다 작거나 같으면 이벤트로 호출되는 함수
    {
        // 살아있는 몬스터 전부 광폭화 (공격력 +30%, 이동속도 +50%)
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

    public void FinishGame()        // 플레이어 혹은 보스가 죽었을 때 이벤트로 호출되는 함수
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
