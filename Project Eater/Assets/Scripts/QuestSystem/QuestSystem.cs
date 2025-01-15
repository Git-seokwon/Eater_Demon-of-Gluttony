using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;


// ����Ʈ �ý��� ��ü��
// ���� �Ŵ��� Ŭ������ QuestSystem.Instance ȣ���� ���ؼ� �̿��� ����.

public class QuestSystem : MonoBehaviour
{
    #region Save Path
    private const string kSaveRootPath = "questSystem";
    private const string kActiveQuestSavePath = "activeQuests";
    private const string kCompletedQuestSavePath = "completedQuests";
    private const string kActiveAchievementsSavePath = "activeAchievements";
    private const string kCompletedAchievementsSavePath = "completedAchievements";
    #endregion

    #region Events
    public delegate void QuestRegisterHandler(Quest newQuest);
    public delegate void QuestCompletedHandler(Quest quest);
    public delegate void QuestCanceledHandler(Quest quest);
    #endregion

    private static QuestSystem instance;
    private static bool isApplicationQuitting;

    public static QuestSystem Instance
    {
        get
        {
            if(!isApplicationQuitting && instance == null)
            {
                instance = FindObjectOfType<QuestSystem>();
                if(instance == null)
                {
                    instance = new GameObject("Quest System").AddComponent<QuestSystem>();
                    DontDestroyOnLoad(instance.gameObject);
                }
            }
            return instance;
        }
    }

    private List<Quest> activeQuests = new List<Quest>();
    private List<Quest> completedQuests = new List<Quest>();

    private List<Quest> activeAchievements = new List<Quest>();
    private List<Quest> completedAchievements = new List<Quest>();

    private QuestDatabase questDatabase;
    private QuestDatabase achievementDatabase;

    public event QuestRegisterHandler onQuestRegistered;
    public event QuestCompletedHandler onQuestCompleted;
    public event QuestCanceledHandler onQuestCanceled;

    public event QuestRegisterHandler onAchievementRegistered;
    public event QuestCompletedHandler onAchievementCompleted;

    public IReadOnlyList<Quest> ActiveQuests => activeQuests;
    public IReadOnlyList<Quest> CompletedQuests => completedQuests;

    public IReadOnlyList<Quest> ActiveAchievements => activeAchievements;
    public IReadOnlyList<Quest> CompletedAchievements => completedAchievements;

    private void Awake()
    {
        questDatabase = Resources.Load<QuestDatabase>("Quest/QuestDatabase");
        achievementDatabase = Resources.Load<QuestDatabase>("Quest/AchievementDatabase");

        if (!Load())
        {
            Debug.Log("�̰� ��� ����ȴٴ°���?");
            foreach (var achivement in achievementDatabase.Quests)
                Register(achivement);
        }
    }

    private void OnApplicationQuit() // Unity���� �����ϴ°�
    {
        isApplicationQuitting = true;
        Save();
    }

    public Quest Register(Quest quest)
    {
        var newQuest = quest.Clone();
        // Instantiate => scriptableObject�� Ư���� ���� Task�� �����ϴ� quest�� �����ϰ� ��.
        // -> task�� ������ �����ϸ� ��� ����Ʈ�� currentSuccess ���� ���ϴ� ������ ����
        // ��� �̸� �ذ��ϱ� ���� Clone �޼��带 ����� ���.

        if (newQuest is QAchievement)
        {
            newQuest.onCompleted += OnAchievementCompleted;
            // ��Ÿ������ ��ħ -> 12.17

            activeAchievements.Add(newQuest);

            newQuest.OnRegister();
            onAchievementRegistered?.Invoke(newQuest);
        }
        else
        {
            newQuest.onCompleted += OnQuestCompleted;
            newQuest.onCanceled += OnQuestCanceled;

            activeQuests.Add(newQuest);

            newQuest.OnRegister();
            onQuestRegistered?.Invoke(newQuest);
        }

        return newQuest;
    }

    /// <summary>
    /// ����Ʈ �ý��ۿ� ��ϵǾ� �ִ� ����Ʈ�� ���� �ൿ�� ����� �����ϴ� �޼��� (�߿�)
    /// </summary>
    /// <param name="category"></param>
    /// <param name="target"></param>
    /// <param name="successCount"></param>
    public void ReceiveReport(string category, object target, int successCount) 
    {
        ReceiveReport(activeQuests, category, target, successCount);
        ReceiveReport(activeAchievements, category, target, successCount);
    }

    /// <summary>
    /// ����Ʈ �ý��ۿ� ��ϵǾ� �ִ� ����Ʈ�� ���� �ൿ�� ����� �����ϴ� �޼��� (�߿�)
    /// </summary>
    /// <param name="category"></param>
    /// <param name="target"></param>
    /// <param name="successCount"></param>
    public void ReceiveReport(QCategory category, QTaskTarget target, int successCount) // �����ε�
        => ReceiveReport(category.CodeName, target.Value, successCount);

    /// <summary>
    /// ����Ʈ �ý��ۿ� ��ϵǾ� �ִ� ����Ʈ�� ���� �ൿ�� ����� �����ϴ� �޼��� (�߿�)
    /// </summary>
    /// <param name="quests"></param>
    /// <param name="category"></param>
    /// <param name="target"></param>
    /// <param name="successCount"></param>
    private void ReceiveReport(List<Quest> quests, string category, object target, int successCount)
    {
        foreach (var quest in quests.ToArray())
        {
            quest.ReceiveReport(category, target, successCount);
            //Debug.Log(quest.CodeName + ":Checked");
        }  
    }

