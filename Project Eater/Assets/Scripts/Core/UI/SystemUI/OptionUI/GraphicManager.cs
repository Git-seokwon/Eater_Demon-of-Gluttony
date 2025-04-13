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
    private int defaultResolutionIndex = 0;
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

    public List<(int width, int height)> ultraWideResolutions;

    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        Cursor.lockState = CursorLockMode.Confined;

        bDefaultFullScreen = true;
        bDefaultVSyncIsOn = QualitySettings.vSyncCount > 0;

        ultraWideResolutions = new List<(int width, int height)>();
        ultraWideResolutions.Add((960, 480));
        ultraWideResolutions.Add((2160, 1080));
        ultraWideResolutions.Add((2280, 1080));
        ultraWideResolutions.Add((2340, 1080));
        ultraWideResolutions.Add((2520, 1080));
        ultraWideResolutions.Add((2560, 1080));
        ultraWideResolutions.Add((2880, 1440));
        ultraWideResolutions.Add((2960, 1440));
        ultraWideResolutions.Add((3040, 1440));
        ultraWideResolutions.Add((3120, 1440));
        ultraWideResolutions.Add((3200, 1440));
        ultraWideResolutions.Add((3440, 1440));
        ultraWideResolutions.Add((3820, 1600));
        ultraWideResolutions.Add((10240, 4320));

        resolutionIndex = defaultResolutionIndex;
        brightness = defaultBrightness;
        bFullScreen = bDefaultFullScreen;
        bVSyncIsOn = bDefaultVSyncIsOn;
    }
}