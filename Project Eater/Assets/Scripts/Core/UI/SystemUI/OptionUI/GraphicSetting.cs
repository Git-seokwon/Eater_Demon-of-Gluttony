using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphicSetting : MonoBehaviour
{
    [SerializeField]
    private OptionUIBase optionUIBase;
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

    // previous values
    private int previousResolutionIndex = 0;
    private float previousBrightness = 0f;
    private bool bPreviousFullScreen = true;
    private bool bPreviousVSyncIsOn;

    void Awake()
    {
        resolutionLeftBtn.onClick.AddListener(OnClickResolutionLeft);
        resolutionRightBtn.onClick.AddListener(OnClickResolutionRight);
        brightnessSlider.onValueChanged.AddListener(OnChangeBrightness);

        fullScreenToggle.onValueChanged.AddListener(OnToggleWindowMode);
        vSyncToggle.onValueChanged.AddListener(OnToggleVSync);

        Resolution[] resols = Screen.resolutions;
        resolutions = new List<(int width, int height)>();

        foreach (var res in resols)
            resolutions.Add((res.width, res.height));

        optionUIBase.ConfirmSettingAction += ConfirmChanges;
        optionUIBase.CancelSettingAction += CancelChanges;
        optionUIBase.InitializeSettingAction += OnClickInitializeGraphicValues;

        InitializeGraphicValues();
    }

    private void InitializeGraphicValues()
    {
        previousResolutionIndex = GraphicManager.Instance.resolutionIndex;
        ChangeResolution(GraphicManager.Instance.resolutionIndex);

        previousBrightness = GraphicManager.Instance.brightness;
        OnChangeBrightness(GraphicManager.Instance.brightness);

        fullScreenToggle.isOn = GraphicManager.Instance.bFullScreen;
        bPreviousFullScreen = GraphicManager.Instance.bFullScreen;

        vSyncToggle.isOn = GraphicManager.Instance.bVSyncIsOn;
        bPreviousVSyncIsOn = GraphicManager.Instance.bVSyncIsOn;
    }

    private void OnClickResolutionLeft()
    {
        if (0 < GraphicManager.Instance.resolutionIndex)
            ChangeResolution(--GraphicManager.Instance.resolutionIndex);
    }

    private void OnClickResolutionRight()
    {
        if (GraphicManager.Instance.resolutionIndex < resolutions.Count - 1)
            ChangeResolution(++GraphicManager.Instance.resolutionIndex);
    }

    private void ChangeResolution(int resolutionIndex)
    {
        GraphicManager.Instance.resolutionIndex = resolutionIndex;
        int width = resolutions[resolutionIndex].width;
        int height = resolutions[resolutionIndex].height;
        resolutionText.text = $"{width}x{height}";
        Screen.SetResolution(width, height, GraphicManager.Instance.bFullScreen);
    }

    private void OnToggleWindowMode(bool boolean)
    {
        GraphicManager.Instance.bFullScreen = boolean;
        Screen.fullScreen = GraphicManager.Instance.bFullScreen;
    }

    private void OnToggleVSync(bool boolean)
    {
        GraphicManager.Instance.bVSyncIsOn = boolean;
        QualitySettings.vSyncCount = GraphicManager.Instance.bVSyncIsOn ? 1 : 0;
    }

    private void OnChangeBrightness(float value)
    {
        GraphicManager.Instance.brightness = value;
        int brightness = (int)GraphicManager.Instance.brightness;
        brightnessSlider.value = brightness;
        brightnessText.text = brightness == 100 ? brightness.ToString() : brightness.ToString("00");
        // change brightness
    }

    // for InitializeButton
    private void OnClickInitializeGraphicValues()
    {
        previousResolutionIndex = GraphicManager.Instance.DefaultResolutionIndex;
        ChangeResolution(GraphicManager.Instance.DefaultResolutionIndex);

        previousBrightness = GraphicManager.Instance.DefaultBrightness;
        OnChangeBrightness(GraphicManager.Instance.DefaultBrightness);

        fullScreenToggle.isOn = GraphicManager.Instance.BDefaultFullScreen;
        bPreviousFullScreen = GraphicManager.Instance.BDefaultFullScreen;

        vSyncToggle.isOn = GraphicManager.Instance.BDefaultVSyncIsOn;
        bPreviousVSyncIsOn = GraphicManager.Instance.BDefaultVSyncIsOn;
    }

    private void ConfirmChanges()
    {
        previousResolutionIndex = GraphicManager.Instance.resolutionIndex;
        bPreviousFullScreen = GraphicManager.Instance.bFullScreen;
        bPreviousVSyncIsOn = GraphicManager.Instance.bVSyncIsOn;
        previousBrightness = GraphicManager.Instance.brightness;
    }

    private void CancelChanges()
    {
        GraphicManager.Instance.resolutionIndex = previousResolutionIndex;
        fullScreenToggle.isOn = bPreviousFullScreen;
        vSyncToggle.isOn = bPreviousVSyncIsOn;
        brightnessSlider.value = previousBrightness;

        ChangeResolution(previousResolutionIndex);
        OnToggleWindowMode(fullScreenToggle.isOn);
        OnToggleVSync(vSyncToggle.isOn);
        OnChangeBrightness(previousBrightness);
    }
}
