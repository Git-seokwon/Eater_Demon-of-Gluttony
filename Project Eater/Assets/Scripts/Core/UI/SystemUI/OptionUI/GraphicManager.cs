using System.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography;

public class GraphicManager : MonoBehaviour
{
    private static GraphicManager instance;
    public static GraphicManager Instance => instance;

    public int resolutionIndex = 0;
    public float brightness = 0f;
    public bool bFullScreen;
    public bool bVSyncIsOn;

    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        Resolution[] resolution = Screen.resolutions;
        resolutionIndex = resolution.Length - 1;
        
        bFullScreen = true;
        bVSyncIsOn = QualitySettings.vSyncCount > 0;
    }
}