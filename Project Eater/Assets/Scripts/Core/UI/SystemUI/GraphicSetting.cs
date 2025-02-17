using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphicSetting : MonoBehaviour
{
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
    private TMP_Text currentResolutionText;
    [SerializeField]
    private TMP_Text brightnessText;

    private int currentResolutionIndex = 0;
    private List<(int width, int height)> resolutions;
    private bool bWindowed = false;

    void Awake()
    {
        resolutions = new List<(int width, int height)>();

        resolutionLeftBtn.onClick.AddListener(OnClickResolutionLeft);
        resolutionRightBtn.onClick.AddListener(OnClickResolutionRight);
        brightnessSlider.onValueChanged.AddListener(OnChangeBrightness);

        resolutions.Add((1920, 1080));
        resolutions.Add((2560, 1440));
        resolutions.Add((3840, 2160));
    }

    private void OnClickResolutionLeft()
    {
        if (0 < currentResolutionIndex)
        {
            int width = resolutions[--currentResolutionIndex].width;
            int height = resolutions[currentResolutionIndex].height;
            currentResolutionText.text = $"{width}x{height}";
            Screen.SetResolution(width, height, !bWindowed);
        }
    }

    private void OnClickResolutionRight()
    {
        if (currentResolutionIndex < resolutions.Count - 1)
        {
            int width = resolutions[++currentResolutionIndex].width;
            int height = resolutions[currentResolutionIndex].height;
            currentResolutionText.text = $"{width}x{height}";
            Screen.SetResolution(width, height, !bWindowed);
        }
    }

    private void OnChangeBrightness(float value)
    {
        int brightness = (int)value;
        brightnessText.text = brightness.ToString();
        // change brightness
    }

    private void ConfirmChanges()
    {
        // change previous values to changed values
    }

    private void CancelChanges()
    {
        // use previous values to return everything
    }
}
