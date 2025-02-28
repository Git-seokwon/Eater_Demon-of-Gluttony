using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }

            return instance;
        }
    }

    #region Header PLAYER
    [Space(10)]
    [Header("PLAYER")]
    #endregion

    #region HEADER MATERIAL
    [Space(10)]
    [Header("MATERIALS")]
    #endregion
    #region Tooltip
    [Tooltip("Dimmed Material")]
    #endregion
    public Material dimmedMaterial;

    #region Tooltip
    [Tooltip("Sprite-Lit-Default Materials")]
    #endregion
    public Material litMaterial;

    #region Tooltip
    [Tooltip("Dark Material")]
    #endregion
    public Material darkMaterial;

    #region Tooltip
    [Tooltip("Populate with the Variable Lit Shader")]
    #endregion
    public Shader variableLitShader;

    #region Tooltip
    [Tooltip("Skill Border Image - 포식 / 고유")]
    #endregion
    public Sprite[] borderImages = new Sprite[2];
    public Sprite[] borderImagesByGrade = new Sprite[3];

    public Sprite GetBorderImageByGrade(SkillGrade grade)
    {
        int index = (int)grade - 1;
        return index >= 0 && index < borderImagesByGrade.Length ? borderImagesByGrade[index] : null;
    }

    #region Tooltip
    [Tooltip("additionalGoodsChoice Image")]
    #endregion
    public Sprite additionalGoodsChoiceImage;

    #region SOUNDS
    [Space(10)]
    [Header("SOUNDS")]
    #endregion
    public AudioMixerGroup soundsMasterMixerGroup;
    // 각 스킬 SFX는 스킬 오브젝트에 동봉되어 있음
    public SoundEffectSO hitSound;
    public SoundEffectSO DashSound;
    public SoundEffectSO getMeatSound;
    public SoundEffectSO getDNASound;
    public SoundEffectSO getGreatShadrSound;
    public SoundEffectSO statUpgradeSound;
    public SoundEffectSO latentSkillUpgradeSound;

    #region SOUNDS
    [Space(10)]
    [Header("MUSIC")]
    #endregion
    // 각 Stage Music은 스테이지 오브젝트에 동봉되어 있음 
    public AudioMixerGroup musicMasterMixerGroup;
    public AudioMixerSnapshot musicOnFullSnapshot;
    public AudioMixerSnapshot musicLowSnapshot;
    public AudioMixerSnapshot musicOffSnapshot;
    public MusicTrackSO mainMenuMusic;

    #region IMPACT
    [Space(10)]
    [Header("IMPACT")]
    public GameObject hitImpact;
    public GameObject critHitImpact;
    #endregion
}
