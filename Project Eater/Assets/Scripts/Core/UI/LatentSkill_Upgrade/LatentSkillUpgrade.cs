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
    [SerializeField]
    private Button closeButton;

    // 강화 재료 → Awake 시점에서 latentSkillUpgradeDB로부터 정보를 가져와서 보관
    private int[] upgradeCost_Baal_GreatShard = new int[3];
    private int[] upgradeCost_Baal_Flesh = new int[3];

    // SetUp 함수를 통해 Player Entity로부터 할당 받는다. 
    private List<LatentSkillSlotNode> ownLatentSkills;
    private LatentSkillSlotNode currentLatentSkill;
    // 플레이어가 현재 장착하고 있는 해방 스킬의 Index
    private int currentSkillIndex = 0;
    private Skill currentBasicAttackSkill;
    private Skill currentPassiveSkill;
    private int maxLatentSkillsIndex = 0;

    private bool isMaxLevel = false;
    private bool hasSufficientCost = false;
    private int currentUpgradeLevel = 0;

    private void Awake()
    {
        // 강화 재료 정보 가져오기 
        for (int i = 0; i < latentSkillUpgradeDB.Baal_Flesh.Count; i++)
            upgradeCost_Baal_Flesh[i] = latentSkillUpgradeDB.Baal_Flesh[i].value;
        for (int i = 0; i < latentSkillUpgradeDB.Baal_GreatShard.Count; i++)
            upgradeCost_Baal_GreatShard[i] = latentSkillUpgradeDB.Baal_GreatShard[i].value;

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        // 비활성 문구들 비활성 해주기 
        maxUpgradeText.gameObject.SetActive(false);
        noCostWarningText.gameObject.SetActive(false);

        // 버튼 활성화 하기 
        upgradeButton.interactable = true;

        // 활성화 문구 활성화 하기 
        nextLatentSkillDescription.gameObject.SetActive(true);

        ownLatentSkills = null;
        currentLatentSkill = null;

        // 버튼 Event 등록 해제
        upgradeButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();
        leftButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            gameObject.SetActive(false);
        }
    }

    // 오른쪽 버튼 클릭 시, 다음 해방 스킬을 장착
    private void NextLatentSkillChoice()
    {
        // 현재 해방 스킬이 마지막 Index라면 return 
        if (maxLatentSkillsIndex <= currentSkillIndex)
            return;

        // 현재 스킬 사본 삭제하기 
        Destroy(currentPassiveSkill);
        Destroy(currentBasicAttackSkill);

        // 다음 해방 스킬 가져오기 
        currentSkillIndex++;
        currentLatentSkill = ownLatentSkills[currentSkillIndex];
        currentPassiveSkill = currentLatentSkill.Skill[0].Clone() as Skill;
        currentBasicAttackSkill = currentLatentSkill.Skill[1].Clone() as Skill;

        SetupSkills(currentPassiveSkill, currentBasicAttackSkill, currentLatentSkill.Level);

        // 스킬 정보 표기 Update
        UpdateLatentSkillUI();

        // 해방 스킬 실제로 변경
        GameManager.Instance.player.ChangeLatentSkill(currentSkillIndex);
    }

    // 왼쪽 버튼 클릭 시, 이전 해방 스킬을 장착 
    private void PrevLatentSkillChoice()
    {
        // 현재 해방 스킬이 맨 처음 Index라면 return 
        if (currentSkillIndex <= 0)
            return;

        // 현재 스킬 사본 삭제하기 
        Destroy(currentPassiveSkill);
        Destroy(currentBasicAttackSkill);

        // 다음 해방 스킬 가져오기 
        currentSkillIndex--;
        currentLatentSkill = ownLatentSkills[currentSkillIndex];
        currentPassiveSkill = currentLatentSkill.Skill[0].Clone() as Skill;
        currentBasicAttackSkill = currentLatentSkill.Skill[1].Clone() as Skill;

        SetupSkills(currentPassiveSkill, currentBasicAttackSkill, currentLatentSkill.Level);

        // 스킬 정보 표기 Update
        UpdateLatentSkillUI();

        // 해방 스킬 실제로 변경
        GameManager.Instance.player.ChangeLatentSkill(currentSkillIndex);
    }

    // 해방 스킬 강화 버튼
    private void latentSkillUpgrade()
    {
        // 재화 감소 
        GameManager.Instance.BaalFlesh = -upgradeCost_Baal_Flesh[currentUpgradeLevel];
        GameManager.Instance.Baal_GreatShard = -upgradeCost_Baal_GreatShard[currentUpgradeLevel];

        // 해방 스킬 강화 
        GameManager.Instance.player.LevelUpLatentSkill(currentLatentSkill);

        // 해방 스킬 사본 강화 
        currentPassiveSkill.LevelUp(true);
        currentBasicAttackSkill.LevelUp(true);

        // 스킬 정보 표기 Update
        UpdateLatentSkillUI();
    }

    private void UpdateLatentSkillUI()
    {
        currentLatentSkillName.text = "/ " + currentLatentSkill.LatentSkillName;

        // 레벨 및 설명 업데이트
        currentUpgradeLevel = Mathf.Max(0, currentPassiveSkill.Level - 1);

        currentLatentSkillLevel.text = currentPassiveSkill.Level.ToString();
        currentLatentSkillDescription.text =
            $"{currentPassiveSkill.DisplayName} - 기본 지속 효과\n{currentPassiveSkill.Description}\n\n" +
            $"{currentBasicAttackSkill.DisplayName} - 기본 공격\n{currentBasicAttackSkill.Description}";

        // 다음 레벨 확인 및 업데이트
        isMaxLevel = currentLatentSkill.IsMaxLevel;
        if (!isMaxLevel)
        {
            Skill nextPassiveSkill = currentPassiveSkill.Clone() as Skill;
            Skill nextBasicAttackSkill = currentBasicAttackSkill.Clone() as Skill;
            nextPassiveSkill.LevelUp(true);
            nextBasicAttackSkill.LevelUp(true);

            nextLatentSkillLevel.text = nextPassiveSkill.Level.ToString();
            nextLatentSkillDescription.text =
                $"{nextPassiveSkill.DisplayName} - 기본 지속 효과\n{nextPassiveSkill.Description}\n\n" +
                $"{nextBasicAttackSkill.DisplayName} - 기본 공격\n{nextBasicAttackSkill.Description}";

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

        // 강화 재료 및 보유 수량 업데이트 
        requiredUpgradeCostText_Baal_GreatShard.text = upgradeCost_Baal_GreatShard[currentUpgradeLevel].ToString();
        ownedUpgradeCost_Baal_GreatShard.text = "보유 : " + GameManager.Instance.Baal_GreatShard.ToString();
        requiredUpgradeCostText_Baal_Flesh.text = upgradeCost_Baal_Flesh[currentUpgradeLevel].ToString();
        ownedUpgradeCost_Baal_Flesh.text = "보유 : " + GameManager.Instance.BaalFlesh.ToString();

        // 강화 재료 충분 여부 체크
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

        // 버튼 Event 등록 
        upgradeButton.onClick.AddListener(latentSkillUpgrade);
        rightButton.onClick.AddListener(NextLatentSkillChoice);
        leftButton.onClick.AddListener(PrevLatentSkillChoice);
        closeButton.onClick.AddListener(Close);

        gameObject.SetActive(true);
    }

    // 스킬 정보를 출력하기 전에 각 스킬들을 현재 레벨에 맞게 Setup해야 한다. 
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

        // 스킬 사본 삭제하기 
        Destroy(currentPassiveSkill);
        Destroy(currentBasicAttackSkill);

        gameObject.SetActive(false);
    }
}
