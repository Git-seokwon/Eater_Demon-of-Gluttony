using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class MainRoom : Room
{
    private RoomLightingController roomLighting;

    protected override void Awake()
    {
        base.Awake();

        roomLighting = GetComponent<RoomLightingController>();
    }

    protected override void Start()
    {
        base.Start();

        groundTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.darkMaterial;
        shadowTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.darkMaterial;
        decorationTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.darkMaterial;
        frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.darkMaterial;

        for (int i = 0; i < roomLighting.NPC.Length; i++)
        {
            roomLighting.NPC[i].GetComponent<SpriteRenderer>().material = GameResources.Instance.darkMaterial;
        }

        DeActivateEnvironmentGameObject();
    }

    protected override void PopulateTilemapMemberVariable()
    {
        GameObject room = gameObject;

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

                default:
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            roomLighting.RoomEnter();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            roomLighting.RoomEnter();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            roomLighting.RoomExit();
        }
    }
}