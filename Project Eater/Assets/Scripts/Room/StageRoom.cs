using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(PolygonCollider2D))]
public class StageRoom : Room
{
    private PolygonCollider2D polygonCollider2D;

    public PolygonCollider2D PolygonCollider2D => polygonCollider2D;

    private void Awake()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    protected override void Start()
    {
        base.Start();
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
}
