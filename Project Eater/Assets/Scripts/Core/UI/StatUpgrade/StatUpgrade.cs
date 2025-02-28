using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeStats
{
    Fullness,
    Attack,
    Defence,
    CritRate,
    CritDamage,
    MoveSpeed,
    AbilityHaste,
    Absorption
}

public struct StatUpgradeData
{
    // 스텟 별 최대 강화 레벨
    public int maxLevel;
    // 강화 레벨에 따른 수치 값
    public float[] value;
}

public class StatUpgrade : MonoBehaviour
{
    [SerializeField]
    private StatUpgradeDB statUpgradeDB;

    [Space(10)]
    [SerializeField]
    private TextMeshProUGUI[] currentStatInfo = new TextMeshProUGUI[8]; // 현재 레벨 스텟 수치 UI
    [SerializeField]
    private TextMeshProUGUI[] currentUpgradeCost = new TextMeshProUGUI[8]; // 스텟 강화에 필요한 재화량 UI
    [SerializeField]
    private TextMeshProUGUI baalFlesh; // 현재 가지고 있는 재화량 UI

    [Space(10)]
    [SerializeField]
    private Button[] upgradeButtons = new Button[8]; // 업그레이드 버튼
    [SerializeField]
    private Button returnButton; // 돌아가기 버튼

    // 각 스텟의 강화 단계 
    [HideInInspector]
    public int[] currentStatUpgradeLevel = new int[8];
    // 각 단계별 강화에 필요한 재화량
    private float[] upgradeCost = new float[5];
    private StatUpgradeData[] statUpgradeDatas = new StatUpgradeData[8];
    // 플레이어 스텟 정보
    private Stats stats;

    private bool isLoading = false;

    private void Awake()
    {
        // upgradeCost 데이터 가져오기  
        for (int i = 0; i < statUpgradeDB.NeedBaalFlesh.Count; i++)
            upgradeCost[i] = statUpgradeDB.NeedBaalFlesh[i].value;

        // 스텟 강화 데이터 가져오기 
        foreach (UpgradeStats statType in System.Enum.GetValues(typeof(UpgradeStats)))
        {
            List<StatUpgradeDBEntity> dbList = statUpgradeDB.GetStatUpgradeList(statType);
            InitializeStatUpgradeData(statType, dbList);
        }
    }

    private void Start()
    {
        // 플레이어 Stat 정보들 가져오기 
        stats = GameManager.Instance.player.Stats;

        // 로드된 강화 단계를 적용
        ApplyLoadedStatUpgrades();

        isLoading = true;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (!isLoading) return;

        // ※ System.Enum.GetValues() : C#에서 열거형(Enum)에 정의된 모든 값을 배열로 반환하는 메서드
        // → 열거형의 모든 항목을 배열로 반환
        // → foreach 루프를 사용해서 UpgradeStats의 모든 항목을 하나씩 반복 처리하는 부분
        foreach (UpgradeStats statType in System.Enum.GetValues(typeof(UpgradeStats)))
        {
            // 현재 스텟 정보 출력하기
            InitializeCurrentStat(statType);

            // 현재 스텟 강화에 필요한 재화량 출력하기 
            InitializeCurrentUpgradeCost(statType);
        }

        // Event 등록 
        GameManager.Instance.onBaalFleshValueChanged += UpdateBaalFlesh; // 재화(바알의 살점) 수치 변화 Event 등록

        // 버튼 클릭 이벤트 추가
        returnButton.onClick.AddListener(Return); // 돌아가기 기능 Event 등록
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            int statIndex = i;
            UpgradeStats statType = (UpgradeStats)statIndex;

            upgradeButtons[i].onClick.AddListener(() => UpgradeStat(statType));
        }

