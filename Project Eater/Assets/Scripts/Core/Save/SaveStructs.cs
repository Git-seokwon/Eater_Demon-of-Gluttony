using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public struct GameManagerSave
{
    public int BaalFlesh;
    public int Baal_GreatShard;
    public List<int> savedMonsterDNA;
    public List<int> savedLatentSkills;
}

[Serializable]
public struct StatUpgradeDataSave
{
    public int[] levels;
    public int reroll;
}

[Serializable]
public struct PlayerEntitySave
{
    public LatentSkillData[] datas;
}

[SerializeField]
public struct NPCProgress
{
    public int Baal;
    public int Sigma;
    public int Charles;
}

[Serializable]
public struct PlayerAcquirableSkills
{
    public bool[] tier_01_skills;
}

[Serializable]
public struct StageClearData
{
    public int[] stageClearDatas;
}

[Serializable]
public struct TutorialData
{
    public bool isTutorialClear;
}

[Serializable]
public struct GraphicData
{
    public int resolutionIndex;
    public float brightness;
    public bool bFullScreen;
    public bool bVSyncIsOn;
}

[Serializable]
public struct EventData
{
    public bool[] entranceTrigger;
    public int eventIndex;
}

[Serializable]
public struct SkillInventoryTutorialData
{
    public int isTutorialClear;
}