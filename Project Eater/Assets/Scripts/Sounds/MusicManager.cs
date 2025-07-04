using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    public static MusicManager Instance => instance;

    private AudioSource musicAudioSource = null;
    private AudioClip currentAudioClip = null;
    private Coroutine fadeOutMusicCoroutine;
    private Coroutine fadeInMusicCoroutine;
    public int musicVolume = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Load components
            musicAudioSource = GetComponent<AudioSource>();

            // Start with music off (게임 시작 시 음악을 끈다)
            // AudioMixerSnapshot.TransitionTo : 특정 스냅샷(AudioMixerSnapshot)으로 부드럽게 전환하는 기능
            // -> timeToReach 시간이 지난 동안 해당 snapshot(musicOffSnapshot) 상태가 된다. 
            GameResources.Instance.musicOffSnapshot.TransitionTo(0f);
        }
        else
            Destroy(gameObject); // 중복된 SaveSystem 제거
    }

    private void Start()
    {
        // Check if volume levels have been saved in playerprefs - if so retrieve and set them
        // retrieve : 검색하다
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicVolume = PlayerPrefs.GetInt("musicVolume");
        }

        SetMusicVolume(musicVolume);
    }

    private void OnDisable()
    {
        // Save volume settings in playerprefs
        PlayerPrefs.SetInt("musicVolume", musicVolume);
    }

    public void PlayMusic(MusicTrackSO musicTrack, float fadeOutTime = Settings.musicFadeOutTime, float fadeInTime = Settings.musicFadeInTime)
    {
        // Play music track
        StartCoroutine(PlayMusicRoutine(musicTrack, fadeOutTime, fadeInTime));
    }

    // Play music for room routine
    private IEnumerator PlayMusicRoutine(MusicTrackSO musicTrack, float fadeOutTime, float fadeInTime)
    {
        // if fade out routine already running then stop it
        if (fadeOutMusicCoroutine != null)
        {
            StopCoroutine(fadeOutMusicCoroutine);
        }

        // id fade in routine already running then stop it
        if (fadeInMusicCoroutine != null)
        {
            StopCoroutine(fadeInMusicCoroutine);
        }

        // If the music track has changed then play new music track 
        // -> 현재 재생되고 있는 music만 체크 
        if (musicTrack.musicClip != currentAudioClip)
        {
            currentAudioClip = musicTrack.musicClip;

            // fadeOutMusicCoroutine : 변경되기 전 music을 fadeOutTime만큼 fadeOut
            yield return fadeOutMusicCoroutine = StartCoroutine(FadeOutMusic(fadeOutTime));

            // fadeInMusicCoroutine : 입력받은 musicTrack의 music을 fadein
            yield return fadeInMusicCoroutine = StartCoroutine(FadeInMusic(musicTrack, fadeInTime));
        }

        yield return null;
    }

    // Fade Out music routine
    private IEnumerator FadeOutMusic(float fadeOutTime)
    {
        // 새로운 음악이 재생될 때 기존 음악의 볼륨을 점진적으로 낮춘다. 
        GameResources.Instance.musicLowSnapshot.TransitionTo(fadeOutTime);

        yield return new WaitForSeconds(fadeOutTime);
    }

    // Fade in music routine 
    private IEnumerator FadeInMusic(MusicTrackSO musicTrack, float fadeInTime)
    {
        // Set clip & play
        musicAudioSource.clip = musicTrack.musicClip;
        musicAudioSource.volume = musicTrack.musicVolume;
        musicAudioSource.Play();

        // 새로운 음악이 시작될 때 볼륨을 점진적으로 키운다. 
        GameResources.Instance.musicOnFullSnapshot.TransitionTo(fadeInTime);

        yield return new WaitForSeconds(fadeInTime);
    }

    public void StopMusic(float fadeOutTime = Settings.musicFadeOutTime)
    {
        StartCoroutine(StopMusicRoutine(fadeOutTime));
    }

    private IEnumerator StopMusicRoutine(float fadeOutTime)
    {
        if (fadeOutMusicCoroutine != null)
        {
            StopCoroutine(fadeOutMusicCoroutine);
        }

        if (fadeInMusicCoroutine != null)
        {
            StopCoroutine(fadeInMusicCoroutine);
        }

        // 음악이 재생 중이라면 페이드 아웃 후 정지
        if (musicAudioSource.isPlaying)
        {
            yield return fadeOutMusicCoroutine = StartCoroutine(FadeOutMusic(fadeOutTime));

            musicAudioSource.Stop();
            musicAudioSource.clip = null;
            currentAudioClip = null;
        }
    }

    // Set music volume
    public void SetMusicVolume(int musicVolume)
    {
        this.musicVolume = Mathf.Clamp(musicVolume, 0, 20);

        float muteDecibels = -80f;

        if (this.musicVolume == 0)
        {
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", HelperUtilities.LinearToDecibels(this.musicVolume));
        }
    }
}
