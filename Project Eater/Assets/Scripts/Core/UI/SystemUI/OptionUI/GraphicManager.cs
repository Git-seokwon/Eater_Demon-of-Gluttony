using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphicManager : MonoBehaviour
{
    private static GraphicManager instance;
    public static GraphicManager Instance => instance;

    public int resolutionIndex = 0;
    public float brightness = 0f;
    public bool bFullScreen;
    public bool bVSyncIsOn;
    public bool bBloodEffectIsOn;

    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        Debug.Log("GraphicManager Awake");

        bFullScreen = Screen.fullScreen;
        bVSyncIsOn = QualitySettings.vSyncCount > 0;
        bBloodEffectIsOn = true;
    }
}