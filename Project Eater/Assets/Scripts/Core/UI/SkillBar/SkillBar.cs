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

    // ��ų ����� ������ Target SkillSystem : Player
    [SerializeField]
    private SkillSystem skillSystem;
    // ������ Slot�� ���� 
    // 1) 0 ~ 3 : Active Skill
    // 2) 4 ~ 7 : Passive Skill
    [SerializeField]
    private List<ActiveSkillSlot> activeSlots = new();
    [SerializeField]
    private List<PassiveSkillSlot> passiveSlots = new();

    private void OnEnable()
    {
        // ��ų �ý����� onSkillEquipped Event�� OnSkillEquipped �޼��� ��� 
        skillSystem.onSkillEquipped += OnSkillEquipped;
        skillSystem.onSkillDisarm += OnSkillDisarmed;

        // Active ��ų�� ���, ��ų ���Ű�� ������� �� 
        for (int i = 0; i < slotCount; i++)
            activeSlots[i].SetupActive((KeyCode)49 + i);
    }

    private void OnDisable()
    {
        skillSystem.onSkillEquipped -= OnSkillEquipped;
        skillSystem.onSkillDisarm -= OnSkillDisarmed;

        foreach (var slot in activeSlots)
            slot.Skill = null;
        foreach (var slot in passiveSlots)
            slot.Skill = null;
    }

    // ��ų�� Slot�� �߰��ϴ� �Լ� 
    private void TryEquipSlot(Skill skill, int keyNumber)
    {
        if (skill.Grade != SkillGrade.Latent && skill.Type == SkillType.Active)
        {
            activeSlots[keyNumber - 1].Skill = skill;
        }
        else if (skill.Grade != SkillGrade.Latent && skill.Type == SkillType.Passive)
        {
            passiveSlots[(keyNumber - 1) % 4].Skill = skill;
        }
    }

    // ��ų�� Slot���� �����ϴ� �Լ� 
    private void TryDisarmSlot(Skill skill, int keyNumber)
    {
        if (skill.Type == SkillType.Active)
        {
            activeSlots[keyNumber - 1].Skill = null;
        }
        else if (skill.Type == SkillType.Passive)
        {
            passiveSlots[(keyNumber - 1) % 4].Skill = null;
        }
    }

    private void OnSkillEquipped(SkillSystem skillSystem, Skill skill, int keyNumbder)
        => TryEquipSlot(skill, keyNumbder);

    private void OnSkillDisarmed(SkillSystem skillSystem, Skill skill, int keyNumbder)
        => TryDisarmSlot(skill, keyNumbder);
}
