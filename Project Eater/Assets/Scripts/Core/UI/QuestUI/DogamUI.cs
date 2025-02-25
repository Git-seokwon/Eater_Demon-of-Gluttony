using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.Runtime.CompilerServices;

public class DogamUI : MonoBehaviour
{
    
    [Header("Properties")]
    [SerializeField] private RectTransform targetRectTransform;
    [SerializeField] private TextMeshProUGUI monsterNameField;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image monsterImageField;
    [SerializeField] private GameObject rewardButton;
    [SerializeField] private VerticalLayoutGroup skillField;
    
    
    private QuestSystem questSystem; // ���� ������ QuestSystem
    private DogamDB dogamDB; // ���� ���� DB
    private IReadOnlyList<Quest> dActiveMonsters;
    private IReadOnlyList<Quest> dCompletedMonsters;
    
    private DogamMonster currentMonster; // Ȥ�� ���� �ϴ� �߰�.

    //250204
    private List<DogamMonster> dogamMonsters = new(); // �ν��Ͻ� �����
    private static DogamUI Instance;

    public IReadOnlyList<DogamMonster> DogamMonsters => dogamMonsters;

    private int currentIndex;
    public int CurrentIndex
    {
        get { return currentIndex; }
        set
        {
            currentIndex = Mathf.Clamp(value, 0, dogamMonsters.Count);
            ChangeDescriptionPage();
            // Debug.Log(currentIndex); // 2���� �۵��ϴ°� Ȯ��~
        }
    }
    private static bool isApplicationQuitting = false;

    private void Awake()
    {
        if (!QuestSystem.isInited)
            QuestSystem.OnInitialized += Initialize;
        else
            Initialize();
        if (gameObject.activeSelf)
            Close();
        Instance = this;
    }

    //250204 ���� -> scriptableObject Ŭ�� ���.
    private void Initialize()
    {
        questSystem = QuestSystem.Instance; // ����Ʈ �ý��� �ν��Ͻ� �ε�
        dogamDB = Resources.Load<DogamDB>("Quest/DogamDB"); // ���� DB �ε�

        //250204
        foreach(var mtemp in dogamDB.DogamMonsters)
        {
            dogamMonsters.Add(Instantiate(mtemp));
        }

        CurrentIndex = 0; // �ε��� �ʱ�ȭ

        dActiveMonsters = questSystem.ActiveAchievements; // Ȱ��ȭ�Ǿ��ִ� ���� ���� ����Ʈ
        dCompletedMonsters = questSystem.CompletedAchievements; // �Ϸ�� ���� ���� ����Ʈ
        questSystem.onAchievementCompleted += DogamQuestSynch;

        foreach (var x in dogamMonsters)
        {
            if (dActiveMonsters.FirstOrDefault(a => a.CodeName == x.CodeName) != null)
            {
                //.Log(x.CodeName + "not registered");
                x.isRegistered = false;
            }

            if (dCompletedMonsters.FirstOrDefault(a => a.CodeName == x.CodeName) != null)
            {
                //Debug.Log(x.CodeName + "registered");
                x.isRegistered = true;
                if (dCompletedMonsters.FirstOrDefault(a => a.CodeName == x.CodeName && a.IsRewardGiven))
                    x.isRewardGiven = true;
            }
        }
    }


    // OnEnable���� ����
    private void OnEnableInit()
    {
        CurrentIndex = 0; // �ε��� �ʱ�ȭ
        dActiveMonsters = questSystem.ActiveAchievements; // Ȱ��ȭ�Ǿ��ִ� ���� ���� ����Ʈ
        dCompletedMonsters = questSystem.CompletedAchievements; // �Ϸ�� ���� ���� ����Ʈ

        VerticalLayoutGroup vlg = gameObject.GetComponentInChildren<VerticalLayoutGroup>();

        // ScrollField�� NameField �߰� �� �ʱ�ȭ
        for (int i = 0; i < dogamMonsters.Count; i++)
        {
            Object obj = Resources.Load("Quest/Quests/Dogam/NameField");
            GameObject instance = (GameObject)Instantiate(obj, vlg.transform);

            // TextField �ʱ�ȭ
            ImageControl img = instance.GetComponentInChildren<ImageControl>();
            img.bg = gameObject.GetComponentInChildren<InfiniteSnapScroll>();
            img.targetRect = targetRectTransform;

            // text �ʱ�ȭ
            TextMeshProUGUI tmp = instance.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = dogamMonsters[i].DisplayName;
        }
}


