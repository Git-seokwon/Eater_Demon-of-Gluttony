using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundEffectManager : MonoBehaviour
{
    [SerializeField]
    private GameObject lobbyEnterSoundGO;

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
            Destroy(gameObject); // �ߺ��� SaveSystem ����
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

    public void PlayLobbyEnterSound()
    {
        SoundEffect sound = lobbyEnterSoundGO.GetComponent<SoundEffect>();

        // ���� ȿ�� ���� 
        sound.SetSound(GameResources.Instance.uilobbyEnterSound);
        // sound.OnEable �Լ� ���� : ���
        sound.gameObject.SetActive(true);

        // ���� ȿ�� ���� �ð�(�Ҹ� ��� �ð�)�� �ڷ�ƾ���� ���� 
        // soundEffect.soundEffectClip.length : The length of the audio clip in seconds. (Read Only)
        StartCoroutine(DisableSound(sound, GameResources.Instance.uilobbyEnterSound.soundEffectClip.length));
    }

    // Disable sound effect object after it has played thus returning it to the object pool
    private IEnumerator DisableSound(SoundEffect sound, float length)
    {
        yield return new WaitForSeconds(length);

        // Debug.Log("DisableSound ����");
        sound.gameObject.SetActive(false);
    }

    // Set sounds volume
    public void SetSoundVolume(int soundsVolume)
    {
        this.soundsVolume = Mathf.Clamp(soundsVolume, 0, 20);

        float muteDecibels = -80f;

        if (this.soundsVolume == 0)
        {
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", HelperUtilities.LinearToDecibels(this.soundsVolume));
        }
    }

    // Set UI sounds volume
    public void SetUISoundVolume(int uiSoundsVolume)
    {
        this.uiSoundsVolume = Mathf.Clamp(uiSoundsVolume, 0, 20);

        float muteDecibels = -80f;

        if (this.uiSoundsVolume == 0)
        {
            GameResources.Instance.uiSoundsMasterMixerGroup.audioMixer.SetFloat("uiSoundsVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.uiSoundsMasterMixerGroup.audioMixer.SetFloat("uiSoundsVolume", HelperUtilities.LinearToDecibels(this.uiSoundsVolume));
        }
    }
}
