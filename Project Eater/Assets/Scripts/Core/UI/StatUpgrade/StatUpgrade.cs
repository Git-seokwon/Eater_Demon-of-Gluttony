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
    // ���� �� �ִ� ��ȭ ����
    public int maxLevel;
    // ��ȭ ������ ���� ��ġ ��
    public float[] value;
}

public class StatUpgrade : MonoBehaviour
{
    [SerializeField]
    private StatUpgradeDB statUpgradeDB;

    [Space(10)]
    [SerializeField]
    private TextMeshProUGUI[] currentStatInfo = new TextMeshProUGUI[8]; // ���� ���� ���� ��ġ UI
    [SerializeField]
    private TextMeshProUGUI[] currentUpgradeCost = new TextMeshProUGUI[8]; // ���� ��ȭ�� �ʿ��� ��ȭ�� UI
    [SerializeField]
    private TextMeshProUGUI baalFlesh; // ���� ������ �ִ� ��ȭ�� UI

    [Space(10)]
    [SerializeField]
    private Button[] upgradeButtons = new Button[8]; // ���׷��̵� ��ư
    [SerializeField]
    private Button returnButton; // ���ư��� ��ư

    // �� ������ ��ȭ �ܰ� 
    private int[] currentStatUpgradeLevel = new int[8];
    // �� �ܰ躰 ��ȭ�� �ʿ��� ��ȭ��
    private float[] upgradeCost = new float[5];
    private StatUpgradeData[] statUpgradeDatas = new StatUpgradeData[8];
    // �÷��̾� ���� ����
    private Stats stats;

    private void Awake()
    {
        // upgradeCost �ʱ�ȭ 
        for (int i = 0; i < statUpgradeDB.NeedBaalFlesh.Count; i++)
            upgradeCost[i] = statUpgradeDB.NeedBaalFlesh[i].value;

        // ���� ��ȭ ������ �������� 
        foreach (UpgradeStats statType in System.Enum.GetValues(typeof(UpgradeStats)))
        {
            List<StatUpgradeDBEntity> dbList = statUpgradeDB.GetStatUpgradeList(statType);
            InitializeStatUpgradeData(statType, dbList);
        }

        gameObject.SetActive(false);
    }

    private void Start()
    {
        // �÷��̾� Stat ������ �������� 
        stats = GameManager.Instance.player.Stats;
    }

    private void OnEnable()
    {
        // �� System.Enum.GetValues() : C#���� ������(Enum)�� ���ǵ� ��� ���� �迭�� ��ȯ�ϴ� �޼���
        // �� �������� ��� �׸��� �迭�� ��ȯ
        // �� foreach ������ ����ؼ� UpgradeStats�� ��� �׸��� �ϳ��� �ݺ� ó���ϴ� �κ�
        foreach (UpgradeStats statType in System.Enum.GetValues(typeof(UpgradeStats)))
        {
            // ���� ���� ���� ����ϱ�
            InitializeCurrentStat(statType);

            // ���� ���� ��ȭ�� �ʿ��� ��ȭ�� ����ϱ� 
            InitializeCurrentUpgradeCost(statType);
        }

        // Event ��� 
        GameManager.Instance.onBaalFleshValueChanged += UpdateBaalFlesh; // ��ȭ(�پ��� ����) ��ġ ��ȭ Event ���

        // ��ư Ŭ�� �̺�Ʈ �߰�
        returnButton.onClick.AddListener(Return); // ���ư��� ��� Event ���
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            int statIndex = i;
            UpgradeStats statType = (UpgradeStats)statIndex;

            upgradeButtons[i].onClick.AddListener(() => UpgradeStat(statType));
        }

        // �پ��� ���� UI ������Ʈ 
        GameManager.Instance.OnValueChanged();
    }

    private void OnDisable()
    {
        // Event ��� ����  
        if (GameManager.Instance != null)
            GameManager.Instance.onBaalFleshValueChanged -= UpdateBaalFlesh;

        // ��ư Ŭ�� �̺�Ʈ ����
        returnButton.onClick.RemoveAllListeners();
        foreach (Button button in upgradeButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void InitializeStatUpgradeData(UpgradeStats statType, List<StatUpgradeDBEntity> dbList)
    {
        int statIndex = (int)statType;

        // maxLevel ����
        statUpgradeDatas[statIndex].maxLevel = dbList.Count - 1;

        // value �迭 �ʱ�ȭ
        statUpgradeDatas[statIndex].value = new float[dbList.Count];

        // value �� ����
        for (int i = 0; i < dbList.Count; i++)
        {
            statUpgradeDatas[statIndex].value[i] = dbList[i].value;
        }
    }

    private void InitializeCurrentStat(UpgradeStats statType)
    {
        int statIndex = (int)statType;

        if (statType == UpgradeStats.CritRate || statType == UpgradeStats.CritDamage
            || statType == UpgradeStats.AbilityHaste)
        {
            currentStatInfo[statIndex].text
                = statUpgradeDatas[statIndex].value[currentStatUpgradeLevel[statIndex]].ToString() + "%";
        }
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

    private void ProcessUpgrade(UpgradeStats statType, System.Action<float> setStatAction)
    {
        int statIndex = (int)statType;

        // �پ��� ���� ��ȭ üũ
        if (GameManager.Instance.BaalFlesh < upgradeCost[currentStatUpgradeLevel[statIndex]])
            return;

        // ��ȭ ����
        GameManager.Instance.BaalFlesh = -(int)upgradeCost[currentStatUpgradeLevel[statIndex]];

        // ���� ��ȭ �ܰ� ����
        currentStatUpgradeLevel[statIndex]++;

        // ���� ��ȭ
        if (statType == UpgradeStats.Fullness)
        {
            stats.FullnessStat.MaxValue = statUpgradeDatas[statIndex].value[currentStatUpgradeLevel[statIndex]];
        }
        setStatAction(statUpgradeDatas[statIndex].value[currentStatUpgradeLevel[statIndex]]);

        // �ִ� ��ȭ ����
        if (currentStatUpgradeLevel[statIndex] >= statUpgradeDatas[statIndex].maxLevel)
        {
            // ��ȭ ��ư ��Ȱ��ȭ 
            upgradeButtons[statIndex].interactable = false;
            // "�ִ�"�� ǥ�������ν� �ִ� ��ȭ�� ������ ���� �˸� 
            currentUpgradeCost[statIndex].text = "�ִ�";
            // UI ����
            InitializeCurrentStat(statType);
            return;
        }

        // UI ����
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