    // ���� �޼��� �̺�Ʈ ȣ��
    private void DogamQuestSynch(Quest quest)
    {
        DogamMonster ms = dogamMonsters.FirstOrDefault(x => x.CodeName == quest.CodeName);
        if(quest.IsComplete)
            ms.isRegistered = true;
        ChangeDescriptionPage();
    }

    private void ChangeDescriptionPage()
    {
        if (dogamMonsters[CurrentIndex].isRegistered)
        {
            //Debug.Log("called");
            rewardButton.SetActive(true);
            if (dogamMonsters[CurrentIndex].isRewardGiven) //////
                rewardButton.GetComponent<Button>().interactable = false;

            monsterNameField.text = dogamMonsters[CurrentIndex].DisplayName;
            description.text = dogamMonsters[CurrentIndex].Description;
            monsterImageField.sprite = dogamMonsters[CurrentIndex].Image;
            monsterImageField.color = Color.white;

            if(skillField.transform.childCount != 0)
            {
                foreach(Transform a in skillField.transform)
                {
                    Destroy(a.gameObject);
                }
                /*
                for(int i = 0; i < skillField.transform.childCount; i++)
                {
                    Destroy(skillField.transform.GetChild(i).gameObject);
                }
                // 25.1.27 ����
                */
            }

            for(int i=0; i < dogamMonsters[currentIndex].Skills.Count; i++)
            {
                Object obj = Resources.Load("Quest/Quests/Dogam/SkillSprite");
                GameObject instance = (GameObject)Instantiate(obj, skillField.transform);
                instance.GetComponent<Image>().sprite = dogamMonsters[currentIndex].Skills[i].Icon;
            }
        }
        else
        {
            rewardButton.SetActive(false);
            monsterNameField.text = "???";
            description.text = "???";
            monsterImageField.sprite = null;

            if (skillField.transform.childCount != 0)
            {
                for (int i = 0; i < skillField.transform.childCount; i++)
                {
                    Destroy(skillField.transform.GetChild(i).gameObject);
                }
            }
        }
    }
    public void Open()
    {
        gameObject.SetActive(true);
        GameManager.Instance.CinemachineTarget.enabled = false;
    }


    public void Close()
    {
        gameObject.SetActive(false);
        if(GameManager.Instance != null )
            GameManager.Instance.CinemachineTarget.enabled = true;
    }

    public void Reward()
    {
        Quest currentQuest = questSystem.CompletedAchievements.FirstOrDefault(x => x.CodeName == dogamMonsters[currentIndex].CodeName);

        if (!currentQuest.IsRewardGiven)
        {
            //Debug.Log("Rewards received successfully");
            foreach (var x in currentQuest.Rewards)
            {
                x.Give(currentQuest);
                currentQuest.SetIsReward(true);
            }
            dogamMonsters[currentIndex].isRewardGiven = true;
        }
            //Debug.Log("Rewards already received!");
    }

    private void OnEnable()
    {
        OnEnableInit();
        ChangeDescriptionPage();
        if(PlayerController.Instance != null)
            PlayerController.Instance.enabled = false;
    }

    private void OnDisable()
    {
        VerticalLayoutGroup vlg = gameObject.GetComponentInChildren<VerticalLayoutGroup>();
        foreach(Transform child in vlg.transform)
        { 
            Destroy(child.gameObject); 
        }
        if(PlayerController.Instance != null)
            PlayerController.Instance.enabled = true;
    }
}
