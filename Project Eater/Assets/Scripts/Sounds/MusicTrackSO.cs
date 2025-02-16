using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicTrack_", menuName = "Scriptable Object/Sound/MusicTrack")]
public class MusicTrackSO : ScriptableObject
{
    public string musicName;

    public AudioClip musicClip;

    [Range(0f, 1f)]
    public float musicVolume = 1f;
}
