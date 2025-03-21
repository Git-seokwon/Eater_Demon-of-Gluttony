using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundSetting : MonoBehaviour
{
    [SerializeField]
    private OptionUIBase optionUIBase;

    [SerializeField]
    private Button backGroundMusicVolumeLeftBtn;
    [SerializeField]
    private Button backGroundMusicVolumeRightBtn;
    [SerializeField]
    private Button gameSoundEffetcsVolumeLeftBtn;
    [SerializeField]
    private Button gameSoundEffetcsVolumeRightBtn;
    [SerializeField]
    private Button uISoundEffectsVolumeLeftBtn;
    [SerializeField]
    private Button uISoundEffectsVolumeRightBtn;

    [SerializeField]
    private TMP_Text backGroundMusicVolumeText;
    [SerializeField]
    private TMP_Text gameSoundEffectsVolumeText;
    [SerializeField]
    private TMP_Text uISoundEffectsVolumeText;

    private Dictionary<string, float> previousVolume;
    private Dictionary<string, float> currentVolume;

    private float defaultBGMVolume = 20f;
    private float defaultGameSFXVolume = 8f;
    private float defaultUISFXVolume = 8f;

    void Awake()
    {
        backGroundMusicVolumeLeftBtn.onClick.AddListener(() => OnClickDecreaseVolume("BGMVolume"));
        backGroundMusicVolumeRightBtn.onClick.AddListener(() => OnClickIncreaseVolume("BGMVolume"));
        gameSoundEffetcsVolumeLeftBtn.onClick.AddListener(() => OnClickDecreaseVolume("GameSFXVolume"));
        gameSoundEffetcsVolumeRightBtn.onClick.AddListener(() => OnClickIncreaseVolume("GameSFXVolume"));
        uISoundEffectsVolumeLeftBtn.onClick.AddListener(() => OnClickDecreaseVolume("UISFXVolume"));
        uISoundEffectsVolumeRightBtn.onClick.AddListener(() => OnClickIncreaseVolume("UISFXVolume"));

        previousVolume = new Dictionary<string, float>();
        currentVolume = new Dictionary<string, float>();

        optionUIBase.ConfirmSettingAction += ConfirmChanges;
        optionUIBase.CancelSettingAction += CancelChanges;
        optionUIBase.InitializeSettingAction += OnClickInitializeVolumes;

        InitializeVolumes();
    }

    private void InitializeVolumes()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            float musicVolume = PlayerPrefs.GetInt("musicVolume");
            previousVolume.Add("BGMVolume", musicVolume);
            currentVolume.Add("BGMVolume", musicVolume);
        }
        if (PlayerPrefs.HasKey("soundsVolume"))
        {
            float gameSoundVolume = PlayerPrefs.GetInt("soundsVolume");
            previousVolume.Add("GameSFXVolume", gameSoundVolume);
            currentVolume.Add("GameSFXVolume", gameSoundVolume);
        }
        if (PlayerPrefs.HasKey("uiSoundsVolume"))
        {
            float uiSoundVolume = PlayerPrefs.GetInt("uiSoundsVolume");
            previousVolume.Add("UISFXVolume", uiSoundVolume);
            currentVolume.Add("UISFXVolume", uiSoundVolume);
        }

        int soundValue = (int)currentVolume["BGMVolume"] * 5;
        backGroundMusicVolumeText.text = soundValue == 100 ? soundValue.ToString() : soundValue.ToString("00");
        soundValue = (int)currentVolume["GameSFXVolume"] * 5;
        gameSoundEffectsVolumeText.text = soundValue == 100 ? soundValue.ToString() : soundValue.ToString("00");
        soundValue = (int)currentVolume["UISFXVolume"] * 5;
        uISoundEffectsVolumeText.text = soundValue == 100 ? soundValue.ToString() : soundValue.ToString("00");
    }

    private void OnClickDecreaseVolume(string soundName)
    {
        if (2 <= currentVolume[soundName])
        {
            currentVolume[soundName] -= 2;
            ChangeVolume(soundName, currentVolume[soundName]);
        }
    }

    private void OnClickIncreaseVolume(string soundName)
    {
        if (currentVolume[soundName] <= 18)
        {
            currentVolume[soundName] += 2;
            ChangeVolume(soundName, currentVolume[soundName]);
        }
    }

    private void ChangeVolume(string soundName, float value)
    {
        int soundValue = (int)value;
        switch(soundName)
        {
            case "BGMVolume":
                MusicManager.Instance.SetMusicVolume(soundValue);
                soundValue *= 5;
                backGroundMusicVolumeText.text = soundValue == 100 ? soundValue.ToString() : soundValue.ToString("00");
                break;

            case "GameSFXVolume":
                SoundEffectManager.Instance.SetSoundVolume(soundValue);
                soundValue *= 5;
                gameSoundEffectsVolumeText.text = soundValue == 100 ? soundValue.ToString() : soundValue.ToString("00");
                break;

            case "UISFXVolume":
                SoundEffectManager.Instance.SetSoundVolume(soundValue);
                soundValue *= 5;
                uISoundEffectsVolumeText.text = soundValue == 100 ? soundValue.ToString() : soundValue.ToString("00");
                break;
        }
    }    

    // for InitializeButton
    private void OnClickInitializeVolumes()
    {
        currentVolume["BGMVolume"] = defaultBGMVolume;
        currentVolume["GameSFXVolume"] = defaultGameSFXVolume;
        currentVolume["UISFXVolume"] = defaultUISFXVolume;

        ChangeVolume("BGMVolume", currentVolume["BGMVolume"]);
        ChangeVolume("GameSFXVolume", currentVolume["GameSFXVolume"]);
        ChangeVolume("UISFXVolume", currentVolume["UISFXVolume"]);
    }

    private void ConfirmChanges()
    {
        previousVolume["BGMVolume"] = currentVolume["BGMVolume"];
        previousVolume["GameSFXVolume"] = currentVolume["GameSFXVolume"];
        previousVolume["UISFXVolume"] = currentVolume["UISFXVolume"];
    }

    private void CancelChanges()
    {
        currentVolume["BGMVolume"] = previousVolume["BGMVolume"];
        currentVolume["GameSFXVolume"] = previousVolume["GameSFXVolume"];
        currentVolume["UISFXVolume"] = previousVolume["UISFXVolume"];

        ChangeVolume("BGMVolume", currentVolume["BGMVolume"]);
        ChangeVolume("GameSFXVolume", currentVolume["GameSFXVolume"]);
        ChangeVolume("UISFXVolume", currentVolume["UISFXVolume"]);
    }
}
