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

    // �÷��̾� ����
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

        // �÷��̾� �ʱ� ���� ��������
        playerStat = GameResources.Instance.playerStat;
        // �ع� ��ų ������ ��������
        latentSkillSO = GameResources.Instance.latentSkills;
    }

    private void Start()
    {
        roomArray = mapLevel.GetComponentsInChildren<Room>();

        //     

        // Player ����
        InstantiatePlayer();
    }

    private void InstantiatePlayer()
    {
        // �÷��̾� �ν��Ͻ�ȭ 
        GameObject playerPrefab = Instantiate(playerStat.playerPrefab);

        // Player ������Ʈ �������� 
        player = playerPrefab.GetComponent<Player>();

        // �÷��̾� �ʱ�ȭ
        // 1. ���� �ʱ�ȭ
        // 2. �ع� ��ų ��� 
    }

    // �ع� ��ų �ʱ�ȭ 
    private void InitializeLatentSkill()
    {

    }

    // �÷��̾� ���� �ʱ�ȭ
    public void InitializePlayerLevel() => playerLevel = 0;

    // ������
    public void LevelUp() => playerLevel++;
}
