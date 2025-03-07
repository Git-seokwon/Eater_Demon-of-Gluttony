using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private PlayerEntity player;
    [SerializeField] private StatUpgrade statUpgrade;
    [Space]
    [SerializeField] private Baal baal;
    [SerializeField] private Sigma sigma;
    [SerializeField] private Charles charles;

    private void Awake()
    {
        if (SaveSystem.Instance == null)
            SaveSystem.OnLoaded += LoadDatas;
        else
            LoadDatas();

        SaveSystem.OnSave += SaveDatas;
    }

    private void LoadDatas()
    {
        LoadGMData();
        LoadPlayerData();
        LoadStatData();
        LoadNPCDatas();

        SaveSystem.OnLoaded -= LoadDatas;
    }
    private void SaveDatas()
    {
        SaveGMData();
        SavePlayerData();
        SaveStatData();
        SaveNPCDatas();

        SaveSystem.OnSave -= SaveDatas;
    }

    #region PlayerEntity
    private void LoadPlayerData()
    {
        PlayerEntitySave temp;
        temp = SaveSystem.Instance.FindSaveData<PlayerEntitySave>("PlayerUpgradeData");
        if (temp.datas != null)
        {
            List<LatentSkillData> listtemp = new();
            foreach (var t in temp.datas)
                listtemp.Add(t);
            player.savedLatentSkills = listtemp;
        }
    }
    private void SavePlayerData()
    {
        PlayerEntitySave temp;
        temp.datas = player.savedLatentSkills.ToArray();
        SaveSystem.Instance.AddSaves("PlayerUpgradeData", temp);
    }
    #endregion

    #region GameManager
    private void LoadGMData() // Load를 위한 임시메서드
    {
        if (GameManager.Instance != null)
        {
            GameManagerSave temp = SaveSystem.Instance.FindSaveData<GameManagerSave>("GameManager");
            GameManager.Instance.BaalFlesh = temp.BaalFlesh;
            GameManager.Instance.Baal_GreatShard = temp.BaalFlesh;

            if (temp.savedLatentSkills != null)
            {
                GameManager.Instance.savedLatentSkills = temp.savedLatentSkills;
                //Debug.Log("savedLatentSkills Loaded");
            }
            else
                GameManager.Instance.savedLatentSkills = new();

            if(temp.savedMonsterDNA != null)
            {
                //Debug.Log("savedMonsterDNA Loaded");
                GameManager.Instance.savedMonsterDNA = temp.savedMonsterDNA;
            }
            else
                GameManager.Instance.savedMonsterDNA = new();
        }
    }
    private void SaveGMData()
    {
        GameManagerSave temp = new();
        temp.BaalFlesh = GameManager.Instance.BaalFlesh;
        temp.Baal_GreatShard = GameManager.Instance.Baal_GreatShard;
        temp.savedMonsterDNA = GameManager.Instance.savedMonsterDNA;
        temp.savedLatentSkills = GameManager.Instance.savedLatentSkills;
        SaveSystem.Instance.AddSaves("GameManager", temp);
    }
    #endregion

    #region StatUpgrade
    private void LoadStatData()
    {
        StatUpgradeDataSave temp;
        temp = SaveSystem.Instance.FindSaveData<StatUpgradeDataSave>("StatUpgradeData");
        if (temp.levels != null)
            statUpgrade.currentStatUpgradeLevel = temp.levels;
    }

    private void SaveStatData()
    {
        StatUpgradeDataSave temp = new(); //250306 added
        temp.levels = statUpgrade.currentStatUpgradeLevel;
        SaveSystem.Instance.AddSaves("StatUpgradeData", temp);
    }
    #endregion

    #region NPCProgress
    private void LoadNPCDatas()
    {
        NPCProgress temp = new();
        temp = SaveSystem.Instance.FindSaveData<NPCProgress>("NPCProgress");
        baal.Affinity = temp.Baal;
        charles.Affinity = temp.Charles;
        sigma.Affinity = temp.Sigma;
    }

    private void SaveNPCDatas()
    {
        NPCProgress temp = new();
        temp.Baal = baal.Affinity;
        temp.Sigma = sigma.Affinity;
        temp.Charles = charles.Affinity;
        SaveSystem.Instance.AddSaves("NPCProgress", temp);
    }
    #endregion
}
