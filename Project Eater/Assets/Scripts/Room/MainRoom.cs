using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class MainRoom : Room
{
    [Space(10)]
    [Header("MainRoomTemplateSO")]
    [Tooltip("Populate MainRoomTemplateSO")]
    [SerializeField] private MainRoomTemplateSO mainRoom;

    public bool isLit = false; // Light À¯¹«

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void PopulateTilemapMemberVariable()
    {
        base.PopulateTilemapMemberVariable();

        GameObject room = mainRoom.prefab;

        grid = room.GetComponentInChildren<Grid>();

        Tilemap[] tilemaps = room.GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
        {
            switch (tilemap.gameObject.tag)
            {
                case "Ground":
                    groundTilemap = tilemap;
                    break;

                case "Shadow":
                    shadowTilemap = tilemap;
                    break;

                case "Decoration":
                    decorationTilemap = tilemap;
                    break;

                case "Front":
                    frontTilemap = tilemap;
                    break;

                case "Collision":
                    collisionTilemap = tilemap;
                    break;

                case "Minimap":
                    minimapTilemap = tilemap;
                    break;

                default:
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            StaticEventHandler.CallRoomEnterEvent(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            StaticEventHandler.CallRoomExitEvent(this);
        }
    }
}