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

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (isDontDestroy)
        {
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    private void OnDisable()
    {
        audioSource.Stop();
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
}
