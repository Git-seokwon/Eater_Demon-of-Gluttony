using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect_", menuName = "Scriptable Object/Sound/SoundEffect")]
public class SoundEffectSO : ScriptableObject
{
	#region HEADER SOUND EFFECT INFO
	[Space(10)]
	[Header("The name for the sound effect")]
	#endregion

	#region ToolTip
	[Tooltip("The name for the sound effect")]
	#endregion
	public string soundEffectName;

	#region Tooltip
	[Tooltip("The prefab the sound effect")]
	#endregion
	public GameObject soundPrefab;

	#region ToolTip
	[Tooltip("The audio clip for the sound effect")]
	#endregion
	public AudioClip soundEffectClip;

    #region Tooltip
    [Tooltip("The audio is UI Sounds or Sounds")]
    #endregion
    public bool isUISounds;

    #region ToolTip
    [Tooltip("The minimum pitch variation for the sound effect. A random pitch variation will be generated between the minimum and " +
		"maximum values. A random pitch variation makes sound effects more natural.")]
	#endregion
	[Range(0.1f, 1.5f)]
	public float soundEffectPitchRandomVariationMin = 0.1f;

    #region ToolTip
    [Tooltip("The maximum pitch variation for the sound effect. A random pitch variation will be generated between the minimum and " +
        "maximum values. A random pitch variation makes sound effects more natural.")]
    #endregion
    [Range(0.1f, 1.5f)]
    public float soundEffectPitchRandomVariationMax = 1.5f;

	#region Tooltip
	[Tooltip("The sound effect volume")]
	#endregion
	[Range(0.1f, 1f)]
	public float soundEffectVolume = 1f;
}
