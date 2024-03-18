using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    [field: SerializeField]
    public Player player { get; private set; }
    public PlayerStatSO playerStat { get; private set; }
    public List<LatentSkillSO> latentSkillSO { get; private set; }
    public List<LatentSkill> latentSkillList { get; private set;}

    // 플레이어 레벨
    [field: SerializeField]
    public int playerLevel { get; private set; }

    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;

    private GameObject mapLevel;
    public Room[] roomArray { get; private set; } 

    protected override void Awake()
    {
        base.Awake();

        mapLevel = GameObject.Find("Level");

        // 플레이어 초기 스텟 가져오기
        playerStat = GameResources.Instance.playerStat;
        // 해방 스킬 데이터 가져오기
        latentSkillSO = GameResources.Instance.latentSkills;
    }

    private void Start()
    {
        roomArray = mapLevel.GetComponentsInChildren<Room>();

        //     

        // Player 생성
        InstantiatePlayer();
    }

    private void InstantiatePlayer()
    {
        // 플레이어 인스턴스화 
        GameObject playerPrefab = Instantiate(playerStat.playerPrefab);

        // Player 컴포넌트 가져오기 
        player = playerPrefab.GetComponent<Player>();

        // 플레이어 초기화
        // 1. 스텟 초기화
        // 2. 해방 스킬 등록 
    }

    // 해방 스킬 초기화 
    private void InitializeLatentSkill()
    {

    }

    // 플레이어 레벨 초기화
    public void InitializePlayerLevel() => playerLevel = 0;

    // 레벨업
    public void LevelUp() => playerLevel++;
}
