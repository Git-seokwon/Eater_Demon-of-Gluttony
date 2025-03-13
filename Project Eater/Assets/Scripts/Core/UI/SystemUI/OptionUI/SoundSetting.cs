using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundSetting : MonoBehaviour
{
    [SerializeField]
    private LobbyOptionUI lobbyOptionUI;
    [SerializeField]
    private GameObject backGround;

    [SerializeField]
    private Slider backGroundMusicVolumeSlider;
    [SerializeField]
    private Slider gameSoundEffectsVolumeSlider;
    [SerializeField]
    private Slider uISoundEffectsVolumeSlider;

    [SerializeField]
    private TMP_Text backGroundMusicVolumeText;
    [SerializeField]
    private TMP_Text gameSoundEffectsVolumeText;
    [SerializeField]
    private TMP_Text uISoundEffectsVolumeText;

    private Dictionary<string, float> previousVolume;

    void Awake()
    {
        backGroundMusicVolumeSlider.onValueChanged.AddListener(OnChangeBGMVolume);
        gameSoundEffectsVolumeSlider.onValueChanged.AddListener(OnChangeGameSFXVolume);
        uISoundEffectsVolumeSlider.onValueChanged.AddListener(OnChangeUISFXVolume);

        previousVolume = new Dictionary<string, float>();

        lobbyOptionUI.ConfirmSettingAction += ConfirmChanges;
        lobbyOptionUI.CancelSettingAction += CancelChanges;
    }

    public void InitializeBGMVolume(int value)
    {
        previousVolume.Add("BGMVolume", value);
    }

    public void InitializeGameSFXVolume(int value)
    {
        previousVolume.Add("SFXVolume", value);
    }

    public void InitializeUISFXVolume(int value)
    {
        previousVolume.Add("SFXVolume", value);
    }

    private void OnClickIncreaseBGMVolume()
    {
        MusicManager.Instance.IncreaseMusicVolume();
        backGroundMusicVolumeText.text = soundValue == 100 ? soundValue.ToString() : soundValue.ToString("00");
    }

    private void OnChangeBGMVolume(float value)
    {
        int soundValue = (int)value;
        backGroundMusicVolumeText.text = soundValue == 100 ? soundValue.ToString() : soundValue.ToString("00");
        MusicManager.Instance.SetMusicVolume(soundValue);
    }

    private void OnChangeGameSFXVolume(float value)
    {
        int soundValue = (int)value;
        gameSoundEffectsVolumeText.text = soundValue == 100 ? soundValue.ToString() : soundValue.ToString("00");
        SoundEffectManager.Instance.SetSoundVolume(soundValue);
    }

    private void OnChangeUISFXVolume(float value)
    {
        int soundValue = (int)value;
        uISoundEffectsVolumeText.text = soundValue == 100 ? soundValue.ToString() : soundValue.ToString("00");
        SoundEffectManager.Instance.SetSoundVolume(soundValue);
    }

    private void ConfirmChanges()
    {
        Debug.Log("change savedVolume");
    }

    private void CancelChanges()
    {
        Debug.Log("use savedVolume to return sound values");
    }
}
