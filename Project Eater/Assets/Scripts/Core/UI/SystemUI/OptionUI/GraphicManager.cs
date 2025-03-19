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

    private List<(int width, int height)> resolutions;

    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        int i_width = Screen.width;
        int i_height = Screen.height;

        resolutions = new List<(int width, int height)>();

        resolutions.Add((1920, 1080));
        resolutions.Add((2560, 1440));
        resolutions.Add((3840, 2160));

        for (int i = 0; i < resolutions.Count; ++i)
        {
            if (i_width == resolutions[i].width && i_height == resolutions[i].height)
            {
                Debug.Log(i);
                resolutionIndex = i;
                break;
            }
        }
        bFullScreen = true;
        bVSyncIsOn = QualitySettings.vSyncCount > 0;
    }
}