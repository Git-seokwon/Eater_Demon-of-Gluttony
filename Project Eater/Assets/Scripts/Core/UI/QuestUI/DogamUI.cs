using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.Runtime.CompilerServices;

public class DogamUI : SingletonMonobehaviour<DogamUI>
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
    
    private int currentIndex;
    public int CurrentIndex
    {
        get { return currentIndex; }
        set
        {
            currentIndex = Mathf.Clamp(value, 0, dogamDB.DogamMonsters.Count);
            ChangeDescriptionPage();
            // Debug.Log(currentIndex); // 2���� �۵��ϴ°� Ȯ��~
        }
    }
    private static bool isApplicationQuitting = false;

    private void Awake()
    {
        Initialize();
        if(gameObject.activeSelf)
            Close();
    }

    private void Initialize()
    {
        questSystem = QuestSystem.Instance; // �ν��Ͻ�
        dogamDB = Resources.Load<DogamDB>("Quest/DogamDB"); // ���� DB �ε�

        CurrentIndex = 0; // �ε��� �ʱ�ȭ

        dActiveMonsters = questSystem.ActiveAchievements; // Ȱ��ȭ�Ǿ��ִ� ���� ���� ����Ʈ
        dCompletedMonsters = questSystem.CompletedAchievements; // �Ϸ�� ���� ���� ����Ʈ
        questSystem.onAchievementCompleted += DogamQuestSynch;

        foreach (var x in dogamDB.DogamMonsters)
        {
            if (dActiveMonsters.FirstOrDefault(a => a.CodeName == x.CodeName) != null)
            {
                x.isRegistered = false;
            }

            if (dCompletedMonsters.FirstOrDefault(a => a.CodeName == x.CodeName) != null)
            {
                x.isRegistered = true;
            }
        }
        Debug.Log("���� �ʱ�ȭ �Ϸ�");
    }


    // OnEnable���� ����
    private void OnEnableInit()
    {
        CurrentIndex = 0; // �ε��� �ʱ�ȭ
        dActiveMonsters = questSystem.ActiveAchievements; // Ȱ��ȭ�Ǿ��ִ� ���� ���� ����Ʈ
        dCompletedMonsters = questSystem.CompletedAchievements; // �Ϸ�� ���� ���� ����Ʈ

        VerticalLayoutGroup vlg = gameObject.GetComponentInChildren<VerticalLayoutGroup>();

        // ScrollField�� NameField �߰� �� �ʱ�ȭ
        for (int i = 0; i < dogamDB.DogamMonsters.Count; i++)
        {
            Object obj = Resources.Load("Quest/Quests/Dogam/NameField");
            GameObject instance = (GameObject)Instantiate(obj, vlg.transform);

            // TextField �ʱ�ȭ
            ImageControl img = instance.GetComponentInChildren<ImageControl>();
            img.bg = gameObject.GetComponentInChildren<InfiniteSnapScroll>();
            img.targetRect = targetRectTransform;

            // text �ʱ�ȭ
            TextMeshProUGUI tmp = instance.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = dogamDB.DogamMonsters[i].DisplayName;
        }
}


    // ���� �޼��� �̺�Ʈ ȣ��
    private void DogamQuestSynch(Quest quest)
    {
        DogamMonster ms = dogamDB.DogamMonsters.FirstOrDefault(x => x.CodeName == quest.CodeName);
        ms.isRegistered = true;
        ChangeDescriptionPage();
    }

    private void ChangeDescriptionPage()
    {
        if (dogamDB.DogamMonsters[CurrentIndex].isRegistered)
        {
            //Debug.Log("called");
            rewardButton.SetActive(true);
            monsterNameField.text = dogamDB.DogamMonsters[CurrentIndex].DisplayName;
            description.text = dogamDB.DogamMonsters[CurrentIndex].Description;
            monsterImageField.sprite = dogamDB.DogamMonsters[CurrentIndex].Image;

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

            for(int i=0; i < dogamDB.DogamMonsters[currentIndex].Skills.Count; i++)
            {
                Object obj = Resources.Load("Quest/Quests/Dogam/SkillSprite");
                GameObject instance = (GameObject)Instantiate(obj, skillField.transform);
                instance.GetComponent<SpriteRenderer>().sprite = dogamDB.DogamMonsters[currentIndex].Skills[i].Icon;
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
        GameManager.Instance.CinemachineTarget.enabled = true;
    }

    public void Reward()
    {
        if (!dogamDB.DogamMonsters[currentIndex].isRewardGiven)
        {
            Debug.Log("Rewards received successfully");
            dogamDB.DogamMonsters[currentIndex].isRewardGiven = true;
        }
        else
            Debug.Log("Rewards already received!");
    }

    private void OnEnable()
    {
        OnEnableInit();
        ChangeDescriptionPage();
        PlayerController.Instance.enabled = false;
    }

    private void OnDisable()
    {
        VerticalLayoutGroup vlg = gameObject.GetComponentInChildren<VerticalLayoutGroup>();
        foreach(Transform child in vlg.transform)
        { 
            Destroy(child.gameObject); 
        }
        PlayerController.Instance.enabled = true;
    }
}
