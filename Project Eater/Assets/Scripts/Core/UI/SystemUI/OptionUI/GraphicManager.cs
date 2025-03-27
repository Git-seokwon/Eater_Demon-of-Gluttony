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

    // default
    private int defaultResolutionIndex = -1;
    private float defaultBrightness = 100f;
    private bool bDefaultFullScreen;
    private bool bDefaultVSyncIsOn;

    public int DefaultResolutionIndex => defaultResolutionIndex;
    public float DefaultBrightness => defaultBrightness;
    public bool BDefaultFullScreen => bDefaultFullScreen;
    public bool BDefaultVSyncIsOn => bDefaultVSyncIsOn;

    public int resolutionIndex;
    public float brightness = 0f;
    public bool bFullScreen;
    public bool bVSyncIsOn;

    private int GCD(int a, int b)
    {
        while (b != 0)
        {
            int c = b;
            b = a % b;
            a = c;
        }
        return a;
    }

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
        foreach (var res in resolution)
        {
            var MAX = GCD(res.width, res.height);
            if ((res.width / MAX == 16) && (res.height / MAX == 9))
                defaultResolutionIndex++;
        }
        
        bDefaultFullScreen = true;
        bDefaultVSyncIsOn = QualitySettings.vSyncCount > 0;

        resolutionIndex = defaultResolutionIndex;
        brightness = defaultBrightness;
        bFullScreen = bDefaultFullScreen;
        bVSyncIsOn = bDefaultVSyncIsOn;
    }
}