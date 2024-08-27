using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    [field: SerializeField]
    public PlayerEntity player { get; private set; }

    // 플레이어 레벨
    [field: SerializeField]
    public int playerLevel { get; private set; }

    #region ROOM
    private Room currentStage;
    #endregion

    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;

    private GameObject mapLevel;
    public Room[] roomArray { get; private set; } 

    protected override void Awake()
    {
        base.Awake();

        mapLevel = GameObject.Find("Level");
    }

    private void Start()
    {
        roomArray = mapLevel.GetComponentsInChildren<Room>(); 
    }


    // 해방 스킬 초기화 
    private void InitializeLatentSkill()
    {

    }

    // 플레이어 레벨 초기화
    public void InitializePlayerLevel() => playerLevel = 0;

    // 레벨업
    public void LevelUp() => playerLevel++;

    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    public void SetCurrentStage(Room stage)
    {
        currentStage = stage;
    }

    public Room GetCurrentRoom()
    {
        return currentStage;
    }
}
