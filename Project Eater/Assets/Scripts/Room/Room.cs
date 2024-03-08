using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [HideInInspector] public Grid grid; 
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap shadowTilemap;
    [HideInInspector] public Tilemap decorationTilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;

    #region HEADER OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the environment child placeholder gameobject")]
    #endregion
    [SerializeField] private GameObject environmentGameObejct;

    protected BoxCollider2D boxCollider2D;

    protected virtual void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    protected virtual void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        PopulateTilemapMemberVariable();

        DisableCollisionTilemapRenderer();
    }

    protected virtual void PopulateTilemapMemberVariable()
    {
        
    }

    private void DisableCollisionTilemapRenderer()
    {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }

    public void ActivateEnvironmentGameObject()
    {
        if (environmentGameObejct != null)
        {
            environmentGameObejct.SetActive(true);
        }
    }

    public void DeActivateEnvironmentGameObject()
    {
        if (environmentGameObejct != null)
        {
            environmentGameObejct.SetActive(false);
        }
    }
}