    // ����Ʈ�� ��Ͽ� �ִ��� Ȯ���ϴ� �޼���
    public bool ContainsInActiveQuests(Quest quest) => activeQuests.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInCompletedQuests(Quest quest) => completedQuests.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInActiveAchievements(Quest quest) => activeAchievements.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInCompletedAchievements(Quest quest) => completedAchievements.Any(x => x.CodeName == quest.CodeName);

    /*
    private void Save()
    {
        var root = new Dictionary<string , List<QuestSaveData>>();
        root.Add(kActiveAchievementsSavePath, CreateQuestSaveData(activeAchievements));
        root.Add(kActiveQuestSavePath, CreateQuestSaveData(activeQuests));
        root.Add(kCompletedAchievementsSavePath, CreateQuestSaveData(completedAchievements));
        root.Add(kCompletedQuestSavePath, CreateQuestSaveData(completedQuests));

        string jsonData = JsonUtility.ToJson(root, true);
        string path = Path.Combine(Application.dataPath, "questData.json");

        Debug.Log("QuestSystem - Save - Executed");

        File.WriteAllText(path, jsonData);
    }
    */

    [Serializable]
    class QuestSave
    {
        public List<QuestSaveWrap> quests = new();
        
        public void AddQuest(QuestSaveWrap wrap)
        {
            quests.Add(wrap);
        }
    }

    private void Save()
    {
        List<QuestSaveData> activeQuestSaveDatas = CreateQuestSaveData(activeQuests);
        List<QuestSaveData> completedQuestSaveDatas = CreateQuestSaveData(completedQuests);
        List<QuestSaveData> activeArchievementSaveDatas = CreateQuestSaveData(activeAchievements);
        List<QuestSaveData> completedArchievementSaveDatas = CreateQuestSaveData(completedAchievements);

        QuestSave tessst = new QuestSave();

        tessst.AddQuest(new QuestSaveWrap(kActiveQuestSavePath, activeQuestSaveDatas));
        tessst.AddQuest(new QuestSaveWrap(kCompletedQuestSavePath, completedQuestSaveDatas));
        tessst.AddQuest(new QuestSaveWrap(kActiveAchievementsSavePath, activeArchievementSaveDatas));
        tessst.AddQuest(new QuestSaveWrap(kCompletedAchievementsSavePath, completedArchievementSaveDatas));

        string jsonData = JsonUtility.ToJson(tessst, true);
        string path = Path.Combine(Application.dataPath, "questData.json");

        Debug.Log("QuestSystem - Save - Executed");
        if (jsonData == "{}")
            Debug.Log("save failed");

        File.WriteAllText(path, jsonData);
    }

    private bool Load()
    {
        string path = Path.Combine(Application.dataPath, "questData.json");
        string jsonData = "{}";
        
        try
        {
            jsonData = File.ReadAllText(path);
        }
        catch
        {
            Debug.Log("�̰� ����ؾ��ϳ�");
            File.WriteAllText(path, jsonData);
            return false;
        }

        var root = JsonUtility.FromJson<QuestSave>(jsonData);

        if (root == null)
        {
            Debug.Log("QuestSystem - Load - Failed");
            return false;
        }

        LoadSaveDatas(root.quests.FirstOrDefault(x => x.key == kActiveQuestSavePath)?.value, questDatabase, LoadActiveQuest);
        LoadSaveDatas(root.quests.FirstOrDefault(x => x.key == kCompletedQuestSavePath)?.value, questDatabase, LoadCompletedQuest);
        LoadSaveDatas(root.quests.FirstOrDefault(x => x.key == kActiveAchievementsSavePath)?.value, achievementDatabase, LoadActiveQuest);
        LoadSaveDatas(root.quests.FirstOrDefault(x => x.key == kCompletedAchievementsSavePath)?.value, achievementDatabase, LoadCompletedQuest);

        Debug.Log("QuestSystem - Load - Executed");
        return true;
    }
    
    private List<QuestSaveData> CreateQuestSaveData(IReadOnlyList<Quest> quests)
    {
        var saveDatas = new List<QuestSaveData>();
        foreach(var quest in quests)
        {
            if(quest.IsSavable)
                saveDatas.Add(quest.ToSaveData());
        }
        return saveDatas;
    }

    private void LoadSaveDatas(List<QuestSaveData> quests, QuestDatabase database, System.Action<QuestSaveData, Quest> onSuccess)
    {
        foreach(var data in quests)
        {
            var quest = database.FindQuestBy(data.codeName);
            onSuccess.Invoke(data, quest);
        }
    }

    private void LoadActiveQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = Register(quest);
        newQuest.LoadFrom(saveData);

        if (newQuest is QAchievement)
            activeAchievements.Add(newQuest);
        else
            activeQuests.Add(newQuest);
    }

    private void LoadCompletedQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = quest.Clone();
        newQuest.LoadFrom(saveData);

        if (newQuest is QAchievement)
            completedAchievements.Add(newQuest);
        else
            completedQuests.Add(newQuest);
    }

    #region Callback
    private void OnQuestCompleted(Quest quest)
    {
        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        onQuestCompleted?.Invoke(quest);
    }

    private void OnQuestCanceled(Quest quest)
    {
        activeQuests.Remove(quest);
        onQuestCanceled?.Invoke(quest);

        Destroy(quest, Time.deltaTime);
    }

    private void OnAchievementCompleted(Quest achievement)
    {
        activeAchievements.Remove(achievement);
        completedAchievements.Add(achievement);

        onAchievementCompleted?.Invoke(achievement);
    }
    #endregion
}
