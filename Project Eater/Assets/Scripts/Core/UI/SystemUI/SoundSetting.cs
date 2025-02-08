using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundSetting : MonoBehaviour
{
    [SerializeField]
    private GameObject master;
    [SerializeField]
    private GameObject backGround;
    [SerializeField]
    private GameObject soundEffects;

    [SerializeField]
    private Slider masterSoundSlider;
    [SerializeField]
    private Slider backGroundSoundSlider;
    [SerializeField]
    private Slider soundEffectsSoundSlider;

    [SerializeField]
    private TMP_Text masterSoundText;
    [SerializeField]
    private TMP_Text backGroundSoundText;
    [SerializeField]
    private TMP_Text soundEffectsSoundText;

    void Awake()
    {
        masterSoundSlider.onValueChanged.AddListener(OnChangeMasterSound);
        backGroundSoundSlider.onValueChanged.AddListener(OnChangeBGSound);
        soundEffectsSoundSlider.onValueChanged.AddListener(OnChangeSFXSound);
    }

    private void OnChangeMasterSound(float value)
    {
        int soundValue = (int)value;
        masterSoundText.text = soundValue.ToString();
    }

    private void OnChangeBGSound(float value)
    {
        int soundValue = (int)value;
        backGroundSoundText.text = soundValue.ToString();
    }

    private void OnChangeSFXSound(float value)
    {
        int soundValue = (int)value;
        soundEffectsSoundText.text = soundValue.ToString();
    }
}
