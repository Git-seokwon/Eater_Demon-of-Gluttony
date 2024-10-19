using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    [field: SerializeField]
    public PlayerEntity player { get; private set; }

    #region ROOM
    [SerializeField]
    private Room currentStage;
    #endregion

    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;

    private GameObject mapLevel;
    public Room[] roomArray { get; private set; }

    #region ����ġ
    // �÷��̾� ����
    public int playerLevel { get; private set; }
    // �÷��̾� ����ġ 
    private int exp;
    private float nextExp;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        mapLevel = GameObject.Find("Level");
    }

    private void Start()
    {
        roomArray = mapLevel.GetComponentsInChildren<Room>(); 
    }

    // �÷��̾� ���� �� ����ġ �ʱ�ȭ
    public void InitializePlayer()
    {
        playerLevel = 0;
        nextExp = Mathf.FloorToInt(player.Stats.ExpStat.Value);
    }

    public Vector2 GetPlayerPosition()
    {
        return player.transform.position;
    }

    public Room GetCurrentRoom()
    {
        return currentStage;
    }

    public void GetExp()
    {
        exp++;

        if (exp >= nextExp)
        {
            playerLevel++;
            exp = 0;
            // TODO : ���Ŀ� ���� nextExp�� ���� �����Ѵ�. 
            // nextExp = 
            // TODO : ������ UI�� Show �Ѵ�. 
        }
    }
}
