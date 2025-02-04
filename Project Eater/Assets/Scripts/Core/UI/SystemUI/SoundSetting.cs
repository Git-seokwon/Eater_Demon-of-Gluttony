using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundSetting : MonoBehaviour
{
    [SerializeField]
    private GameObject Master;
    [SerializeField]
    private GameObject BackGround;
    [SerializeField]
    private GameObject SoundEffects;

    [SerializeField]
    private Slider MasterSoundSlider;
    [SerializeField]
    private Slider BackGroundSoundSlider;
    [SerializeField]
    private Slider SoundEffectsSoundSlider;

    [SerializeField]
    private TMP_Text MasterSoundText;
    [SerializeField]
    private TMP_Text BackGroundSoundText;
    [SerializeField]
    private TMP_Text SoundEffectsSoundText;

    void Awake()
    {
        MasterSoundSlider.onValueChanged.AddListener(OnChangeMasterSound);
        BackGroundSoundSlider.onValueChanged.AddListener(OnChangeBGSound);
        SoundEffectsSoundSlider.onValueChanged.AddListener(OnChangeSFXSound);
    }

    private void OnChangeMasterSound(float value)
    {
        int soundValue = (int)value;
        MasterSoundText.text = soundValue.ToString();
    }

    private void OnChangeBGSound(float value)
    {
        int soundValue = (int)value;
        BackGroundSoundText.text = soundValue.ToString();
    }

    private void OnChangeSFXSound(float value)
    {
        int soundValue = (int)value;
        SoundEffectsSoundText.text = soundValue.ToString();
    }
}
