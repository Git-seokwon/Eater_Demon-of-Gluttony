using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class RoomLightingController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] npc;

    private bool isLit = false; // Light À¯¹«
    private MainRoom mainRoom;

    public GameObject[] NPC => npc;

    private void Awake()
    {
        mainRoom = GetComponent<MainRoom>();
    }

    public void RoomEnter()
    {
        if (!isLit)
        {
            FadeInRoomLighting();
            mainRoom.ActivateEnvironmentGameObject();
            FadeInEnvironmentLighting();
            StartCoroutine(FadeInNPCLighting());
            isLit = true;
        }
    }
    public void RoomExit()
    {
        if (isLit)
        {
            FadeOutRoomLighting();
            mainRoom.DeActivateEnvironmentGameObject();
            FadeOutEnvironmentLighting();
            StartCoroutine(FadeOutNPCLighting());
            isLit = false;
        }
    }

    private void FadeInRoomLighting()
    {
        StartCoroutine(FadeInRoomLightingRoutine(mainRoom));
    }

    private void FadeOutRoomLighting()
    {
        StartCoroutine(FadeOutRoomLightingRoutine(mainRoom));
    }

    private IEnumerator FadeInRoomLightingRoutine(MainRoom mainRoom)
    {
        Material material = new Material(GameResources.Instance.variableLitShader);

        LoadTilemapRenderer(mainRoom, material);

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("_Alpha", i);
            yield return null;
        }

        RevertDimmedTilemapRenderer(mainRoom);
    }

    private IEnumerator FadeOutRoomLightingRoutine(MainRoom mainRoom)
    {
        Material material = new Material(GameResources.Instance.variableLitShader);

        LoadTilemapRenderer(mainRoom, material);

        for (float i = 1f; i >= 0.05f; i -= Time.deltaTime / Settings.fadeOutTime)
        {
            material.SetFloat("_Alpha", i);
            yield return null;
        }

        RevertDarkTilemapRenderer(mainRoom);
    }

    private void FadeInEnvironmentLighting()
    {
        Material material = new Material(GameResources.Instance.variableLitShader);

        Environment[] environments = GetComponentsInChildren<Environment>();

        foreach (var environment in environments)
        {
            if (environment.sprite)
                environment.sprite.material = material;
        }

        StartCoroutine(FadeInEnvironmentLightingRoutine(material, environments));
    }

    private void FadeOutEnvironmentLighting()
    {
        Material material = new Material(GameResources.Instance.variableLitShader);

        Environment[] environments = GetComponentsInChildren<Environment>();

        foreach (var environment in environments)
        {
            if (environment.sprite)
                environment.sprite.material = material;
        }

        StartCoroutine(FadeOutEnvironmentLightingRoutine(material, environments));
    }

    private IEnumerator FadeInNPCLighting()
    {
        Material material = new Material(GameResources.Instance.variableLitShader);

        for (int i = 0; i < NPC.Length; i++)
        {
            NPC[i].GetComponent<SpriteRenderer>().material = material;
        }

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("_Alpha", i);
            yield return null;
        }

        for (int i = 0; i < NPC.Length; i++)
        {
            NPC[i].GetComponent<SpriteRenderer>().material = GameResources.Instance.dimmedMaterial; ;
        }
    }

    private IEnumerator FadeOutNPCLighting()
    {
        Material material = new Material(GameResources.Instance.variableLitShader);

        for (int i = 0; i < NPC.Length; i++)
        {
            NPC[i].GetComponent<SpriteRenderer>().material = material;
        }

        for (float i = 1f; i >= 0.05f; i -= Time.deltaTime / Settings.fadeOutTime)
        {
            material.SetFloat("_Alpha", i);
            yield return null;
        }

        for (int i = 0; i < NPC.Length; i++)
        {
            NPC[i].GetComponent<SpriteRenderer>().material = GameResources.Instance.darkMaterial;
        }
    }

    private IEnumerator FadeInEnvironmentLightingRoutine(Material material, Environment[] environments)
    {
        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("_Alpha", i);
            yield return null;
        }

        foreach (var environment in environments)
        {
            if (environment.sprite)
                environment.sprite.material = GameResources.Instance.dimmedMaterial;
        }
    }

    private IEnumerator FadeOutEnvironmentLightingRoutine(Material material, Environment[] environments) 
    {
        for (float i = 1f; i >= 0.05f; i -= Time.deltaTime / Settings.fadeOutTime)
        {
            material.SetFloat("_Alpha", i);
            yield return null;
        }

        foreach (var environment in environments)
        {
            if (environment.sprite)
                environment.sprite.material = GameResources.Instance.darkMaterial;
        }
    }

    private void LoadTilemapRenderer(MainRoom mainRoom, Material material)
    {
        mainRoom.groundTilemap.GetComponent<TilemapRenderer>().material = material;
        mainRoom.shadowTilemap.GetComponent<TilemapRenderer>().material = material;
        mainRoom.decorationTilemap.GetComponent<TilemapRenderer>().material = material;
        mainRoom.frontTilemap.GetComponent<TilemapRenderer>().material = material;
    }

    private void RevertDimmedTilemapRenderer(MainRoom mainRoom)
    {
        mainRoom.groundTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.dimmedMaterial;
        mainRoom.shadowTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.dimmedMaterial;
        mainRoom.decorationTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.dimmedMaterial;
        mainRoom.frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.dimmedMaterial;
    }

    private void RevertDarkTilemapRenderer(MainRoom mainRoom)
    {
        mainRoom.groundTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.darkMaterial;
        mainRoom.shadowTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.darkMaterial;
        mainRoom.decorationTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.darkMaterial;
        mainRoom.frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.darkMaterial;
    }
}
