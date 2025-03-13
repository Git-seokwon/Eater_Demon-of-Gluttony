using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundEffectManager : MonoBehaviour
{
    private static SoundEffectManager instance;
    public static SoundEffectManager Instance => instance;

    public int soundsVolume = 8;
    public int uiSoundsVolume = 8;

    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject); // 중복된 SaveSystem 제거
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("soundsVolume"))
        {
            soundsVolume = PlayerPrefs.GetInt("soundsVolume");
        }
        if (PlayerPrefs.HasKey("uiSoundsVolume"))
        {
            uiSoundsVolume = PlayerPrefs.GetInt("uiSoundsVolume");
        }

        SetSoundVolume(soundsVolume);
        SetUISoundVolume(uiSoundsVolume);
    }

    private void OnDisable()
    {
        // Save volume settings in playerprefs
        PlayerPrefs.SetInt("soundsVolume", soundsVolume);
        PlayerPrefs.SetInt("uiSoundsVolume", uiSoundsVolume);
    }

    // Play the sound & UI sound effect
    public void PlaySoundEffect(SoundEffectSO soundEffect)
    {
        // Play sound using a sound gameobject and component from the object pool
        // -> SFX도 하나의 오브젝트로 만들어 Pool Manager에 저장해 두었다가 필요시 Pool에서 꺼내서 사용한다. 
        // -> 위치나 회전 값은 중요하지 않다. (소리이기 때문)
        SoundEffect sound = PoolManager.Instance.ReuseGameObject(soundEffect.soundPrefab, Vector3.zero, Quaternion.identity, false)
            .GetComponent<SoundEffect>();

        // 음향 효과 설정 
        sound.SetSound(soundEffect);
        // sound.OnEable 함수 실행 : 재생
        sound.gameObject.SetActive(true);
        // 음향 효과 지속 시간(소리 재생 시간)은 코루틴으로 구현 
        // soundEffect.soundEffectClip.length : The length of the audio clip in seconds. (Read Only)
        StartCoroutine(DisableSound(sound, soundEffect.soundEffectClip.length));
    }

    // Disable sound effect object after it has played thus returning it to the object pool
    private IEnumerator DisableSound(SoundEffect sound, float length)
    {
        yield return new WaitForSeconds(length);
        sound.gameObject.SetActive(false);
    }

    // Increase sounds volume
    public void IncreaseSoundsVolume()
    {
        int maxSoundsVolume = 20;

        if (soundsVolume >= maxSoundsVolume) return;

        soundsVolume += 1;

        SetSoundVolume(soundsVolume);
    }

    // Decrease sounds volume 
    public void DecreaseSoundsVolume()
    {
        if (soundsVolume == 0) return;

        soundsVolume -= 1;

        SetSoundVolume(soundsVolume);
    }

    // Increase UI sounds volume
    public void IncreaseUISoundsVolume()
    {
        int maxSoundsVolume = 20;

        if (uiSoundsVolume >= maxSoundsVolume) return;

        uiSoundsVolume += 1;

        SetUISoundVolume(uiSoundsVolume);
    }

    // Decrease UI sounds volume 
    public void DecreaseUISoundsVolume()
    {
        if (uiSoundsVolume == 0) return;

        uiSoundsVolume -= 1;

        SetUISoundVolume(uiSoundsVolume);
    }

    // Set sounds volume
    public void SetSoundVolume(int soundsVolume)
    {
        float muteDecibels = -80f;

        if (soundsVolume == 0)
        {
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", HelperUtilities.LinearToDecibels(soundsVolume));
        }
    }

    // Set UI sounds volume
    public void SetUISoundVolume(int uiSoundsVolume)
    {
        float muteDecibels = -80f;

        if (soundsVolume == 0)
        {
            GameResources.Instance.uiSoundsMasterMixerGroup.audioMixer.SetFloat("uiSoundsVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.uiSoundsMasterMixerGroup.audioMixer.SetFloat("uiSoundsVolume", HelperUtilities.LinearToDecibels(uiSoundsVolume));
        }
    }
}
