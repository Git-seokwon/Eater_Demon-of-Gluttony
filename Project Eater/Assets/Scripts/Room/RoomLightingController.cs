using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(MainRoom))]
public class RoomLightingController : MonoBehaviour
{
    private MainRoom mainRoom;

    private void Awake()
    {
        mainRoom = GetComponent<MainRoom>();
    }

    private void OnEnable()
    {
        StaticEventHandler.OnRoomEntered += RoomEnter;
        StaticEventHandler.OnRoomExited  += RoomExit;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomEntered -= RoomEnter;
        StaticEventHandler.OnRoomExited  -= RoomExit;
    }

    private void RoomEnter(RoomChangedEventArgs roomChangedEventArgs)
    {
        if (!mainRoom.isLit)
        {
            FadeInRoomLighting();
            mainRoom.ActivateEnvironmentGameObject();
            mainRoom.isLit = true;
        }
    }

    private void RoomExit(RoomChangedEventArgs roomChangedEventArgs)
    {
        if (mainRoom.isLit)
        {
            FadeOutRoomLighting();
            mainRoom.DeActivateEnvironmentGameObject();
            mainRoom.isLit = false;
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
            material.SetFloat("Alpha_Slider", i);
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
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        RevertDarkTilemapRenderer(mainRoom);
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
        mainRoom.groundTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        mainRoom.shadowTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        mainRoom.decorationTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        mainRoom.frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
    }

    private void RevertDarkTilemapRenderer(MainRoom mainRoom)
    {
        mainRoom.groundTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.darkMaterial;
        mainRoom.shadowTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.darkMaterial;
        mainRoom.decorationTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.darkMaterial;
        mainRoom.frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.darkMaterial;
    }
}
