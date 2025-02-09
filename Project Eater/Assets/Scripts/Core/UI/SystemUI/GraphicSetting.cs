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
    private List<string> resolutions;

    void Awake()
    {
        resolutions = new List<string>();

        resolutionLeftBtn.onClick.AddListener(OnClickResolutionLeft);
        resolutionRightBtn.onClick.AddListener(OnClickResolutionRight);
        brightnessSlider.onValueChanged.AddListener(OnChangeBrightness);

        resolutions.Add("1920x1080");
        resolutions.Add("2560x1440");
        resolutions.Add("3840x2160");
    }

    private void OnClickResolutionLeft()
    {
        if (0 < currentResolutionIndex)
        {
            currentResolutionText.text = resolutions[--currentResolutionIndex];
        }

        // change game resolution
    }

    private void OnClickResolutionRight()
    {
        if (currentResolutionIndex < resolutions.Count - 1)
        {
            currentResolutionText.text = resolutions[++currentResolutionIndex];
        }

        // change game resolution
    }

    private void OnChangeBrightness(float value)
    {
        int brightness = (int)value;
        brightnessText.text = brightness.ToString();
    }
}
