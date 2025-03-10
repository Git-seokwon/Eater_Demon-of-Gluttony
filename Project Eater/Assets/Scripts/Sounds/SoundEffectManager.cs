using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundEffectManager : MonoBehaviour
{
    private static SoundEffectManager instance;
    public static SoundEffectManager Instance => instance;

    public int soundVolume = 8;

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
            soundVolume = PlayerPrefs.GetInt("soundVolume");
        }

        SetSoundVolume(soundVolume);
    }

    private void OnDisable()
    {
        // Save volume settings in playerprefs
        PlayerPrefs.SetInt("soundsVolume", soundVolume);
    }

    // Play the sound effect
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

        if (soundVolume >= maxSoundsVolume) return;

        soundVolume += 1;

        SetSoundVolume(soundVolume);
    }

    // Decrease sounds volume 
    public void DecreaseSoundsVolume()
    {
        if (soundVolume == 0) return;

        soundVolume -= 1;

        SetSoundVolume(soundVolume);
    }

    // Set sounds volume
    public void SetSoundVolume(int soundVolume)
    {
        float muteDecibels = -80f;

        if (soundVolume == 0)
        {
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", HelperUtilities.LinearToDecibels(soundVolume));
        }
    }
}
