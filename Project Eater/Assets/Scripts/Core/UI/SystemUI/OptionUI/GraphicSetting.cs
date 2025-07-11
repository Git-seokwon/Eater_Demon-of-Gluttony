using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using TMPro;
using Cinemachine.PostFX;

public class GraphicSetting : MonoBehaviour
{
    [SerializeField]
    private OptionUIBase optionUIBase;

    [SerializeField]
    private TMP_Dropdown resolutionDropdown;
    [SerializeField]
    private Slider brightnessSlider;
    [SerializeField]
    private CinemachineVolumeSettings cinemachineVS;
    [SerializeField]
    private Toggle fullScreenToggle;
    [SerializeField]
    private Toggle vSyncToggle;

    [SerializeField]
    private TMP_Text brightnessText;

    private List<(int width, int height)> resolutions;
    private List<string> resolutionOptions;
    
    private ColorAdjustments colorAdjustmentsSetting;

    // pre-modification values
    private int previousResolutionIndex = 0;
    private float previousBrightness = 0f;
    private bool bPreviousFullScreen = true;
    private bool bPreviousVSyncIsOn;

    void Awake()
    {
        resolutionDropdown.onValueChanged.AddListener(OnChangeResolutionOptions);
        brightnessSlider.onValueChanged.AddListener(OnChangeBrightness);
        fullScreenToggle.onValueChanged.AddListener(OnToggleWindowMode);
        vSyncToggle.onValueChanged.AddListener(OnToggleVSync);

        cinemachineVS?.m_Profile.TryGet(out colorAdjustmentsSetting);

        resolutions = new List<(int width, int height)>();
        resolutionDropdown.ClearOptions();
        resolutionOptions = new List<string>();

        foreach (var res in Screen.resolutions)
        {
            if (res.refreshRate == Screen.currentResolution.refreshRate)
            {
                if (GraphicManager.Instance.ultraWideResolutions.Contains((res.width, res.height)) == false)
                {
                    resolutions.Add((res.width, res.height));
                    resolutionOptions.Add($"{res.width}x{res.height}");
                }
            }
        }
        resolutions.Reverse();
        resolutionOptions.Reverse();
        resolutionDropdown.AddOptions(resolutionOptions);

        optionUIBase.ConfirmSettingAction += ConfirmChanges;
        optionUIBase.CancelSettingAction += CancelChanges;
        optionUIBase.InitializeSettingAction += OnClickInitializeGraphicValues;

        LoadSavedGraphicValues();
    }

    // load save file values
    private void LoadSavedGraphicValues()
    {
        previousResolutionIndex = GraphicManager.Instance.resolutionIndex;
        ChangeResolution(GraphicManager.Instance.resolutionIndex);

        previousBrightness = GraphicManager.Instance.brightness;
        brightnessSlider.value = GraphicManager.Instance.brightness;
        OnChangeBrightness(GraphicManager.Instance.brightness);

        bPreviousFullScreen = GraphicManager.Instance.bFullScreen;
        fullScreenToggle.isOn = GraphicManager.Instance.bFullScreen;
        OnToggleWindowMode(GraphicManager.Instance.bFullScreen);

        bPreviousVSyncIsOn = GraphicManager.Instance.bVSyncIsOn;
        vSyncToggle.isOn = GraphicManager.Instance.bVSyncIsOn;
        OnToggleVSync(GraphicManager.Instance.bVSyncIsOn);
    }

    private void OnChangeResolutionOptions(int value)
    {
        ChangeResolution(value);
    }

    private void ChangeResolution(int resolutionIndex)
    {
        if (!GraphicManager.Instance.bFullScreen)
            GraphicManager.Instance.resolutionIndex = resolutionIndex;
        resolutionDropdown.value = resolutionIndex;
        int width = resolutions[resolutionIndex].width;
        int height = resolutions[resolutionIndex].height;
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
        brightnessText.text = brightness == 100 ? brightness.ToString() : brightness.ToString("00");

        if (cinemachineVS != null)
            colorAdjustmentsSetting.postExposure.value = (GraphicManager.Instance.brightness - 100) / 25;
        
    }

    // for InitializeButton
    private void OnClickInitializeGraphicValues()
    {
        ChangeResolution(GraphicManager.Instance.DefaultResolutionIndex);

        GraphicManager.Instance.brightness = GraphicManager.Instance.DefaultBrightness;
        brightnessSlider.value = GraphicManager.Instance.brightness;
        OnChangeBrightness(GraphicManager.Instance.brightness);

        fullScreenToggle.isOn = GraphicManager.Instance.BDefaultFullScreen;
        OnToggleWindowMode(GraphicManager.Instance.BDefaultFullScreen);

        vSyncToggle.isOn = GraphicManager.Instance.BDefaultVSyncIsOn;
        GraphicManager.Instance.bVSyncIsOn = GraphicManager.Instance.BDefaultVSyncIsOn;
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
