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
    private Toggle bloodEffectToggle;

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
    private bool bCurrentBloodEffectIsOn;

    // previous values
    private int previousResolutionIndex = 0;
    private float previousBrightness = 0f;
    private bool bPreviousFullScreen = true;
    private bool bPreviousVSyncIsOn;
    private bool bPreviousBloodEffectIsOn;

    private static GraphicSetting instance;
    public static GraphicSetting Instance => instance;

    void Awake()
    {
        if (Instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);

        resolutions = new List<(int width, int height)>();
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
        bCurrentVSyncIsOn = QualitySettings.vSyncCount > 0;
        bPreviousVSyncIsOn = QualitySettings.vSyncCount > 0;
        bCurrentBloodEffectIsOn = true;
        bPreviousBloodEffectIsOn = true;

        fullScreenToggle.isOn = Screen.fullScreen;
        bCurrentFullScreen = Screen.fullScreen;
        bPreviousFullScreen = Screen.fullScreen;

        resolutionLeftBtn.onClick.AddListener(OnClickResolutionLeft);
        resolutionRightBtn.onClick.AddListener(OnClickResolutionRight);
        brightnessSlider.onValueChanged.AddListener(OnChangeBrightness);

        fullScreenToggle.onValueChanged.AddListener(OnToggleWindowMode);
        vSyncToggle.onValueChanged.AddListener(OnToggleVSync);
        bloodEffectToggle.onValueChanged.AddListener(OnToggleBloodEffect);

        resolutions.Add((1920, 1080));
        resolutions.Add((2560, 1440));
        resolutions.Add((3840, 2160));

        optionUIBase.ConfirmSettingAction += ConfirmChanges;
        optionUIBase.CancelSettingAction += CancelChanges;
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

    private void OnToggleWindowMode(bool boolean)
    {
        bCurrentFullScreen = boolean;
        Screen.fullScreen = bCurrentFullScreen;
    }

    private void OnToggleVSync(bool boolean)
    {
        bCurrentVSyncIsOn = boolean;
        QualitySettings.vSyncCount = bCurrentVSyncIsOn ? 1 : 0;
    }

    private void OnChangeBrightness(float value)
    {
        currentBrightness = value;
        int brightness = (int)currentBrightness;
        brightnessText.text = brightness == 100 ? brightness.ToString() : brightness.ToString("00");
        // change brightness
    }

    private void OnToggleBloodEffect(bool boolean)
    {
        bCurrentBloodEffectIsOn = boolean;
    }

    private void ConfirmChanges()
    {
        previousResolutionIndex = currentResolutionIndex;
        bPreviousFullScreen = bCurrentFullScreen;
        bPreviousVSyncIsOn = bCurrentVSyncIsOn;
        previousBrightness = currentBrightness;
        bPreviousBloodEffectIsOn = bCurrentBloodEffectIsOn;

        Debug.Log(bPreviousVSyncIsOn);
        Debug.Log(vSyncToggle.isOn);
        Debug.Log(bPreviousBloodEffectIsOn);
        Debug.Log(bloodEffectToggle.isOn);
    }

    private void CancelChanges()
    {
        currentResolutionIndex = previousResolutionIndex;
        fullScreenToggle.isOn = bPreviousFullScreen;
        vSyncToggle.isOn = bPreviousVSyncIsOn;
        brightnessSlider.value = previousBrightness;
        bloodEffectToggle.isOn = bPreviousBloodEffectIsOn;

        Debug.Log(bPreviousVSyncIsOn);
        Debug.Log(vSyncToggle.isOn);
        Debug.Log(bPreviousBloodEffectIsOn);
        Debug.Log(bloodEffectToggle.isOn);

        ChangeResolution(previousResolutionIndex);
        OnToggleWindowMode(fullScreenToggle.isOn);
        OnToggleVSync(vSyncToggle.isOn);
        OnChangeBrightness(previousBrightness);
        OnToggleBloodEffect(bloodEffectToggle.isOn);
    }
}
