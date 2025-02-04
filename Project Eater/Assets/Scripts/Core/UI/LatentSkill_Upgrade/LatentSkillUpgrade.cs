using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LatentSkillUpgrade : MonoBehaviour
{
    [SerializeField]
    private LatentSkillUpgradeDB latentSkillUpgradeDB;

    [Space(10)]
    [Header("TEXT")]
    [SerializeField]
    private TextMeshProUGUI currentLatentSkillName;
    [SerializeField]
    private TextMeshProUGUI currentLatentSkillLevel;
    [SerializeField]
    private TextMeshProUGUI nextLatentSkillLevel;
    [SerializeField]
    private TextMeshProUGUI currentLatentSkillDescription;
    [SerializeField]
    private TextMeshProUGUI nextLatentSkillDescription;
    [SerializeField]
    private TextMeshProUGUI maxUpgradeText;
    [SerializeField]
    private TextMeshProUGUI noCostWarningText;
    [SerializeField]
    private TextMeshProUGUI requiredUpgradeCostText_Baal_Flesh;
    [SerializeField]
    private TextMeshProUGUI ownedUpgradeCost_Baal_Flesh;
    [SerializeField]
    private TextMeshProUGUI requiredUpgradeCostText_Baal_GreatShard;
    [SerializeField]
    private TextMeshProUGUI ownedUpgradeCost_Baal_GreatShard;

    [Space(10)]
    [Header("BUTTON")]
    [SerializeField]
    private Button upgradeButton;
    [SerializeField]
    private Button rightButton;
    [SerializeField]
    private Button leftButton;

    [Space(10)]
    [Header("IMAGE")]
    [SerializeField]
    private Image latentSkillIcon;

    [Space(10)]
    [SerializeField]
    private LatentSkillSlot latentSlot;

    // ��ȭ ��� �� Awake �������� latentSkillUpgradeDB�κ��� ������ �����ͼ� ����
    private int[] upgradeCost_Baal_GreatShard = new int[3];
    private int[] upgradeCost_Baal_Flesh = new int[3];

    // SetUp �Լ��� ���� Player Entity�κ��� �Ҵ� �޴´�. 
    private List<LatentSkillSlotNode> ownLatentSkills;
    private LatentSkillSlotNode currentLatentSkill;
    // �÷��̾ ���� �����ϰ� �ִ� �ع� ��ų�� Index
    private int currentSkillIndex = 0;
    private Skill currentBasicAttackSkill;
    private Skill currentPassiveSkill;
    private int maxLatentSkillsIndex = 0;

    private bool isMaxLevel = false;
    private bool hasSufficientCost = false;
    private int currentUpgradeLevel = 0;

    private void Awake()
    {
        // ��ȭ ��� ���� �������� 
        for (int i = 0; i < latentSkillUpgradeDB.Baal_Flesh.Count; i++)
            upgradeCost_Baal_Flesh[i] = latentSkillUpgradeDB.Baal_Flesh[i].value;
        for (int i = 0; i < latentSkillUpgradeDB.Baal_GreatShard.Count; i++)
            upgradeCost_Baal_GreatShard[i] = latentSkillUpgradeDB.Baal_GreatShard[i].value;

        // ��ư Event ��� 
        upgradeButton.onClick.AddListener(latentSkillUpgrade);
        rightButton.onClick.AddListener(NextLatentSkillChoice);
        leftButton.onClick.AddListener(PrevLatentSkillChoice);

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        // ��Ȱ�� ������ ��Ȱ�� ���ֱ� 
        maxUpgradeText.gameObject.SetActive(false);
        noCostWarningText.gameObject.SetActive(false);

        // ��ư Ȱ��ȭ �ϱ� 
        upgradeButton.interactable = true;

        // Ȱ��ȭ ���� Ȱ��ȭ �ϱ� 
        nextLatentSkillDescription.gameObject.SetActive(true);

        // ��ų �纻 �����ϱ� 
        Destroy(currentPassiveSkill);
        Destroy(currentBasicAttackSkill);
        ownLatentSkills = null;
        currentLatentSkill = null;

        if (PlayerController.Instance != null)
            PlayerController.Instance.enabled = true;

        if (GameManager.Instance != null)
            GameManager.Instance.CinemachineTarget.enabled = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            gameObject.SetActive(false);
        }
    }

    // ������ ��ư Ŭ�� ��, ���� �ع� ��ų�� ����
    private void NextLatentSkillChoice()
    {
        // ���� �ع� ��ų�� ������ Index��� return 
        if (maxLatentSkillsIndex <= currentSkillIndex)
            return;

        // ���� ��ų �纻 �����ϱ� 
        Destroy(currentPassiveSkill);
        Destroy(currentBasicAttackSkill);

        // ���� �ع� ��ų �������� 
        currentSkillIndex++;
        currentLatentSkill = ownLatentSkills[currentSkillIndex];
        currentPassiveSkill = currentLatentSkill.Skill[0].Clone() as Skill;
        currentBasicAttackSkill = currentLatentSkill.Skill[1].Clone() as Skill;

        SetupSkills(currentPassiveSkill, currentBasicAttackSkill, currentLatentSkill.Level);

        // ��ų ���� ǥ�� Update
        UpdateLatentSkillUI();

        // �ع� ��ų ������ ����
        GameManager.Instance.player.ChangeLatentSkill(currentSkillIndex);
    }

    // ���� ��ư Ŭ�� ��, ���� �ع� ��ų�� ���� 
    private void PrevLatentSkillChoice()
    {
        // ���� �ع� ��ų�� �� ó�� Index��� return 
        if (currentSkillIndex <= 0)
            return;

        // ���� ��ų �纻 �����ϱ� 
        Destroy(currentPassiveSkill);
        Destroy(currentBasicAttackSkill);

        // ���� �ع� ��ų �������� 
        currentSkillIndex--;
        currentLatentSkill = ownLatentSkills[currentSkillIndex];
        currentPassiveSkill = currentLatentSkill.Skill[0].Clone() as Skill;
        currentBasicAttackSkill = currentLatentSkill.Skill[1].Clone() as Skill;

        SetupSkills(currentPassiveSkill, currentBasicAttackSkill, currentLatentSkill.Level);

        // ��ų ���� ǥ�� Update
        UpdateLatentSkillUI();

        // �ع� ��ų ������ ����
        GameManager.Instance.player.ChangeLatentSkill(currentSkillIndex);
    }

    // �ع� ��ų ��ȭ ��ư
    private void latentSkillUpgrade()
    {
        // ��ȭ ���� 
        GameManager.Instance.BaalFlesh = -upgradeCost_Baal_Flesh[currentUpgradeLevel];
        GameManager.Instance.Baal_GreatShard = -upgradeCost_Baal_GreatShard[currentUpgradeLevel];

        // �ع� ��ų ��ȭ 
        currentLatentSkill.LatentSkillLevelUp();

        // �ع� ��ų �纻 ��ȭ 
        currentPassiveSkill.LevelUp(true);
        currentBasicAttackSkill.LevelUp(true);

        // ��ų ���� ǥ�� Update
        UpdateLatentSkillUI();
    }

    private void UpdateLatentSkillUI()
    {
        currentLatentSkillName.text = "/ " + currentLatentSkill.LatentSkillName;
        latentSkillIcon.sprite = currentPassiveSkill.Icon;
        // Player HUD �ع� ��ų ������ ǥ�� 
        latentSlot.Skill = currentPassiveSkill;

        // ���� �� ���� ������Ʈ
        currentUpgradeLevel = Mathf.Max(0, currentPassiveSkill.Level - 1);

        currentLatentSkillLevel.text = currentPassiveSkill.Level.ToString();
        currentLatentSkillDescription.text =
            $"{currentPassiveSkill.DisplayName} - �⺻ ���� ȿ��\n{currentPassiveSkill.Description}\n\n" +
            $"{currentBasicAttackSkill.DisplayName} - �⺻ ����\n{currentBasicAttackSkill.Description}";

        // ���� ���� Ȯ�� �� ������Ʈ
        isMaxLevel = currentLatentSkill.IsMaxLevel;
        if (!isMaxLevel)
        {
            Skill nextPassiveSkill = currentPassiveSkill.Clone() as Skill;
            Skill nextBasicAttackSkill = currentBasicAttackSkill.Clone() as Skill;
            nextPassiveSkill.LevelUp(true);
            nextBasicAttackSkill.LevelUp(true);

            nextLatentSkillLevel.text = nextPassiveSkill.Level.ToString();
            nextLatentSkillDescription.text =
                $"{nextPassiveSkill.DisplayName} - �⺻ ���� ȿ��\n{nextPassiveSkill.Description}\n\n" +
                $"{nextBasicAttackSkill.DisplayName} - �⺻ ����\n{nextBasicAttackSkill.Description}";

            Destroy(nextPassiveSkill);
            Destroy(nextBasicAttackSkill);

            nextLatentSkillDescription.gameObject.SetActive(true);
            maxUpgradeText.gameObject.SetActive(false);
        }
        else
        {
            nextLatentSkillLevel.text = currentPassiveSkill.Level.ToString();
            nextLatentSkillDescription.gameObject.SetActive(false);
            maxUpgradeText.gameObject.SetActive(true);
            upgradeButton.enabled = false;
        }

        // ��ȭ ��� �� ���� ���� ������Ʈ 
        requiredUpgradeCostText_Baal_GreatShard.text = upgradeCost_Baal_GreatShard[currentUpgradeLevel].ToString();
        ownedUpgradeCost_Baal_GreatShard.text = "���� : " + GameManager.Instance.Baal_GreatShard.ToString();
        requiredUpgradeCostText_Baal_Flesh.text = upgradeCost_Baal_Flesh[currentUpgradeLevel].ToString();
        ownedUpgradeCost_Baal_Flesh.text = "���� : " + GameManager.Instance.BaalFlesh.ToString();

        // ��ȭ ��� ��� ���� üũ
        hasSufficientCost =
            GameManager.Instance.BaalFlesh >= upgradeCost_Baal_Flesh[currentUpgradeLevel] &&
            GameManager.Instance.Baal_GreatShard >= upgradeCost_Baal_GreatShard[currentUpgradeLevel];

        if (upgradeButton.interactable)
            upgradeButton.interactable = hasSufficientCost;

        noCostWarningText.gameObject.SetActive(!hasSufficientCost);
    }

    public void SetUp(List<LatentSkillSlotNode> ownLatentSkills, LatentSkillSlotNode currentLatentSkill)
    {
        this.ownLatentSkills = ownLatentSkills;
        this.currentLatentSkill = currentLatentSkill;

        currentSkillIndex = ownLatentSkills.IndexOf(currentLatentSkill);
        maxLatentSkillsIndex = ownLatentSkills.Count - 1;

        currentPassiveSkill = this.currentLatentSkill.Skill[0].Clone() as Skill;
        currentBasicAttackSkill = this.currentLatentSkill.Skill[1].Clone() as Skill;

        SetupSkills(currentPassiveSkill, currentBasicAttackSkill, this.currentLatentSkill.Level);

        UpdateLatentSkillUI();

        gameObject.SetActive(true);
    }

    // ��ų ������ ����ϱ� ���� �� ��ų���� ���� ������ �°� Setup�ؾ� �Ѵ�. 
    private void SetupSkills(Skill passiveSkill, Skill basicAttackSkill, int level)
    {
        passiveSkill.Setup(GameManager.Instance.player, level);
        basicAttackSkill.Setup(GameManager.Instance.player, level);
    }
}
