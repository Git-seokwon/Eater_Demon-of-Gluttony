using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    [field: SerializeField]
    public Player player { get; private set; }

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
}
