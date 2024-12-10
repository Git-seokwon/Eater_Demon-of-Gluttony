using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour, IDropHandler
{
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private int slotIndex;
    [SerializeField]
    private bool isActive;

    private Skill slotSkill;

    public Skill SlotSkill
    {
        get => slotSkill;
        set
        {
            if (value == null)
            {
                slotSkill = null;
                iconImage.sprite = null;
                SetIconAlpha(0f);
                return;
            }
            if (slotSkill == value)
                return;

            // �ߺ� ��� ���� 
            HandleDuplicateSkill(value);

            // �� ���ǹ� ������ prevSlotSkill�� slotSkill�� �ٸ���.
            var prevSlotSkill = slotSkill;
            slotSkill = value;

            iconImage.sprite = slotSkill.Icon;
            SetIconAlpha(1f);

            var skillSystem = GameManager.Instance.player.SkillSystem;
            // ��ų ��ü : ���� ������ ��ų ���� 
            if (prevSlotSkill != null)
                skillSystem.Disarm(prevSlotSkill, prevSlotSkill.skillKeyNumber);

            skillSystem.Equip(slotSkill, slotIndex);
        }
    }

    private void OnEnable()
    {
        CheckAndEquipEvolvedSkill("DEATHSCYTHE", "DEATHSCYTHE_EVOLVE");

        // ��ų �������� ���� ������ ��ų�� �Ҹ�� ���, Slot���� ����� 
        if (SlotSkill != null && !GameManager.Instance.player.SkillSystem.ContainsInequippedskills(SlotSkill))
            SlotSkill = null;
    }

    // �ش� ��ũ��Ʈ�� ������Ʈ�� �߰� �� ���� ������Ʈ RactTransform ���� ������ ����� �߻��ϸ� ����Ǵ� �ݹ� �Լ�
    public void OnDrop(PointerEventData eventData)
    {
        if (IconDrag.skill == null)
            return;

        if ((isActive && IconDrag.skill.Type == SkillType.Active) ||
           (!isActive && IconDrag.skill.Type == SkillType.Passive))
        {
            SlotSkill = IconDrag.skill;
        }
    }

    private void SetIconAlpha(float alpha)
    {
        alpha = Mathf.Clamp01(alpha);
        Color color = iconImage.color;
        color.a = alpha;
        iconImage.color = color;
    }

    private void HandleDuplicateSkill(Skill value)
    {
        var equipSlots = isActive ? GameManager.Instance.EquipActiveSlots : GameManager.Instance.EquipPassiveSlots;
        foreach (var equipSlot in equipSlots)
        {
            if (equipSlot != this && equipSlot.slotSkill?.ID == value.ID)
            {
                equipSlot.ClearSlot();
                break;
            }
        }
    }

    private void ClearSlot()
    {
        GameManager.Instance.player.SkillSystem.Disarm(SlotSkill, SlotSkill.skillKeyNumber);
        SlotSkill = null;
    }

    private void CheckAndEquipEvolvedSkill(string baseSkillCodeName, string evolvedSkillCodeName)
    {
        var skillSystem = GameManager.Instance.player.SkillSystem;
        if (SlotSkill?.CodeName == baseSkillCodeName && skillSystem.ContainsInequippedskills(evolvedSkillCodeName))
            SlotSkill = skillSystem.FindEquippedSkill(evolvedSkillCodeName);
    }
}
