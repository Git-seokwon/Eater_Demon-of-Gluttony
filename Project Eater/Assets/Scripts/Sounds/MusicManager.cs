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

            // Start with music off (���� ���� �� ������ ����)
            // AudioMixerSnapshot.TransitionTo : Ư�� ������(AudioMixerSnapshot)���� �ε巴�� ��ȯ�ϴ� ���
            // -> timeToReach �ð��� ���� ���� �ش� snapshot(musicOffSnapshot) ���°� �ȴ�. 
            GameResources.Instance.musicOffSnapshot.TransitionTo(0f);
        }
        else
            Destroy(gameObject); // �ߺ��� SaveSystem ����
    }

    private void Start()
    {
        // Check if volume levels have been saved in playerprefs - if so retrieve and set them
        // retrieve : �˻��ϴ�
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
        // -> ���� ����ǰ� �ִ� music�� üũ 
        if (musicTrack.musicClip != currentAudioClip)
        {
            currentAudioClip = musicTrack.musicClip;

            // fadeOutMusicCoroutine : ����Ǳ� �� music�� fadeOutTime��ŭ fadeOut
            yield return fadeOutMusicCoroutine = StartCoroutine(FadeOutMusic(fadeOutTime));

            // fadeInMusicCoroutine : �Է¹��� musicTrack�� music�� fadein
            yield return fadeInMusicCoroutine = StartCoroutine(FadeInMusic(musicTrack, fadeInTime));
        }

        yield return null;
    }

    // Fade Out music routine
    private IEnumerator FadeOutMusic(float fadeOutTime)
    {
        // ���ο� ������ ����� �� ���� ������ ������ ���������� �����. 
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

        // ���ο� ������ ���۵� �� ������ ���������� Ű���. 
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

        // ������ ��� ���̶�� ���̵� �ƿ� �� ����
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
