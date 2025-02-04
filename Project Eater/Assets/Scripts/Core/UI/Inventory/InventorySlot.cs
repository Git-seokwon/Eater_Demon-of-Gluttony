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
                // ��ų ������ �Ҵ� 
                iconImage.sprite = slotSkill.Icon;
                // level Text �Ҵ�
                level.text = slotSkill.Level.ToString();

                // ���� �̹��� �Ҵ�
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

        // ANNIHILATION_SCYTHE ��ų�� ���, �÷��̾ �������� �����ϴ� ���� �ƴ� �ڵ������̱� ������ 
        // �׿� �°� Inventory Slot�� ��Ȳ�� ��������� �Ѵ�. 
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
        // Skill Tooltip �����ֱ� 
        if (SlotSkill != null)
            SkillTooltip.Instance.Show(SlotSkill);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Skill Tooltip ����
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
