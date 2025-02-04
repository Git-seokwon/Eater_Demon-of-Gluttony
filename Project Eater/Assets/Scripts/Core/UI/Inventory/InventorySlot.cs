using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    protected Image borderImage;
    [SerializeField]
    private Image highlightImage;
    [SerializeField]
    private TextMeshProUGUI level;

    private SkillInventory skillInventory;
    private Skill slotSkill;
    private CanvasGroup canvasGroup;

    public Skill SlotSkill
    {
        get => slotSkill;
        set
        {
            slotSkill = value;

            if (slotSkill != null)
            {
                // 스킬 아이콘 할당 
                iconImage.sprite = slotSkill.Icon;
                // level Text 할당
                level.text = slotSkill.Level.ToString();

                // 슬롯 이미지 할당
                borderImage.sprite = GameResources.Instance.GetBorderImageByGrade(slotSkill.Grade);

                if (!gameObject.activeSelf)
                    gameObject.SetActive(true);
            }
            else
                gameObject.SetActive(false);
        }
    }

    private void Awake() => canvasGroup = GetComponent<CanvasGroup>();

    private void OnEnable()
    {
        var skillSystem = GameManager.Instance.player.SkillSystem;

        skillSystem.onSkillEquipped += OnSkillEquipped;
        skillSystem.onSkillDisarm += OnSkillDisarmed;

        if (SlotSkill == null) return;

        // ANNIHILATION_SCYTHE 스킬의 경우, 플레이어가 수동으로 장착하는 것이 아닌 자동장착이기 때문에 
        // 그에 맞게 Inventory Slot의 상황도 변경해줘야 한다. 
        bool isEquipped = SlotSkill.CodeName == "ANNIHILATION_SCYTHE" && skillSystem.ContainsInequippedskills(SlotSkill)
                         || skillSystem.FindEquippedSkill(SlotSkill);

        UpdateSlotVisuals(isEquipped);
    }

    private void OnDisable()
    {
        GameManager.Instance.player.SkillSystem.onSkillEquipped -= OnSkillEquipped;
        GameManager.Instance.player.SkillSystem.onSkillDisarm -= OnSkillDisarmed;

        SlotSkill = null;
    }

    public void Setup(Skill skill, SkillInventory skillInventory)
    {
        this.skillInventory = skillInventory;
        SlotSkill = skill;
    }

    public void SetHighlight(bool isActive) => highlightImage.gameObject.SetActive(isActive);

    public Transform GetTransform() => skillInventory.transform;

    public CanvasGroup GetActiveSkillsInEquip() => skillInventory.activeSkills;
    public CanvasGroup GetPassiveSkillsInEquip() => skillInventory.passiveSkills;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Skill Tooltip 보여주기 
        if (SlotSkill != null)
            SkillTooltip.Instance.Show(SlotSkill);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Skill Tooltip 끄기
        if (SlotSkill != null)
            SkillTooltip.Instance.Hide();
    }

    private void OnSkillEquipped(SkillSystem skillSystem, Skill skill, int keyNumbder)
    {
        if (skill.ID != SlotSkill.ID)
            return;

        SetAlpha(1f);
        SetHighlight(true);
    }

    private void OnSkillDisarmed(SkillSystem skillSystem, Skill skill, int keyNumbder)
    {
        if (skill.ID != SlotSkill.ID)
            return;

        SetAlpha(0.5f);
        SetHighlight(false);
    }

    private void SetAlpha(float alpha)
    {
        alpha = Mathf.Clamp01(alpha);
        canvasGroup.alpha = alpha;
    }

    private void UpdateSlotVisuals(bool isEquipped)
    {
        SetAlpha(isEquipped ? 1f : 0.5f);
        SetHighlight(isEquipped);
    }
}
