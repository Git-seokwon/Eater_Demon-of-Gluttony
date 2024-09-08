using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SkillBar : MonoBehaviour
{
    // �� Skill Slot �ִ� ĭ �� 
    [SerializeField]
    private int slotCount;

    // ��ų ����� ������ Target SkillSystem
    [SerializeField]
    private SkillSystem skillSystem;
    // ������ Slot�� ���� 
    // 1) 0 ~ 3 : Active Skill
    // 2) 4 ~ 7 : Passive Skill
    [SerializeField]
    private List<ActiveSkillSlot> activeSlots = new();
    [SerializeField]
    private List<PassiveSkillSlot> passiveSlots = new();

    private bool IsActiveSkillSlotFull => activeSlots.Count >= slotCount;
    private bool IsPassiveSkillSlotFull => passiveSlots.Count >= slotCount;
    private bool IsActiveSkillSlotEmpty => activeSlots.Count <= 0;
    private bool IsPassiveSkillSlotEmpty => passiveSlots.Count <= 0;

    private void Start()
    {
        // ��ų �ý����� onSkillEquipped Event�� OnSkillEquipped �޼��� ��� 
        skillSystem.onSkillEquipped += OnSkillEquipped;
        skillSystem.onSkillDisarm += OnSkillDisarmed;

        // Active ��ų�� ���, ��ų ���Ű�� ������� �� 
        for (int i = 0; i < slotCount; i++)
            activeSlots[i].SetupActive((KeyCode)49 + i);
    }

    private void OnDestroy()
    {
        skillSystem.onSkillEquipped -= OnSkillEquipped;
        skillSystem.onSkillDisarm -= OnSkillDisarmed;
    }

    // ��ų�� Slot�� �߰��ϴ� �Լ� 
    private void TryEquipSlot(Skill skill, int keyNumber)
    {
        // keyNumbder�� -1�̸� �ع� ��ų�̹Ƿ� slot ��� X
        if (keyNumber == -1) return;

        if (skill.Type == SkillType.Active && !IsActiveSkillSlotFull)
        {
            activeSlots[keyNumber].Skill = skill;
        }
        else if (skill.Type == SkillType.Passive && !IsPassiveSkillSlotFull)
        {
            passiveSlots[keyNumber].Skill = skill;
        }
    }

    // ��ų�� Slot���� �����ϴ� �Լ� 
    private void TryDisarmSlot(Skill skill, int keyNumber)
    {
        // keyNumbder�� -1�̸� �ع� ��ų�̹Ƿ� slot ���� X
        if (keyNumber == -1) return;

        if (skill.Type == SkillType.Active && !IsActiveSkillSlotEmpty)
        {
            activeSlots[keyNumber].Skill = null;
        }
        else if (skill.Type == SkillType.Passive && !IsPassiveSkillSlotEmpty)
        {
            passiveSlots[keyNumber].Skill = null;
        }
    }

    private void OnSkillEquipped(SkillSystem skillSystem, Skill skill, int keyNumbder)
        => TryEquipSlot(skill, keyNumbder);

    private void OnSkillDisarmed(SkillSystem skillSystem, Skill skill, int keyNumbder)
        => TryDisarmSlot(skill, keyNumbder);
}
