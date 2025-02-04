using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphicSetting : MonoBehaviour
{
    [SerializeField]
    private GameObject Resolution;
    [SerializeField]
    private GameObject Brightness;

    [SerializeField]
    private Button ResolutionLeftBtn;
    [SerializeField]
    private Button ResolutionRightBtn;
    [SerializeField]
    private Slider BrightnessSlider;

    [SerializeField]
    private List<TMP_Text> ResolutionTexts;
    [SerializeField]
    private TMP_Text BrightnessText;

    void Awake()
    {
        ResolutionLeftBtn.onClick.AddListener(OnClickResolutionLeft);
        ResolutionRightBtn.onClick.AddListener(OnClickResolutionRight);
        BrightnessSlider.onValueChanged.AddListener(OnChangeBrightness);
    }

    private void OnClickResolutionLeft()
    {
        // text change
    }

    private void OnClickResolutionRight()
    {
        // text change
    }

    private void OnChangeBrightness(float value)
    {
        int brightness = (int)value;
        BrightnessText.text = brightness.ToString();
    }
}
