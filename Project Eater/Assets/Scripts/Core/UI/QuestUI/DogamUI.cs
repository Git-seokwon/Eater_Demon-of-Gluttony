using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class DogamUI : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private RectTransform targetRectTransform;
    [SerializeField] private TextMeshProUGUI monsterNameField;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image monsterImageField;
    [SerializeField] private GameObject rewardButton;
    
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

    private void Awake()
    {
        Initialize();
        Close();
    }

    private void Initialize()
    {
        questSystem = QuestSystem.Instance; // �ν��Ͻ�
        dogamDB = Resources.Load<DogamDB>("Quest/DogamDB"); // ���� DB �ε�

        currentIndex = 0; // �ε��� �ʱ�ȭ

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
        currentIndex = 0; // �ε��� �ʱ�ȭ
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
        PlayerController.Instance.enabled = false;
        Time.timeScale = 0;
}


    // ���� �޼��� �̺�Ʈ ȣ��
    private void DogamQuestSynch(Quest quest)
    {
        DogamMonster ms = dogamDB.DogamMonsters.FirstOrDefault(x => x.CodeName == quest.CodeName);
        ms.isRegistered = true;
        Debug.Log("DogamQuestSynch�� �۵��Ǿ����ϴ�.");
    }

    private void ChangeDescriptionPage()
    {
        if (dogamDB.DogamMonsters[currentIndex].isRegistered)
        {
            rewardButton.SetActive(true);
            monsterNameField.text = dogamDB.DogamMonsters[currentIndex].DisplayName;
            description.text = dogamDB.DogamMonsters[currentIndex].Description;
            monsterImageField.sprite = dogamDB.DogamMonsters[currentIndex].Image;
        }
        else
        {
            rewardButton.SetActive(false);
            monsterNameField.text = "???";
            description.text = "???";
            monsterImageField.sprite = null;
        }
    }
    public void Open()
    {
        gameObject.SetActive(true);
    }


    public void Close()
    {
        gameObject.SetActive(false);
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
    }

    private void OnDisable()
    {
        VerticalLayoutGroup vlg = gameObject.GetComponentInChildren<VerticalLayoutGroup>();
        for(int i=0; i < vlg.transform.childCount; i++)
        {
            Destroy(vlg.transform.GetChild(i).gameObject);  
        }
        PlayerController.Instance.enabled = true;
        Time.timeScale = 1;
    }
}
