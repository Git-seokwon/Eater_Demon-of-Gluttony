using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="DogamMonster_", menuName ="Quest/DogamMonster")]
public class DogamMonster : ScriptableObject
{
    [Header("Text")]
    [SerializeField] private string codeName;
    [SerializeField] private string displayName;
    [SerializeField, TextArea] private string description;

    [Header("Images")]
    [SerializeField] private Sprite image;
    [SerializeField] private List<Skill> skills;

    [Header("Option")]
    [SerializeField] private bool isBoss;

    public bool isRegistered; // 발견여부
    public bool isRewardGiven; // 보상 수령여부


    #region Property
    public string CodeName => codeName;
    public string DisplayName => displayName;
    public string Description => description;
    public bool IsBoss => isBoss;
    public Sprite Image => image;
    public IReadOnlyList<Skill> Skills => skills;
    #endregion
}
