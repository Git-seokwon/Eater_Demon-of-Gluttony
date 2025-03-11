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
    private Slider masterVolumeSlider;
    [SerializeField]
    private Slider backGroundMusicVolumeSlider;
    [SerializeField]
    private Slider soundEffectsVolumeSlider;

    [SerializeField]
    private TMP_Text masterVolumeText;
    [SerializeField]
    private TMP_Text backGroundMusicVolumeText;
    [SerializeField]
    private TMP_Text soundEffectsVolumeText;

    private Dictionary<string, int> savedVolume;

    void Awake()
    {
        masterVolumeSlider.onValueChanged.AddListener(OnChangeMasterVolume);
        backGroundMusicVolumeSlider.onValueChanged.AddListener(OnChangeBGMVolume);
        soundEffectsVolumeSlider.onValueChanged.AddListener(OnChangeSFXVolume);

        savedVolume = new Dictionary<string, int>();
    }

    public void InitializeBGMVolume(int value)
    {
        savedVolume.Add("BGMVolume", value);
    }

    public void InitializeSFXVolume(int value)
    {
        savedVolume.Add("SFXVolume", value);
    }

    private void OnChangeMasterVolume(float value)
    {
        int soundValue = (int)value;
        masterVolumeText.text = soundValue == 100 ? soundValue.ToString() : soundValue.ToString("00");
    }

    private void OnChangeBGMVolume(float value)
    {
        int soundValue = (int)value;
        masterVolumeText.text = soundValue == 100 ? soundValue.ToString() : soundValue.ToString("00");
        MusicManager.Instance.SetMusicVolume(soundValue);
    }

    private void OnChangeSFXVolume(float value)
    {
        int soundValue = (int)value;
        masterVolumeText.text = soundValue == 100 ? soundValue.ToString() : soundValue.ToString("00");
        SoundEffectManager.Instance.SetSoundVolume(soundValue);
    }

    private void ConfirmChanges()
    {
        // change savedVolume
    }

    private void CancelChanges()
    {
        // use savedVolume to return sound values
    }
}
