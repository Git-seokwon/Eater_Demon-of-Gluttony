using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class SoundEffect : MonoBehaviour
{
    [SerializeField]
    private bool isDontDestroy;

    private AudioSource audioSource;
    private bool isSubscribeEvent = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (isDontDestroy)
        {
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
        }
    }

/*    // 스크립트 실행 순서에 따라 OnEnable이 실행되지 않을 수도 있기 때문에 Start에 한 번 더 써줌
    private void Start()
    {
        if (!isSubscribeEvent && GameManager.Instance != null)
        {
            GameManager.Instance.onPlayerLevelUpHabdler += StopSoundEffect;
            isSubscribeEvent = true;
        }
    }*/

    private void OnEnable()
    {
        if (!isSubscribeEvent && GameManager.Instance != null)
        {
            GameManager.Instance.onPlayerLevelUpHabdler += StopSoundEffect;
            isSubscribeEvent = true;    
        }

        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    private void OnDisable()
    {
        audioSource.Stop();

        if (isSubscribeEvent && GameManager.Instance != null)
        {
            GameManager.Instance.onPlayerLevelUpHabdler -= StopSoundEffect;
            isSubscribeEvent = false;
        }
    }

    // Set the sound effect to play
    public void SetSound(SoundEffectSO soundEffect)
    {
        // pitch : 음의 높낮이 => 음높이 
        // Pitch is a quality that makes a melody go higher or lower. As an example imagine playing an audio clip with pitch set to one.
        // Increasing the pitch as the clip plays will make the clip sound like it is higher.
        // Similarly decreasing the pitch less than one makes the clip sound lower.
        audioSource.pitch = Random.Range(soundEffect.soundEffectPitchRandomVariationMin,
                                         soundEffect.soundEffectPitchRandomVariationMax);

        audioSource.volume = soundEffect.soundEffectVolume;
        audioSource.clip   = soundEffect.soundEffectClip;

        if (soundEffect.isUISounds)
            audioSource.outputAudioMixerGroup = GameResources.Instance.uiSoundsMasterMixerGroup;
        else
            audioSource.outputAudioMixerGroup = GameResources.Instance.soundsMasterMixerGroup;
    }

    private void StopSoundEffect() => audioSource.Stop();
}
