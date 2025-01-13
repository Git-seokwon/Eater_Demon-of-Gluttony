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
    
    private QuestSystem questSystem; // 업적 관리용 QuestSystem
    private DogamDB dogamDB; // 도감 몬스터 DB
    private IReadOnlyList<Quest> dActiveMonsters;
    private IReadOnlyList<Quest> dCompletedMonsters;
    

    private DogamMonster currentMonster; // 혹시 몰라서 일단 추가.
    
    private int currentIndex;
    public int CurrentIndex
    {
        get { return currentIndex; }
        set
        {
            currentIndex = Mathf.Clamp(value, 0, dogamDB.DogamMonsters.Count);
            ChangeDescriptionPage();
            // Debug.Log(currentIndex); // 2까지 작동하는거 확인~
        }
    }

    private void Awake()
    {
        Initialize();
        Close();
    }

    private void Initialize()
    {
        questSystem = QuestSystem.Instance; // 인스턴스
        dogamDB = Resources.Load<DogamDB>("Quest/DogamDB"); // 도감 DB 로드

        currentIndex = 0; // 인덱스 초기화

        dActiveMonsters = questSystem.ActiveAchievements; // 활성화되어있는 업적 몬스터 리스트
        dCompletedMonsters = questSystem.CompletedAchievements; // 완료된 업적 몬스터 리스트
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
        Debug.Log("도감 초기화 완료");
    }


    // OnEnable에서 쓸거
    private void OnEnableInit()
    {
        currentIndex = 0; // 인덱스 초기화
        dActiveMonsters = questSystem.ActiveAchievements; // 활성화되어있는 업적 몬스터 리스트
        dCompletedMonsters = questSystem.CompletedAchievements; // 완료된 업적 몬스터 리스트

        VerticalLayoutGroup vlg = gameObject.GetComponentInChildren<VerticalLayoutGroup>();

        // ScrollField에 NameField 추가 및 초기화
        for (int i = 0; i < dogamDB.DogamMonsters.Count; i++)
        {
            Object obj = Resources.Load("Quest/Quests/Dogam/NameField");
            GameObject instance = (GameObject)Instantiate(obj, vlg.transform);

            // TextField 초기화
            ImageControl img = instance.GetComponentInChildren<ImageControl>();
            img.bg = gameObject.GetComponentInChildren<InfiniteSnapScroll>();
            img.targetRect = targetRectTransform;

            // text 초기화
            TextMeshProUGUI tmp = instance.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = dogamDB.DogamMonsters[i].DisplayName;
        }
        PlayerController.Instance.enabled = false;
        Time.timeScale = 0;
}


    // 업적 달성시 이벤트 호출
    private void DogamQuestSynch(Quest quest)
    {
        DogamMonster ms = dogamDB.DogamMonsters.FirstOrDefault(x => x.CodeName == quest.CodeName);
        ms.isRegistered = true;
        Debug.Log("DogamQuestSynch가 작동되었습니다.");
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
