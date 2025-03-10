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
            Destroy(gameObject); // �ߺ��� SaveSystem ����
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
        // -> SFX�� �ϳ��� ������Ʈ�� ����� Pool Manager�� ������ �ξ��ٰ� �ʿ�� Pool���� ������ ����Ѵ�. 
        // -> ��ġ�� ȸ�� ���� �߿����� �ʴ�. (�Ҹ��̱� ����)
        SoundEffect sound = PoolManager.Instance.ReuseGameObject(soundEffect.soundPrefab, Vector3.zero, Quaternion.identity, false)
            .GetComponent<SoundEffect>();

        // ���� ȿ�� ���� 
        sound.SetSound(soundEffect);
        // sound.OnEable �Լ� ���� : ���
        sound.gameObject.SetActive(true);
        // ���� ȿ�� ���� �ð�(�Ҹ� ��� �ð�)�� �ڷ�ƾ���� ���� 
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