        // 바알의 살점 UI 업데이트 
        GameManager.Instance.OnValueChanged();
    }

    private void OnDisable()
    {
        // Event 등록 해제  
        if (GameManager.Instance != null)
            GameManager.Instance.onBaalFleshValueChanged -= UpdateBaalFlesh;

        // 버튼 클릭 이벤트 해제
        returnButton.onClick.RemoveAllListeners();
        foreach (Button button in upgradeButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void InitializeStatUpgradeData(UpgradeStats statType, List<StatUpgradeDBEntity> dbList)
    {
        int statIndex = (int)statType;

        // maxLevel 설정
        statUpgradeDatas[statIndex].maxLevel = dbList.Count - 1;

        // value 배열 초기화
        statUpgradeDatas[statIndex].value = new float[dbList.Count];

        // value 값 복사
        for (int i = 0; i < dbList.Count; i++)
        {
            statUpgradeDatas[statIndex].value[i] = dbList[i].value;
        }
    }

    // 로드된 강화 단계를 기반으로 스탯을 설정하는 함수
    private void ApplyLoadedStatUpgrades()
    {
        foreach (UpgradeStats statType in System.Enum.GetValues(typeof(UpgradeStats)))
        {
            int statIndex = (int)statType;
            int upgradeLevel = currentStatUpgradeLevel[statIndex];

            if (upgradeLevel > 0) // 1레벨 이상 강화된 경우에만 적용
            {
                float upgradedValue = statUpgradeDatas[statIndex].value[upgradeLevel];

                switch (statType)
                {
                    case UpgradeStats.Fullness:
                        stats.FullnessStat.MaxValue = upgradedValue;
                        stats.SetDefaultValue(stats.FullnessStat, upgradedValue);
                        break;
                    case UpgradeStats.Attack:
                        stats.SetDefaultValue(stats.AttackStat, upgradedValue);
                        break;
                    case UpgradeStats.Defence:
                        stats.SetDefaultValue(stats.DefenceStat, upgradedValue);
                        break;
                    case UpgradeStats.CritRate:
                        stats.SetDefaultValue(stats.CritRateStat, upgradedValue);
                        break;
                    case UpgradeStats.CritDamage:
                        stats.SetDefaultValue(stats.CritDamageStat, upgradedValue);
                        break;
                    case UpgradeStats.MoveSpeed:
                        stats.SetDefaultValue(stats.MoveSpeedStat, upgradedValue);
                        break;
                    case UpgradeStats.AbilityHaste:
                        stats.SetDefaultValue(stats.AbilityHasteStat, upgradedValue);
                        break;
                    case UpgradeStats.Absorption:
                        stats.SetDefaultValue(stats.AbsorptionStat, upgradedValue);
                        break;
                }
            }
        }
    }

    // 스텟 정보 출력 함수 
    private void InitializeCurrentStat(UpgradeStats statType)
    {
        // 현재 스텟 타입 
        int statIndex = (int)statType;

        // %형 스텟이면 %로 표기
        if (statType == UpgradeStats.CritRate || statType == UpgradeStats.CritDamage
            || statType == UpgradeStats.AbilityHaste)
        {
            currentStatInfo[statIndex].text
                = statUpgradeDatas[statIndex].value[currentStatUpgradeLevel[statIndex]].ToString() + "%";
        }
        // 아니면 그냥 표기 
        else
        {
            currentStatInfo[statIndex].text
                = statUpgradeDatas[statIndex].value[currentStatUpgradeLevel[statIndex]].ToString();
        }
    }

    private void InitializeCurrentUpgradeCost(UpgradeStats statType)
    {
        int statIndex = (int)statType;

        currentUpgradeCost[statIndex].text = upgradeCost[currentStatUpgradeLevel[statIndex]].ToString();
    }

    public void UpgradeStat(UpgradeStats statType)
    {
        int statIndex = (int)statType;

        switch (statType)
        {
            case UpgradeStats.Fullness:
                ProcessUpgrade(statType, value => stats.SetDefaultValue(stats.FullnessStat, value));
                break;
            case UpgradeStats.Attack:
                ProcessUpgrade(statType, value => stats.SetDefaultValue(stats.AttackStat, value));
                break;
            case UpgradeStats.Defence:
                ProcessUpgrade(statType, value => stats.SetDefaultValue(stats.DefenceStat, value));
                break;
            case UpgradeStats.CritRate:
                ProcessUpgrade(statType, value => stats.SetDefaultValue(stats.CritRateStat, value));
                break;
            case UpgradeStats.CritDamage:
                ProcessUpgrade(statType, value => stats.SetDefaultValue(stats.CritDamageStat, value));
                break;
            case UpgradeStats.MoveSpeed:
                ProcessUpgrade(statType, value => stats.SetDefaultValue(stats.MoveSpeedStat, value));
                break;
            case UpgradeStats.AbilityHaste:
                ProcessUpgrade(statType, value => stats.SetDefaultValue(stats.AbilityHasteStat, value));
                break;
            case UpgradeStats.Absorption:
                ProcessUpgrade(statType, value => stats.SetDefaultValue(stats.AbsorptionStat, value));
                break;

            default:
                break;
        }
    }

    // 스텟 업그레이드 
    private void ProcessUpgrade(UpgradeStats statType, System.Action<float> setStatAction)
    {
        int statIndex = (int)statType;

        // 바알의 살점 재화 체크
        if (GameManager.Instance.BaalFlesh < upgradeCost[currentStatUpgradeLevel[statIndex]])
            return;

        // 재화 감소
        GameManager.Instance.BaalFlesh = -(int)upgradeCost[currentStatUpgradeLevel[statIndex]];

        // 스탯 강화 단계 증가
        currentStatUpgradeLevel[statIndex]++;

        // 스탯 강화
        if (statType == UpgradeStats.Fullness)
        {
            stats.FullnessStat.MaxValue = statUpgradeDatas[statIndex].value[currentStatUpgradeLevel[statIndex]];
        }
        setStatAction(statUpgradeDatas[statIndex].value[currentStatUpgradeLevel[statIndex]]);

        // 최대 강화 도달
        if (currentStatUpgradeLevel[statIndex] >= statUpgradeDatas[statIndex].maxLevel)
        {
            // 강화 버튼 비활성화 
            upgradeButtons[statIndex].interactable = false;
            // "최대"로 표기함으로써 최대 강화에 도달한 것을 알림 
            currentUpgradeCost[statIndex].text = "최대";
            // UI 갱신
            InitializeCurrentStat(statType);
            return;
        }

        // UI 갱신
        InitializeCurrentStat(statType);
        InitializeCurrentUpgradeCost(statType);
    }

    private void UpdateBaalFlesh(int currentValue, int prevValue)
        => baalFlesh.text = currentValue.ToString();

    private void Return()
    {
        PlayerController.Instance.IsInterActive = false;
        PlayerController.Instance.enabled = true;
        GameManager.Instance.CinemachineTarget.enabled = true;

        gameObject.SetActive(false);
    }
}
