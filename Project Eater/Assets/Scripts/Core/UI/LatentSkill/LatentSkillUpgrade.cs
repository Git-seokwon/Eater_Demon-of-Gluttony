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
    [SerializeField]
    private GameObject latentSkillEquipUI;
    [SerializeField]
    private GameObject latentSkillUpgradeUI;
    [SerializeField]
    private Button equipmentUIButton;
    [SerializeField]
    private Button upgradeUIButton;

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
    [SerializeField]
    private Button closeButton;

    [Space(10)]
    [Header("IMAGE")]
    [SerializeField]
    private Image latentSkillIcon;

    [Space(10)]
    [Header("LATENTSKILL_EQUIP")]
    [SerializeField]
    private Image latentEquipSkillIcon;
    [SerializeField]
    private Button equipButton;
    [SerializeField]
    private Button rightEquipButton;
    [SerializeField]
    private Button leftEquipButton;
    [SerializeField]
    private TextMeshProUGUI currentEquipLatentSkillLevel;
    [SerializeField]
    private TextMeshProUGUI currentEquipLatentSkillDescription;
    [SerializeField]
    private TextMeshProUGUI equipText;
    [SerializeField]
    private TextMeshProUGUI upgradeText;

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

        ownLatentSkills = null;
        currentLatentSkill = null;

        // ��ư Event ��� ����
        upgradeButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();
        leftButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();

        equipmentUIButton.onClick.RemoveAllListeners();
        upgradeUIButton.onClick.RemoveAllListeners();

        equipButton.onClick.RemoveAllListeners();
        rightEquipButton.onClick.RemoveAllListeners();
        leftEquipButton.onClick.RemoveAllListeners();
    }

    // ������ ��ư Ŭ�� ��, ���� �ع� ��ų�� ����
    private void NextLatentSkillChoice()
    {
        // ui button ȿ���� ���
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.uiButton);

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
        UpdateLatentSkillEquipUI();
        UpdateLatentSkillUpgradeUI();
    }

    // ���� ��ư Ŭ�� ��, ���� �ع� ��ų�� ���� 
    private void PrevLatentSkillChoice()
    {
        // ui button ȿ���� ���
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.uiButton);

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
        UpdateLatentSkillEquipUI();
        UpdateLatentSkillUpgradeUI();
    }

    // �ع� ��ų ��ȭ ��ư
    private void latentSkillUpgrade()
    {
        // ui button ȿ���� ���
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.uiButton);

        // ��ȭ ���� 
        GameManager.Instance.BaalFlesh = -upgradeCost_Baal_Flesh[currentUpgradeLevel];
        GameManager.Instance.Baal_GreatShard = -upgradeCost_Baal_GreatShard[currentUpgradeLevel];

        // �ع� ��ų ��ȭ 
        GameManager.Instance.player.LevelUpLatentSkill(currentLatentSkill);

        // �ع� ��ų �纻 ��ȭ 
        currentPassiveSkill.LevelUp(true);
        currentBasicAttackSkill.LevelUp(true);

        // ��ų ���� ǥ�� Update
        UpdateLatentSkillUpgradeUI();
        UpdateLatentSkillEquipUI();
    }

    private void LatentSkillEquip()
    {
        // ui button ȿ���� ���
        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.uiButton);

        GameManager.Instance.player.ChangeLatentSkill(currentSkillIndex); // �ع� ��ų ������ ����
    }

    private void UpdateLatentSkillUpgradeUI()
    {
        currentLatentSkillName.text = "/ " + currentLatentSkill.LatentSkillName;

        // �ع� ��ų ������ ���
        latentSkillIcon.sprite = currentLatentSkill.Skill[0].Icon;

        // ���� �� ���� ������Ʈ
        currentUpgradeLevel = Mathf.Max(0, currentPassiveSkill.Level - 1);

        currentLatentSkillLevel.text = currentPassiveSkill.Level.ToString();
        currentLatentSkillDescription.text =
            $"{currentPassiveSkill.DisplayName} - <color=yellow>�⺻ ���� ȿ��</color>\n{currentPassiveSkill.SpecificDescription}\n\n" +
            $"{currentBasicAttackSkill.DisplayName} - <color=yellow>�⺻ ����</color>\n{currentBasicAttackSkill.SpecificDescription}";

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
                $"{nextPassiveSkill.DisplayName} - <color=yellow>�⺻ ���� ȿ��</color>\n{nextPassiveSkill.SpecificDescription}\n\n" +
                $"{nextBasicAttackSkill.DisplayName} - <color=yellow>�⺻ ����</color>\n{nextBasicAttackSkill.SpecificDescription}";

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

    private void UpdateLatentSkillEquipUI()
    {
        currentLatentSkillName.text = "/ " + currentLatentSkill.LatentSkillName;

        // �ع� ��ų ������ ���
        latentEquipSkillIcon.sprite = currentLatentSkill.Skill[0].Icon;

        currentEquipLatentSkillLevel.text = currentPassiveSkill.Level.ToString();
        currentEquipLatentSkillDescription.text =
            $"{currentPassiveSkill.DisplayName} - <color=yellow>�⺻ ���� ȿ��</color>\n{currentPassiveSkill.SpecificDescription}\n\n" +
            $"{currentBasicAttackSkill.DisplayName} - <color=yellow>�⺻ ����</color>\n{currentBasicAttackSkill.SpecificDescription}";
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

        UpdateLatentSkillUpgradeUI();
        UpdateLatentSkillEquipUI();

        // ��ư Event ��� 
        upgradeButton.onClick.AddListener(latentSkillUpgrade);
        rightButton.onClick.AddListener(NextLatentSkillChoice);
        leftButton.onClick.AddListener(PrevLatentSkillChoice);
        closeButton.onClick.AddListener(Close);

        equipmentUIButton.onClick.AddListener(ShowEquipmentUI);
        upgradeUIButton.onClick.AddListener(ShowUpgradeUI);

        equipButton.onClick.AddListener(LatentSkillEquip);
        rightEquipButton.onClick.AddListener(NextLatentSkillChoice);
        leftEquipButton.onClick.AddListener(PrevLatentSkillChoice);

        gameObject.SetActive(true);
        ShowUpgradeUI();
    }

    // ��ų ������ ����ϱ� ���� �� ��ų���� ���� ������ �°� Setup�ؾ� �Ѵ�. 
    private void SetupSkills(Skill passiveSkill, Skill basicAttackSkill, int level)
    {
        passiveSkill.Setup(GameManager.Instance.player, level);
        basicAttackSkill.Setup(GameManager.Instance.player, level);
    }

    private void Close()
    {
        PlayerController.Instance.enabled = true;
        PlayerController.Instance.IsInterActive = false;
        GameManager.Instance.CinemachineTarget.enabled = true;

        // ��ų �纻 �����ϱ� 
        Destroy(currentPassiveSkill);
        Destroy(currentBasicAttackSkill);

        gameObject.SetActive(false);
    }

    private void ShowEquipmentUI()
    {
        latentSkillUpgradeUI.gameObject.SetActive(false);
        latentSkillEquipUI.gameObject.SetActive(true);

        equipText.color = Color.red;
        upgradeText.color = Color.white;
    }

    private void ShowUpgradeUI()
    {
        latentSkillEquipUI.gameObject.SetActive(false);
        latentSkillUpgradeUI.gameObject.SetActive(true);

        equipText.color = Color.white;
        upgradeText.color = Color.red;
    }
}
