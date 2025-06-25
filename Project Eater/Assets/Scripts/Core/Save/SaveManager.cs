using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SaveManager : SingletonMonobehaviour<SaveManager>
{
    [SerializeField] private LobbyUI lobby;

    private PlayerEntity player;
    private StatUpgrade statUpgrade;
    private Baal baal;
    private Sigma sigma;
    private Charles charles;
    private SkillSystem playerSkillSystem;
    private GraphicManager graphicManager;

    protected override void Awake()
    {
        base.Awake();

        if (SaveSystem.Instance == null)
            SaveSystem.OnLoaded += LoadDatasInLobby;
        else
            LoadDatasInLobby();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene" || scene.buildIndex == 3)
        {
            player = FindObjectOfType<PlayerEntity>();
            statUpgrade = FindObjectOfType<StatUpgrade>();
            baal = FindObjectOfType<Baal>();
            sigma = FindObjectOfType<Sigma>();
            charles = FindObjectOfType<Charles>();
            playerSkillSystem = player.SkillSystem;

            PlayerSkillSetUp();
            LoadDatas();

            SaveSystem.OnSave += SaveDatas;
        }
    }

    private void PlayerSkillSetUp()
    {
        playerSkillSystem.InitSkillSlots();
        player.SetUpLatentSkill();
    }

    #region Load
    private void LoadDatasInLobby()
    {
        TutorialData temp = new();
        temp = SaveSystem.Instance.FindSaveData<TutorialData>("TutorialData");

        lobby.isClearTutorial = temp.isTutorialClear;

        LoadGraphicManager();
        SaveSystem.OnLoaded -= LoadDatasInLobby;
        SaveSystem.OnSave += SaveGraphicManager;
    }
    private void LoadDatas()
    {
        LoadGMData();
        LoadPlayerData();
        LoadStatData();
        LoadNPCDatas();
        LoadPlayerAcquirableSkills();
        LoadStageClearInfo();
        LoadEventTrigger();
        LoadSkillInventoryTutorial();
    }
    #endregion

    private void SaveDatas()
    {
        SaveGMData();
        SavePlayerData();
        SaveStatData();
        SaveNPCDatas();
        SavePlayerAcquirableSkills();
        SaveGraphicManager();
        SaveStageClearInfo();
        SaveEventTrigger();
        SaveSkillInventoryTutorial();

        SaveSystem.OnSave -= SaveDatas;
    }

    #region PlayerEntity
    private void LoadPlayerData()
    {
        PlayerEntitySave temp;
        temp = SaveSystem.Instance.FindSaveData<PlayerEntitySave>("PlayerUpgradeData");

        if (temp.datas != null)
        {
            foreach (var data in temp.datas)
                player.LoadLatentSkill(data.index, data.level);

            player.SetLatentSkills();
        }
    }
    private void SavePlayerData()
    {
        var latentSkills = player.OwnLatentSkills;
        LatentSkillData[] savedLatentSkillDatas = new LatentSkillData[latentSkills.Count];

        for (int i = 0; i < savedLatentSkillDatas.Length; i++)
        {
            savedLatentSkillDatas[i] = new LatentSkillData  // 명시적 초기화
            {
                index = latentSkills[i].Index,
                level = latentSkills[i].Level
            };
        }

        PlayerEntitySave temp = new PlayerEntitySave // 명시적 초기화
        {
            datas = savedLatentSkillDatas
        };

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
            GameManager.Instance.Baal_GreatShard = temp.Baal_GreatShard;

            if (temp.savedLatentSkills != null)
            {
                GameManager.Instance.hasLatentSkill = new HashSet<int>(temp.savedLatentSkills);
                //Debug.Log("savedLatentSkills Loaded");
            }
            else
                GameManager.Instance.hasLatentSkill = new();

            if(temp.savedMonsterDNA != null)
            {
                //Debug.Log("savedMonsterDNA Loaded");
                GameManager.Instance.hasMonsterDNA = new HashSet<int>(temp.savedMonsterDNA);
            }
            else
                GameManager.Instance.hasMonsterDNA = new();
        }
    }
    private void SaveGMData()
    {
        GameManagerSave temp = new();
        temp.BaalFlesh       = GameManager.Instance.BaalFlesh;
        temp.Baal_GreatShard = GameManager.Instance.Baal_GreatShard;

        temp.savedMonsterDNA   = new List<int>(GameManager.Instance.hasMonsterDNA);
        temp.savedLatentSkills = new List<int>(GameManager.Instance.hasLatentSkill);
        SaveSystem.Instance.AddSaves("GameManager", temp);
    }
    #endregion

    #region StatUpgrade
    private void LoadStatData()
    {
        StatUpgradeDataSave temp;
        temp = SaveSystem.Instance.FindSaveData<StatUpgradeDataSave>("StatUpgradeData");
        if (temp.levels != null)
        {
            statUpgrade.currentStatUpgradeLevel = temp.levels;
            GameManager.Instance.player.Stats.ReRollStat.DefaultValue = temp.reroll;
        }
    }

    private void SaveStatData()
    {
        StatUpgradeDataSave temp = new(); //250306 added
        temp.levels = statUpgrade.currentStatUpgradeLevel;
        temp.reroll = (int)GameManager.Instance.player.Stats.ReRollStat.DefaultValue;
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

    #region PlayerSkill
    private void LoadPlayerAcquirableSkills()
    {
        PlayerAcquirableSkills temp = new();
        temp = SaveSystem.Instance.FindSaveData<PlayerAcquirableSkills>("PlayerAcquirableSkills");

        if (temp.tier_01_skills == null)
            return;

        var skills = playerSkillSystem.SkillSlot.Where(pair => pair.Key.Item1 == 0)
                                                .OrderBy(pair => pair.Key.Item2)
                                                .Select(pair => pair.Value).ToArray();
        int index = 0;
        foreach (var skill in skills)
        {
            skill.IsDevoured = temp.tier_01_skills[index++];
        }
    }

    private void SavePlayerAcquirableSkills()
    {
        PlayerAcquirableSkills temp = new();
        // playerSkillSystem의 SkillSlot(모든 스킬 정보가 들어있는 Dictionary)에서
        // Tier 0인 Value(SkillCombinationSlotNode)에 접근, Key.Item2(Index 값)을 기준으로 정렬
        // 이후, 해당 값에서 IsDevoured(Bool) 데이터를 array로 가져오기
        temp.tier_01_skills = playerSkillSystem.SkillSlot.Where(pair => pair.Key.Item1 == 0)
                                                         .OrderBy(pair => pair.Key.Item2)
                                                         .Select(pair => pair.Value.IsDevoured).ToArray();

        SaveSystem.Instance.AddSaves("PlayerAcquirableSkills", temp);
    }
    #endregion

    #region Stage Clear
    private void LoadStageClearInfo()
    {
        StageClearData temp;
        temp = SaveSystem.Instance.FindSaveData<StageClearData>("StageClearData");

        int i = 0;
        if (temp.stageClearDatas != null)
        {
            foreach (var stage in StageManager.Instance.Stages)
            {
                stage.ClearCount = temp.stageClearDatas[i];
            }
        }
    }

    private void SaveStageClearInfo()
    {
        var stages = StageManager.Instance.Stages;

        int[] stageClearDatas = new int[stages.Count];

        for (int i = 0; i < stageClearDatas.Length; i++)
        {
            stageClearDatas[i] = stages[i].ClearCount;
        }

        StageClearData temp = new StageClearData()
        {
            stageClearDatas = stageClearDatas
        };

        SaveSystem.Instance.AddSaves("StageClearData", temp);
    }
    #endregion

    #region Tutorial
    public void SaveLatentSkill() // index : 0, level = 1 
    {
        LatentSkillData savedLatentSkillData = new LatentSkillData()
        {
            index = 0,
            level = 1
        };

        PlayerEntitySave temp = new PlayerEntitySave // 명시적 초기화
        {
            datas = new LatentSkillData[] { savedLatentSkillData } // 배열로 변경
        };

        SaveSystem.Instance.AddSaves("PlayerUpgradeData", temp);
    }

    public void SaveShardShot()
    {
        PlayerAcquirableSkills temp = new()
        {
            tier_01_skills = new bool[10] // 배열 길이를 10으로 초기화하고 기본값 false
        };

        temp.tier_01_skills[7] = true; // 7번째 Index 값을 true로 수정
                                       // 7 : 탄환 발사 

        SaveSystem.Instance.AddSaves("PlayerAcquirableSkills", temp);
    }

    public void SaveCoachellaDNA()
    {
        GameManagerSave temp = new()
        {
            savedMonsterDNA = new List<int>(),
            savedLatentSkills = new List<int>()
        };

        temp.savedMonsterDNA.Add(2); // Coachella DNA ID : 2

        SaveSystem.Instance.AddSaves("GameManager", temp);
    }

    public void SaveTutorialClear()
    {
        TutorialData temp = new();

        temp.isTutorialClear = true;

        SaveSystem.Instance.AddSaves("TutorialData", temp);
    }
    #endregion

    #region GraphicManager 
    private void LoadGraphicManager()
    {
        GraphicData temp = new();
        temp = SaveSystem.Instance.FindSaveData<GraphicData>("Graphics");
        
        if (SaveSystem.Instance.firstStart)
            return;

        GraphicManager.Instance.resolutionIndex = temp.resolutionIndex;
        GraphicManager.Instance.brightness = temp.brightness;
        GraphicManager.Instance.bFullScreen = temp.bFullScreen;
        GraphicManager.Instance.bVSyncIsOn = temp.bVSyncIsOn;
    }

    private void SaveGraphicManager()
    {
        GraphicData temp = new();

        temp.resolutionIndex = GraphicManager.Instance.resolutionIndex;
        temp.brightness = GraphicManager.Instance.brightness;
        temp.bFullScreen = GraphicManager.Instance.bFullScreen;
        temp.bVSyncIsOn = GraphicManager.Instance.bVSyncIsOn;

        SaveSystem.Instance.AddSaves("Graphics", temp);
        SaveSystem.OnSave -= SaveGraphicManager;
    }
    #endregion

    #region EventTrigger
    private void LoadEventTrigger()
    {
        EventData temp = new();

        temp = SaveSystem.Instance.FindSaveData<EventData>("EventTriggerData");
        if (temp.entranceTrigger == null)
            temp.entranceTrigger = new bool[2];

        GameManager.Instance.StageEntranceTrigger.isTriggers = temp.entranceTrigger;
        GameManager.Instance.StageEntranceTrigger.eventIndex = temp.eventIndex;
    }

    private void SaveEventTrigger()
    {
        bool[] stageEntranceTrigger = GameManager.Instance.StageEntranceTrigger.isTriggers;
        int eventIndex = GameManager.Instance.StageEntranceTrigger.eventIndex;

        EventData temp = new EventData()
        {
            entranceTrigger = stageEntranceTrigger,
            eventIndex = eventIndex,
        };

        SaveSystem.Instance.AddSaves("EventTriggerData", temp);
    }
    #endregion

    #region Skill Iventory UI Tutorial
    private void LoadSkillInventoryTutorial()
    {
        SkillInventoryTutorialData temp = new();

        temp = SaveSystem.Instance.FindSaveData<SkillInventoryTutorialData>("SkillInventoryTutorialData");

        GameManager.Instance.SkillInventoryTutorial.isTutorialClear = temp.isTutorialClear;
    }

    private void SaveSkillInventoryTutorial()
    {
        int isTutorialClear = GameManager.Instance.SkillInventoryTutorial.isTutorialClear;

        SkillInventoryTutorialData temp = new SkillInventoryTutorialData()
        {
            isTutorialClear = isTutorialClear
        };

        SaveSystem.Instance.AddSaves("SkillInventoryTutorialData", temp);
    }
    #endregion
}
