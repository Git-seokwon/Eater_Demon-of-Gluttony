using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphicSetting : MonoBehaviour
{
    [SerializeField]
    private LobbyOptionUI lobbyOptionUI;
    [SerializeField]
    private GameObject resolution;
    [SerializeField]
    private GameObject brightness;

    [SerializeField]
    private Button resolutionLeftBtn;
    [SerializeField]
    private Button resolutionRightBtn;
    [SerializeField]
    private Slider brightnessSlider;
    [SerializeField]
    private Toggle fullScreenToggle;
    [SerializeField]
    private Toggle vSyncToggle;

    [SerializeField]
    private TMP_Text resolutionText;
    [SerializeField]
    private TMP_Text brightnessText;

    private List<(int width, int height)> resolutions;

    // current values
    private int currentResolutionIndex = 0;
    private float currentBrightness = 0f;
    private bool bCurrentFullScreen = true;
    private bool bCurrentVSyncIsOn;

    // previous values
    private int previousResolutionIndex = 0;
    private float previousBrightness = 0f;
    private bool bPreviousFullScreen = true;
    private bool bPreviousVSyncIsOn;

    void Awake()
    {
        resolutions = new List<(int width, int height)>();
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
        bCurrentVSyncIsOn = QualitySettings.vSyncCount > 0;
        bPreviousVSyncIsOn = QualitySettings.vSyncCount > 0;

        fullScreenToggle.isOn = Screen.fullScreen;
        bCurrentFullScreen = Screen.fullScreen;
        bPreviousFullScreen = Screen.fullScreen;

        resolutionLeftBtn.onClick.AddListener(OnClickResolutionLeft);
        resolutionRightBtn.onClick.AddListener(OnClickResolutionRight);
        brightnessSlider.onValueChanged.AddListener(OnChangeBrightness);

        resolutions.Add((1920, 1080));
        resolutions.Add((2560, 1440));
        resolutions.Add((3840, 2160));

        lobbyOptionUI.ConfirmSettingAction += ConfirmChanges;
        lobbyOptionUI.CancelSettingAction += CancelChanges;
    }

    private void OnClickResolutionLeft()
    {
        if (0 < currentResolutionIndex)
            ChangeResolution(--currentResolutionIndex);
    }

    private void OnClickResolutionRight()
    {
        if (currentResolutionIndex < resolutions.Count - 1)
            ChangeResolution(++currentResolutionIndex);
    }

    private void ChangeResolution(int resolutionIndex)
    {
        int width = resolutions[resolutionIndex].width;
        int height = resolutions[resolutionIndex].height;
        resolutionText.text = $"{width}x{height}";
        Screen.SetResolution(width, height, bCurrentFullScreen);
    }

    private void OnToggleWindowMode()
    {
        bCurrentFullScreen = fullScreenToggle.isOn;
        Screen.fullScreen = bCurrentFullScreen;
    }

    private void OnToggleVSync()
    {
        bCurrentVSyncIsOn = vSyncToggle.isOn;
        QualitySettings.vSyncCount = bCurrentVSyncIsOn ? 1 : 0;
    }

    private void OnChangeBrightness(float value)
    {
        currentBrightness = value;
        int brightness = (int)currentBrightness;
        brightnessText.text = brightness == 100 ? brightness.ToString() : brightness.ToString("00");
        // change brightness
    }

    private void ConfirmChanges()
    {
        previousResolutionIndex = currentResolutionIndex;
        bPreviousFullScreen = bCurrentFullScreen;
        bPreviousVSyncIsOn = bCurrentVSyncIsOn;
        previousBrightness = currentBrightness;
    }

    private void CancelChanges()
    {
        currentResolutionIndex = previousResolutionIndex;
        fullScreenToggle.isOn = bPreviousFullScreen;
        vSyncToggle.isOn = bPreviousVSyncIsOn;
        brightnessSlider.value = previousBrightness;

        ChangeResolution(previousResolutionIndex);
        OnToggleWindowMode();
        OnToggleVSync();
        OnChangeBrightness(previousBrightness);
    }
}
